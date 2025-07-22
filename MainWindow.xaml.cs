using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MMACollaboratorHelper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler OnSearchBandsParams;
        public static event EventHandler OnProcessBandParams;
        public static event EventHandler OnProcessAlbumParams;

        private int nbNewAlbumsProcessed_;
        private bool downloadNewAlbumsOnly_;
        private Thread threadProcess_;
        private bool isProcessing_;

        private const string tooltipTextboxBandStr =
            "Enter band name, prefix or EM URL (case insensitive)"
            + "\n\nExample:"
            + "\n - \"iron maiden\" will search band Iron Maiden,"
            + "\n - \"iro\" will search bands Iron Maiden, Iron Savior..."
            + "\n - \"https://www.metal-archives.com/bands/Abigail/1282\" will search band Abigail from Japan"
            + "\n    (the prefix \"https://\") is optional";

        private delegate void changeProgressLabel(string str);

        public MainWindow()
        {
            InitializeComponent();

            textboxBand.ToolTip = tooltipTextboxBandStr;

            labelVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            OnSearchBandsParams += Form1_OnSearchBandsParams;
            OnProcessBandParams += Form1_OnProcessBandParams;
            OnProcessAlbumParams += Form1_OnProcessAlbumParams;

            nbNewAlbumsProcessed_ = 0;
            downloadNewAlbumsOnly_ = true; // default
            isProcessing_ = false;

            Tools.Initialize();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tools.InitLogsFiles();

            // check if required dlls are present
            string execDir = Environment.CurrentDirectory;
            string dllName = "HtmlAgilityPack.dll";
            string dllPath = System.IO.Path.Combine(execDir, dllName);
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("Library " + dllName + " not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // parse genres
            string urlSite = "https://www.metalmusicarchives.com/";
            //Tools.LogEvent("Searching albums of " + band + "...");
            string sourceHTML = Tools.GetWebPageSourceHTML(urlSite);
            MMAParseGenres parsedGenres = new MMAParseGenres(sourceHTML);

            InitComboboxLetters();
            InitComboboxGenres(parsedGenres);

            // for debug purposes only
            //textboxBand.Text = "blind guardian";
            //textboxBand.Text = "cirith ungol";
            //textboxBand.Text = "cirith";
            //textboxBand.Text = "Mael Mórdha";
            //textboxBand.Text = "Iron Maiden";
            //textboxBand.Text = "Deep Purple";
            //textboxBand.Text = "jjkh9878g7fhghgf";
            //textboxBand.Text = "amo";
            //textboxBand.Text = "killswitch engage";
            //textboxBand.Text = "abigail";
            //textboxBand.Text = "www.metal-archives.com/bands/Abigail";
            //textboxBand.Text = "www.metal-archives.com/bands/Abigail/1282";
            //textboxBand.Text = "www.metal-archives.com/bands/Blind_Guardian/3";
            textboxBand.Text = "www.metal-archives.com/bands/Airs/3540301744";
            //textboxBand.Text = "www.metal-archives.com/bands/La_Torture_des_T%C3%A9n%C3%A8bres/3540410503";
        }

        #region Combo boxes initialization

        public void InitComboboxGenres(MMAParseGenres parsedGenres)
        {
            comboboxGenres.DisplayMemberPath = "Genre";
            comboboxGenres.SelectedValuePath = "Url";

            // fill combobox
            int index = 0;
            foreach (string genre in parsedGenres.SubGenres)
            {
                string url = parsedGenres.SubGenresURLs[index];

                comboboxGenres.Items.Add(new GenreURL(genre, url));
                index++;
            }

            comboboxGenres.SelectedIndex = 0;
        }

        public void InitComboboxLetters()
        {
            // fill combobox
            comboboxFilters.Items.Add("ALL");
            comboboxFilters.Items.Add("0-9");
            for (char letter = 'A'; letter <= 'Z'; letter++)
                comboboxFilters.Items.Add(letter.ToString());

            comboboxFilters.SelectedIndex = 2; // "A"
        }

        #endregion

        #region Interface callbacks

        private void buttonGo_Click(object sender, RoutedEventArgs e)
        {
            // stop process
            if (isProcessing_)
            {
                threadProcess_?.Abort();

                Tools.AbortWebRequest();

                Tools.LogEvent("");
                Tools.LogEvent("SEARCH STOPPED BY USER");
                isProcessing_ = false;
                updateGUI();
                return;
            }

            // start process

            downloadNewAlbumsOnly_ = (bool)checkboxNewAlbumsOnly.IsChecked;

            bool hasBand = !String.IsNullOrEmpty(textboxBand.Text);
            if (hasBand)
            {
                string bandPrefixOrUrl = textboxBand.Text.Trim();

                // handle URLs
                if (bandPrefixOrUrl.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    bandPrefixOrUrl = "https://" + bandPrefixOrUrl;
                if (!Tools.IsStringURL(bandPrefixOrUrl))
                {
                    bandPrefixOrUrl = Tools.ToTitleCase(bandPrefixOrUrl);
                    labelStatus.Content = "Searching bands '" + bandPrefixOrUrl + "...'";
                }
                else
                    labelStatus.Content = "Searching URL " + bandPrefixOrUrl;

                labelStatus.Visibility = Visibility.Visible;

                threadProcess_ = new Thread(() => processByBandPrefixOrUrl(bandPrefixOrUrl));
            }
            else
            {
                // search by genre and filter
                GenreURL item = comboboxGenres.SelectedItem as GenreURL;

                // get search parameters
                string genre = item.Genre;
                string genreURL = item.Url;
                string filter = comboboxFilters.SelectedItem as string;
                Tools.LogEvent("SEARCHING " + genre + " BANDS " + "'" + filter + "'...");

                labelStatus.Visibility = Visibility.Visible;
                labelStatus.Content = "Searching bands...";

                threadProcess_ = new Thread(() => processByGenreAndFilter(genreURL, filter));
            }

            // start process thread
            nbNewAlbumsProcessed_ = 0;
            isProcessing_ = true;
            updateGUI();
            threadProcess_.Start();

            while (isProcessing_)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            // update status label
            string statusText = downloadNewAlbumsOnly_ ?
                nbNewAlbumsProcessed_.ToString() + " new album(s) processed" :
                nbNewAlbumsProcessed_.ToString() + " album(s) processed";
            labelStatus.Content = statusText;
            updateGUI();
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
        {
            threadProcess_?.Abort();

            Tools.AbortWebRequest();

            this.Close();
        }

        private void textboxBand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                buttonGo_Click(sender, e);
        }

        private void textboxBand_TextChanged(object sender, TextChangedEventArgs e)
        {
            // update tooltip
            string tooltip = String.IsNullOrEmpty(textboxBand.Text) ?
                tooltipTextboxBandStr : textboxBand.Text;
            textboxBand.ToolTip = tooltip;

            updateGUI();
        }

        private void textboxBand_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            textboxBand.SelectAll();
        }

        #endregion

        // process by genre + filter
        private void processByGenreAndFilter(string genreURL, string filter)
        {
            try
            {
                // get genre bands
                string urlSite = "https://www.metalmusicarchives.com";
                string urlGenre = urlSite + genreURL;
                string sourceHTML = Tools.GetWebPageSourceHTML(urlGenre);
                MMAParseGenreBands parsedGenreBands = new MMAParseGenreBands(sourceHTML);

                // get number of bands
                int nbBands = 0; // default
                if (parsedGenreBands != null)
                    nbBands = parsedGenreBands.GenreBands.Count;

                int indexBand = 0;
                foreach (string band in parsedGenreBands.GenreBands)
                {
                    if (isBandFiltered(filter, band))
                    {
                        indexBand++;
                        continue;
                    }

                    // process band albums
                    processBandFrontPage(band, parsedGenreBands.GenreBandsURLs[indexBand]);
                    indexBand++;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.LogError(e.Message);
            }

            isProcessing_ = false;
        }

        private bool isBandFiltered(string filter, string band)
        {
            // apply search filter
            if (filter == "ALL")
            {
                return false;
            }
            if (filter == "0-9")
            {
                char c = band[0];
                if (Char.IsLetter(c))
                    return true;
            }
            else   // letter
            {
                string bandName = band;
                if (band.StartsWith("The "))
                    bandName = band.Replace("The ", "");

                if (!bandName.StartsWith(filter))
                    return true;
            }

            return false;
        }

        // process by band name prefix or URL
        private void processByBandPrefixOrUrl(string bandPrefixOrUrl)
        {
            bool isBandExisting = false;

            // handle URLs
            if (Tools.IsStringURL(bandPrefixOrUrl))
            {
                processBandFrontPage(bandPrefixOrUrl);
                isProcessing_ = false;
                return;
            }

            // handle prefix
            try
            {
                // search band with specified prefix in all genres pages
                foreach (GenreURL item in comboboxGenres.Items)
                {
                    string genre = item.Genre;

                    // skip sub-genres
                    if (genre.StartsWith(" "))
                        continue;

                    string genreURL = item.Url;
                    string urlSite = "https://www.metalmusicarchives.com";
                    string urlGenre = urlSite + genreURL;
                    string sourceHTML = Tools.GetWebPageSourceHTML(urlGenre);
                    MMAParseGenreBands parsedGenreBands = new MMAParseGenreBands(sourceHTML);

                    int indexExistingBand = 0;
                    foreach (string existingBand in parsedGenreBands.GenreBands)
                    {
                        if (existingBand.Equals(bandPrefixOrUrl, StringComparison.OrdinalIgnoreCase))
                            isBandExisting = true;

                        if (existingBand.StartsWith(bandPrefixOrUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            // ok, found in genre, process band
                            string existingBandURLSuffix = parsedGenreBands.GenreBandsURLs[indexExistingBand];
                            processBandFrontPage(existingBand, existingBandURLSuffix);
                            sendSearchBandsParams(bandPrefixOrUrl);
                        }

                        indexExistingBand++;
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.LogError(e.Message);
            }

            // if band not existing, search band
            if (!isBandExisting)
                processBandFrontPage(bandPrefixOrUrl);

            isProcessing_ = false;
        }

        private void processBandFrontPage(string bandNameOrUrl, string existingBandURLSuffix = "")
        {
            bool isURL = Tools.IsStringURL(bandNameOrUrl);

            string urlBand;
            if (isURL)
            {
                // search by band URL
                Tools.LogEvent(" ");
                Tools.LogEvent("SEARCHING URL " + bandNameOrUrl + "...");

                urlBand = bandNameOrUrl;

                // get band name from URL
                string sourceHTMLBandPage = Tools.GetWebPageSourceHTML(urlBand);
                if (String.IsNullOrEmpty(sourceHTMLBandPage))
                {
                    Tools.LogEvent("   Page not found for " + bandNameOrUrl);
                    return;
                }
                EMParseBandHomonymsPage page = new EMParseBandHomonymsPage(sourceHTMLBandPage);
                string band = page.Band;
                if (String.IsNullOrEmpty(band)) // no homonyms
                    band = page.GetBandNameFromNonHomonymPage();

                if (String.IsNullOrEmpty(band))
                {
                    Tools.LogEvent("   Band name not found for " + bandNameOrUrl);
                    return;
                }

                sendProcessBandParams(band);

                // search if band already exists
                // if yes, get existing band albums
                List<string> existingAlbumsNamesYears = new List<string>();
                if (downloadNewAlbumsOnly_)
                {
                    List<string> processedBandURLs = new List<string>();
                    foreach (GenreURL item in comboboxGenres.Items)
                    {
                        string genre = item.Genre;

                        // skip sub-genres
                        if (genre.StartsWith(" "))
                            continue;

                        string genreURL = item.Url;
                        string urlSite = "https://www.metalmusicarchives.com";
                        string urlGenre = urlSite + genreURL;
                        string sourceHTML = Tools.GetWebPageSourceHTML(urlGenre);
                        MMAParseGenreBands parsedGenreBands = new MMAParseGenreBands(sourceHTML);

                        int indexExistingBand = 0;
                        foreach (string existingBand in parsedGenreBands.GenreBands)
                        {
                            if (existingBand.Equals(band, StringComparison.OrdinalIgnoreCase))
                            {
                                // ok, found in genre, check if already processed
                                string bandURLSuffix = parsedGenreBands.GenreBandsURLs[indexExistingBand];

                                // check if band already processed
                                bool processed = false;
                                foreach (string processedBandURL in processedBandURLs)
                                {
                                    if (String.Compare(bandURLSuffix, processedBandURL, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        processed = true;
                                        break;
                                    }
                                }

                                // skip if already processed
                                if (processed) continue;

                                // get existing band albums
                                processedBandURLs.Add(bandURLSuffix);
                                existingAlbumsNamesYears.AddRange(getExistingAlbums(band, bandURLSuffix));
                            }

                            indexExistingBand++;
                        }
                    }
                }

                // process band homonyms
                processBandHomonymsPage(page, band, urlBand, existingAlbumsNamesYears);
            }
            else
            {
                // search by band prefix
                sendProcessBandParams(bandNameOrUrl);

                string logText = String.IsNullOrEmpty(existingBandURLSuffix) ?
        "BAND " + bandNameOrUrl + " NOT EXISTING" : " ";
                Tools.LogEvent(logText);
                Tools.LogEvent("PROCESSING BAND " + bandNameOrUrl + "...");

                // build URL suffix
                string bandSuffix = bandNameOrUrl.Replace(' ', '_');
                urlBand = "https://www.metal-archives.com/bands/" + bandSuffix;

                logText = downloadNewAlbumsOnly_ ?
                    "   Searching new albums of " + bandNameOrUrl + "..." :
                    "   Searching all albums of " + bandNameOrUrl + "...";
                Tools.LogEvent(logText);

                // parse existing albums
                List<string> existingAlbumsNamesYears = getExistingAlbums(bandNameOrUrl, existingBandURLSuffix);

                // search for band homonyms
                string sourceHTMLBandPage = Tools.GetWebPageSourceHTML(urlBand);
                if (String.IsNullOrEmpty(sourceHTMLBandPage))
                {
                    Tools.LogEvent("   Page not found for " + bandNameOrUrl);
                    return;
                }

                // process band homonyms
                EMParseBandHomonymsPage page = new EMParseBandHomonymsPage(sourceHTMLBandPage);
                string band = String.IsNullOrEmpty(page.Band) ? bandNameOrUrl : page.Band;
                processBandHomonymsPage(page, band, urlBand, existingAlbumsNamesYears);
            }
        }

        private void processBandHomonymsPage(EMParseBandHomonymsPage page, string band, string urlBand, List<string> existingAlbumsNamesYears)
        {
            int nbBandHomonyms = page.HomonymsURLs.Count;
            if (nbBandHomonyms == 0) // no homonyms
                processBandPage(band, urlBand, existingAlbumsNamesYears);
            else
            {
                // process all bands homonyms
                Tools.LogEvent(nbBandHomonyms + " HOMONYMS FOUND FOR BAND " + band);
                foreach (string homonymURL in page.HomonymsURLs)
                    processBandPage(band, homonymURL, existingAlbumsNamesYears);
            }
        }

        private List<string> getExistingAlbums(string band, string existingBandURLSuffix = "")
        {
            // parse existing albums
            List<string> existingAlbumsNamesYears = new List<string>();
            if (downloadNewAlbumsOnly_ && !String.IsNullOrEmpty(existingBandURLSuffix))
            {
                string existingBandURL = "https://www.metalmusicarchives.com" + existingBandURLSuffix;
                string sourceHTMLExistingBandPage = Tools.GetWebPageSourceHTML(existingBandURL);
                MMAParseExistingBandAlbums existingBandAlbums = new MMAParseExistingBandAlbums(sourceHTMLExistingBandPage);
                existingAlbumsNamesYears = existingBandAlbums.AlbumsNamesYears;

                int nbExistingAlbums = existingAlbumsNamesYears.Count;
                Tools.LogEvent("   " + nbExistingAlbums.ToString() + " existing album(s) found for " + band);
            }

            return existingAlbumsNamesYears;
        }

        private void processBandPage(string band, string urlBand, List<string> existingAlbumsNamesYears)
        {
            // parse band page albums
            string sourceHTMLBandPage = Tools.GetWebPageSourceHTML(urlBand);
            if (String.IsNullOrEmpty(sourceHTMLBandPage))
            {
                Tools.LogEvent("   Page not found for " + band);
                return;
            }
            EMParseBandPage bandPage = new EMParseBandPage(sourceHTMLBandPage);
            if (bandPage.AlbumsURLs == null)
            {
                // parse error or several bands with the same name, skip
                Tools.LogEvent("   Check page for " + band + ". May contain several bands with the same name.");
                return;
            }

            string country = bandPage.Country;

            int nbFoundAlbums = bandPage.AlbumsURLs.Count;
            string logText = downloadNewAlbumsOnly_ ?
                "   " + nbFoundAlbums + " new album(s) found for " + band + " (" + country + ")" :
                "   " + nbFoundAlbums + " album(s) found for " + band + " (" + country + ")";
            Tools.LogEvent(logText);

            // get number of albums
            int nbAlbums = 0; // default
            if (bandPage != null)
                nbAlbums = bandPage.AlbumsURLs.Count;

            // parse band albums
            int indexAlbum = 0;
            foreach (string albumURL in bandPage.AlbumsURLs)
            {
                sendProcessAlbumParams(band, indexAlbum, nbAlbums);

                string albumName = bandPage.AlbumsNames[indexAlbum];
                string albumYear = bandPage.AlbumsYears[indexAlbum];
                string albumType = bandPage.AlbumsTypes[indexAlbum];

                // check if album already exists
                if (downloadNewAlbumsOnly_ && existingAlbumsNamesYears.Count > 0)
                {
                    bool exists = false;
                    string albumNameYear = albumName + "_" + albumYear;
                    foreach (string existingAlbumNameYear in existingAlbumsNamesYears)
                    {
                        if (String.Compare(albumNameYear, existingAlbumNameYear, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            exists = true;
                            break;
                        }
                    }

                    // if album already exists, skip
                    if (exists)
                    {
                        Tools.LogEvent("      Album \'" + band + " (" + country + ")" + " - " + albumName + "\' (" + albumYear + ")" + " already exists");
                        indexAlbum++;
                        continue;
                    }
                }

                // check if album data is already downloaded
                {
                    string downloadFilePath = Tools.DownloadFilePath(band, albumName, albumYear, albumType);
                    bool infoDownloaded = File.Exists(System.IO.Path.Combine(downloadFilePath + ".txt"));
                    bool coverDownloaded = File.Exists(System.IO.Path.Combine(downloadFilePath + ".jpg"));

                    // if album data already downloaded, skip
                    if (infoDownloaded && coverDownloaded)
                    {
                        Tools.LogEvent("      Album \'" + band + " (" + country + ")" + " - " + albumName + "\' (" + albumYear + ")" + " already downloaded");
                        indexAlbum++;
                        continue;
                    }
                }

                // parse album page
                string sourceHTMLAlbumPage = Tools.GetWebPageSourceHTML(albumURL);
                Tools.LogEvent("      Processing album \'" + band + " (" + country + ")" + " - " + albumName + "\' (" + albumYear + ")");
                EMParseAlbumPage page = new EMParseAlbumPage(sourceHTMLAlbumPage, country, bandPage.Genre, albumURL);

                // for debug purposes only
                /*
                //string testAlbumURL = "https://www.metal-archives.com/albums/Killswitch_Engage/Incarnate/558828";
                //string testAlbumURL = "https://www.metal-archives.com/albums/Killswitch_Engage/Incarnate/568298";
                //string testAlbumURL = "https://www.metal-archives.com/albums/Killswitch_Engage/Killswitch_Engage/237410";
                string testAlbumURL = "https://www.metal-archives.com/albums/Ad_Baculum/Blackness_Doctrine/317809";
                string sourceHTMLAlbumPage = Tools.GetWebPageSourceHTML(testAlbumURL);
                EMParseAlbumPage page = new EMParseAlbumPage(sourceHTMLAlbumPage, country, bandPage.Genre, testAlbumURL);
                */

                MMAAlbumDataWriter writer = new MMAAlbumDataWriter(page);

                // handle alternate versions if existing
                int nbAltVersions = (page.AltVersionsURLs == null) ? 0 : page.AltVersionsURLs.Count;
                if (nbAltVersions > 0)
                {
                    Tools.LogEvent("         " + nbAltVersions.ToString() + " alternate version(s) found for \'" + band + " (" + country + ")" + " - " + albumName + "\' (" + albumYear + ")");

                    foreach (string url in page.AltVersionsURLs)
                    {
                        string sourceHTMLAlbumPageAlt = Tools.GetWebPageSourceHTML(url);
                        EMParseAlbumPage pageAlt = new EMParseAlbumPage(sourceHTMLAlbumPageAlt, country, bandPage.Genre, url);

                        // add alternate version data
                        writer.AddAlternateVersion(pageAlt.Version, pageAlt.Year, pageAlt.ReleaseDate, pageAlt.Format, pageAlt.Label, pageAlt.CatalogId, pageAlt.Limitation, pageAlt.Info, pageAlt.Songs, pageAlt.Durations, pageAlt.DiscsTotalTimes);
                    }
                }

                // write album data
                writer.WriteAlbumData();

                nbNewAlbumsProcessed_++;
                indexAlbum++;
            }
        }

        private void updateGUI()
        {
            buttonGo.Content = isProcessing_ ? "Stop" : "GO!";
            bool hasBand = !String.IsNullOrEmpty(textboxBand.Text);

            labelGenre.IsEnabled = !hasBand; //&& !isProcessing_;
            labelFilter.IsEnabled = !hasBand; //&& !isProcessing_;
            comboboxGenres.IsEnabled = !hasBand && !isProcessing_;
            comboboxFilters.IsEnabled = !hasBand && !isProcessing_;

            labelBand.IsEnabled = !isProcessing_ || hasBand;
            textboxBand.IsEnabled = !isProcessing_;
            checkboxNewAlbumsOnly.IsEnabled = !isProcessing_;

            progressbar.Visibility = isProcessing_ ? Visibility.Visible : Visibility.Hidden;

            buttonQuit.IsEnabled = !isProcessing_;
        }

        private void sendSearchBandsParams(string bandPrefix)
        {
            OnSearchBandsParams?.Invoke(bandPrefix, null);
        }

        private void sendProcessBandParams(string band)
        {
            OnProcessBandParams?.Invoke(band, null);
        }

        private void sendProcessAlbumParams(string band, int index, int nbAlbums)
        {
            Tuple<string, int, int> values = Tuple.Create(band, index, nbAlbums);
            OnProcessAlbumParams?.Invoke(values, null);
        }

        void Form1_OnSearchBandsParams(object sender, EventArgs e)
        {
            string bandPrefix = sender as string;
            Dispatcher.Invoke(new changeProgressLabel(changeProgressLabelText), "Searching bands '" + bandPrefix + "...'");
        }

        private void Form1_OnProcessBandParams(object sender, EventArgs e)
        {
            string band = sender as string;
            Dispatcher.Invoke(new changeProgressLabel(changeProgressLabelText), "Processing " + band + "...");
        }

        void Form1_OnProcessAlbumParams(object sender, EventArgs e)
        {
            Tuple<string, int, int> values = (Tuple<string, int, int>)sender;
            string band = values.Item1;
            int index = values.Item2;
            int nbAlbums = values.Item3;

            Dispatcher.Invoke(new changeProgressLabel(changeProgressLabelText), "Processing " + band + " (album " + (index + 1).ToString() + "/" + nbAlbums + ")...");
            //Dispatcher.Invoke(new incrementProgressBar(incrementProgressBarValue), (float)(1.0 / (2 * nbDocs + 4)));
        }

        private void changeProgressLabelText(string text)
        {
            labelStatus.Content = text;
        }
    }
}
