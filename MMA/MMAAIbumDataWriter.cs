using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MMACollaboratorHelper
{
    public class MMAAlbumDataWriter
    {
        // band info
        readonly string country_;
        readonly string genre_;

        // album info
        readonly string bandName_;
        readonly string title_;
        readonly string type_;
        readonly string year_;
        readonly string releaseDate_;
        readonly string format_;
        readonly string label_;
        readonly string catalogId_;
        readonly string limitation_; // can be empty
        readonly string info_;
        readonly List<List<string>> songs_;
        readonly List<List<string>> durations_;
        readonly List<string> discsTotalTimes_;
        readonly List<string> lineup_;
        readonly string coverURL_;
        readonly string albumURL_;

        // alternate versions
        readonly List<AlternateVersion> altVersions_;

        public MMAAlbumDataWriter(EMParseAlbumPage parsedAlbumPage)
        {
            // get band data
            country_ = parsedAlbumPage.Country;
            genre_ = parsedAlbumPage.Genre;

            // get album data
            bandName_ = parsedAlbumPage.Band;
            title_ = parsedAlbumPage.Title;
            type_ = parsedAlbumPage.Type;
            year_ = parsedAlbumPage.Year;
            releaseDate_ = parsedAlbumPage.ReleaseDate;
            format_ = parsedAlbumPage.Format;
            label_ = parsedAlbumPage.Label;
            catalogId_ = parsedAlbumPage.CatalogId;
            limitation_ = parsedAlbumPage.Limitation;
            info_ = parsedAlbumPage.Info;
            songs_ = parsedAlbumPage.Songs;
            durations_ = parsedAlbumPage.Durations;
            discsTotalTimes_ = parsedAlbumPage.DiscsTotalTimes;
            lineup_ = parsedAlbumPage.Lineup;
            coverURL_ = parsedAlbumPage.CoverURL;
            albumURL_ = parsedAlbumPage.AlbumURL;

            altVersions_ = new List<AlternateVersion>();

            if (String.IsNullOrEmpty(title_))
            {
                Tools.LogEvent("      Cannot get album title, skipping...");
                return;
            }
             

            if (String.IsNullOrEmpty(coverURL_))
                Tools.LogEvent("      Cannot get cover image of album \'" + bandName_ + " (" + country_ + ")" + " - " + title_ + "\' (" + year_ + ")");
        }

        // add alternate version data
        public void AddAlternateVersion(string version, string year, string releaseDate, string format, string label, string catalogID, string limitation, string info, List<List<string>> songs, List<List<string>> durations, List<string> discsTotalTimes)
        {
            AlternateVersion altVersion = new AlternateVersion(version, year, releaseDate, format, label, catalogID, limitation, info, songs, durations, discsTotalTimes);
            altVersions_.Add(altVersion);
        }

        #region Write functions

        // write album infos (data + cover image)
        public void WriteAlbumData()
        {
            List<string> bandList = new List<string>
            {
                bandName_ // default
            };

            // detect if split album
            if (String.Compare(type_, "split", StringComparison.OrdinalIgnoreCase) == 0
             || String.Compare(type_, "collaboration", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string[] bands = bandName_.Split('/');
                if (bands.Count() > 1)
                {
                    // split detected
                    bandList.Clear();
                    foreach (string band in bands)
                    {
                        string bandTrimmed = band.Trim();
                        bandList.Add(bandTrimmed);
                    }
                }
            }

            foreach (string band in bandList)
            {
                // create file names
                string downloadFilePath = Tools.DownloadFilePath(band, title_, year_, type_);

                string filePath = downloadFilePath + ".txt";
                writeAlbumContent(filePath);

                string imgPath = downloadFilePath + ".jpg";
                writeCoverImage(imgPath);
            }
        }

        private void writeAlbumContent(string pathFile)
        {
            // create info text

            string RC = "\r\n";  // return carriage
            string NL = RC + RC; // new blank line

            // header
            string text = "Band:\t\t" + bandName_ + RC;
            text += "Country:\t" + country_ + NL;
            text += "Album:\t\t" + title_ + NL;
            text += "Genre:\t\t" + genre_ + NL;
            text += "Release date:\t" + releaseDate_ + RC;
            text += "Release year:\t" + year_ + NL;
            text += "Format:\t\t" + format_ + NL;
            text += "Type:\t\t" + type_ + NL;
            text += "Label:\t\t" + label_ + RC;
            text += "Catalog ID:\t" + catalogId_ + NL;
            text += RC;

            // info
            text += getInfoText(info_, limitation_);

            // main track-list
            text += getTrackListText("", year_, "", "", "", "", songs_, durations_, discsTotalTimes_, true);
            text += RC;

            // write alternate versions data if existing
            foreach (AlternateVersion altVersion in altVersions_)
            {
                text += getTrackListText(altVersion.Version, altVersion.Year, altVersion.ReleaseDate, altVersion.Format, altVersion.Label, altVersion.CatalogID, altVersion.Songs, altVersion.Durations, altVersion.DiscsTotalTimes);
                text += getInfoText(altVersion.Info, altVersion.Limitation);
            }

            // line-up
            text += "Line-up:" + NL;
            if (lineup_ != null)
                foreach (string member in lineup_)
                    text += member + RC;

            // source URL if existing
            if (!String.IsNullOrEmpty(albumURL_))
            {
                text += RC + NL + "Source:" + NL;
                text += albumURL_;
            }

            // write into file
            File.WriteAllText(pathFile, text);
        }

        private string getInfoText(string info, string limitation)
        {
            string text = "";

            string RC = "\r\n";  // return carriage
            string NL = RC + RC; // new blank line

            text += "Info:" + NL;
            text += info;
            if (!String.IsNullOrEmpty(limitation))
                text += NL + "Limited to " + limitation;
            text += NL + RC;

            return text;
        }

        private string getTrackListText(string version, string year, string releaseDate, string format, string label, string catalogID, List<List<string>> songs, List<List<string>> durations, List<string> discsTotalTimes, bool isParent = false)
        {
            string text = "";

            string RC = "\r\n";  // return carriage
            string NL = RC + RC; // new blank line

            // write version
            if (isParent) // parent version
                version = "Track-list";
            else
            {
                if (String.IsNullOrEmpty(version)) // unknown
                    version = "(Unknown alternate version)";
                else
                {
                    // specific rearrangements
                    version = Tools.ToTitleCase(version);

                    if (!version.Contains("Edition"))
                        version += " Edition";

                    version = version.Replace(" Cd ", " CD ");

                    if (version.Equals("Japan Edition", StringComparison.OrdinalIgnoreCase))
                        version = "Japanese Edition";
                }

                // write year if existing
                if (!String.IsNullOrEmpty(year))
                    version = year + " " + version;
            }

            text += version + ":" + RC;

            // if alternate version, write release date, format, label and catalog id
            if (!isParent)
            {
                text += RC;
                if (!String.IsNullOrEmpty(releaseDate))
                    text += "Release date: " + releaseDate + RC;
                if (!String.IsNullOrEmpty(format))
                    text += "Format: " + format + RC;
                if (!String.IsNullOrEmpty(label))
                    text += "Label: " + label + RC;
                if (!String.IsNullOrEmpty(catalogID))
                    text += "Catalog ID: " + catalogID + RC;
            }

            text += RC;

            // write discs and songs
            int nbDiscs = (songs == null) ? 0 : songs.Count;
            for (int discIndex = 1; discIndex <= nbDiscs; discIndex++)
            {
                List<string> discSongs = songs.ElementAt(discIndex - 1);
                List<string> discDurations = durations.ElementAt(discIndex - 1);
                string discTotalTime = discsTotalTimes.ElementAt(discIndex - 1);

                int songIndex = 1; // offset
                if (nbDiscs > 1)
                    text += "Disc " + discIndex.ToString() + RC;

                int nbSongs = discSongs.Count;
                foreach (string song in discSongs)
                {
                    // build song index string
                    string indexString = songIndex.ToString();
                    if (nbSongs >= 10 && songIndex < 10)
                        indexString = "0" + indexString;

                    string songDesc = indexString + ". " + song;

                    string songDuration = "";
                    string duration = discDurations.ElementAt(songIndex - 1);
                    if (!String.IsNullOrEmpty(duration))
                        songDuration = "(" + duration + ")";

                    text += songDesc;
                    if (!String.IsNullOrEmpty(songDuration))
                        text += " " + songDuration;
                    text += RC;
                    songIndex++;
                }

                // total disc time
                text += RC;
                text += "Total Time " + discTotalTime + NL;
                //text += RC;
            }

            return text;
        }

        private void writeCoverImage(string imgPath)
        {
            if (String.IsNullOrEmpty(coverURL_))
                return;

            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(coverURL_, imgPath);
            }
            catch (Exception /*e*/)
            {
                //
            }
        }
        
        #endregion
    }

    // used for alternate versions
    class AlternateVersion
    {
        readonly string version;
        readonly string year;
        readonly string releaseDate;
        readonly string format;
        readonly string label;
        readonly string catalogID;
        readonly string limitation;
        readonly string info;
        readonly List<List<string>> songs;
        readonly List<List<string>> durations;
        readonly List<string> discsTotalTimes;

        public string Version
        {
            get { return version; }
        }

        public string Year
        {
            get { return year; }
        }

        public string ReleaseDate
        {
            get { return releaseDate; }
        }

        public string Format
        {
            get { return format; }
        }

        public string Label
        {
            get { return label; }
        }

        public string CatalogID
        {
            get { return catalogID; }
        }

        public string Limitation
        {
            get { return limitation; }
        }

        public string Info
        {
            get { return info; }
        }

        public List<List<string>> Songs
        {
            get { return songs; }
        }

        public List<List<string>> Durations
        {
            get { return durations; }
        }

        public List<string> DiscsTotalTimes
        {
            get { return discsTotalTimes; }
        }

        public AlternateVersion(string version, string year, string releaseDate, string format, string label, string catalogID, string limitation, string info, List<List<string>> songs, List<List<string>> durations, List<string> discsTotalTimes)
        {
            this.version = version;
            this.year = year;
            this.releaseDate = releaseDate;
            this.format = format;
            this.label = label;
            this.catalogID = catalogID;
            this.limitation = limitation;
            this.info = info;
            this.songs = songs;
            this.durations = durations;
            this.discsTotalTimes = discsTotalTimes;
        }
    }
}
