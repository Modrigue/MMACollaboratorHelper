using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMACollaboratorHelper
{
    public class EMParseAlbumPage
    {
        // band info
        readonly string country_;
        readonly string genre_;

        // album info
        readonly string bandName_;
        readonly string title_;
        readonly string version_; // can be empty
        readonly string type_;
        readonly string year_;
        string releaseDate_;
        readonly string format_;
        readonly string label_;
        readonly string catalogId_;
        readonly string limitation_; // can be empty
        readonly string info_;
        readonly List<List<string>> songs_; // can be empty
        readonly List<List<string>> durations_;
        readonly List<string> discsTotalTimes_;
        readonly List<string> lineup_; // can be empty
        readonly string coverURL_;
        readonly string albumURL_;
        readonly List<string> altVersionsURLs_;

        #region Accessors

        public string Band
        {
            get { return bandName_; }
        }

        public string Country
        {
            get { return country_; }
        }

        public string Genre
        {
            get { return genre_; }
        }

        public string Title
        {
            get { return title_; }
        }

        public string Version
        {
            get { return version_; }
        }

        public string Type
        {
            get { return type_; }
        }

        public string Year
        {
            get { return year_; }
        }

        public string ReleaseDate
        {
            get { return releaseDate_; }
        }

        public string Format
        {
            get { return format_; }
        }

        public string Label
        {
            get { return label_; }
        }

        public string CatalogId
        {
            get { return catalogId_; }
        }

        public string Limitation
        {
            get { return limitation_; }
        }

        public string Info
        {
            get { return info_; }
        }

        public List<List<string>> Songs
        {
            get { return songs_; }
        }

        public List<List<string>> Durations
        {
            get { return durations_; }
        }

        public List<string> DiscsTotalTimes
        {
            get { return discsTotalTimes_; }
        }

        public List<string> Lineup
        {
            get { return lineup_; }
        }

        public string CoverURL
        {
            get { return coverURL_; }
        }

        public string AlbumURL
        {
            get { return albumURL_; }
        }

        public List<string> AltVersionsURLs
        {
            get { return altVersionsURLs_; }
        }

        #endregion

        // parse objects
        readonly HtmlAgilityPack.HtmlDocument htmlDoc_;
        HtmlNode nodeAlbumContent_;
        HtmlNode nodeAlbumInfo_;

        public EMParseAlbumPage(string sourceHTML, string country, string genre, string albumURL = "")
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute album nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            getAlbumNodes();

            // get band data
            country_ = country;
            genre_ = genre;

            // get album data
            bandName_ = getBandName();
            title_ = getAlbumTitle();
            type_ = getAlbumType();
            year_ = getAlbumReleaseYear();
            format_ = getAlbumFormat();
            label_ = getAlbumLabel();
            catalogId_ = getAlbumCatalogId();
            limitation_ = getAlbumLimitation(); // can be empty
            version_ = getAlbumVersion(); // can be empty
            info_ = getAlbumInfo();
            songs_ = getAlbumSongsNames();
            durations_ = getAlbumSongsDurations();
            discsTotalTimes_ = getAlbumDiscsTotalTimes();
            lineup_ = getAlbumLineup();
            coverURL_ = getAlbumCoverURL();
            albumURL_ = albumURL;

            // get album alternate versions if existing
            altVersionsURLs_ = getAltVersionsURLs();
        }

        #region Parse info functions

        private void getAlbumNodes()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(htmlDoc_.DocumentNode, "div", "id", "wrapper");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "content_wrapper");

            nodeAlbumContent_ = Tools.NodeWithAttributeAndValue(node2, "div", "id", "album_content");
            nodeAlbumInfo_ = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "div", "id", "album_info");
        }

        private string getBandName()
        {
            HtmlNode nodeBandName = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "h2", "class", "band_name");
            if (nodeBandName == null) return null;

            string bandName = Tools.CleanString(nodeBandName.InnerText);
            return bandName;
        }

        private string getAlbumTitle()
        {
            HtmlNode nodeAlbumName = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "h1", "class", "album_name") ?? Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "h1", "class", "album_name noCaps");
            if (nodeAlbumName == null) return null;

            string albumTitle = Tools.CleanString(nodeAlbumName.InnerText);
            return albumTitle;
        }

        private string getAlbumType()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_left");
            if (node5 == null) return null;

            HtmlNode node6 = node5.Descendants("dd").FirstOrDefault();
            if (node6 == null) return null;

            string albumType = Tools.CleanString(node6.InnerText);

            // specific cases
            if (String.Compare(albumType, "Full-length", StringComparison.OrdinalIgnoreCase) == 0)
                albumType = "Studio";
            if (String.Compare(albumType, "Live album", StringComparison.OrdinalIgnoreCase) == 0)
                albumType = "Live";

            return albumType;
        }

        private string getAlbumReleaseYear()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_left");
            if (node5 == null) return null;

            HtmlNode node6 = node5.Descendants("dd").ElementAt(1);
            if (node6 == null) return null;

            string releaseDate = node6.InnerText;
            releaseDate_ = releaseDate;

            // remove month and day
            int indexYear = releaseDate.IndexOf(',');
            string albumYear = releaseDate.Substring(indexYear + 1);
            indexYear = albumYear.IndexOf(' ');
            albumYear = albumYear.Substring(indexYear + 1);

            albumYear = Tools.CleanString(albumYear);
            return albumYear;
        }

        private string getAlbumFormat()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_right");
            if (node5 == null) return "";

            // search format entry if existing
            int indexFormat = -1;
            int index = 0;
            foreach (HtmlNode node in node5.Descendants("dt"))
            {
                if (node.InnerText.Equals("Format:", StringComparison.OrdinalIgnoreCase))
                {
                    // found
                    indexFormat = index;
                    break;
                }

                index++;
            }

            if (indexFormat < 0) return "";

            if (node5.Descendants("dd").Count() <= indexFormat) return "";
            HtmlNode node6 = node5.Descendants("dd").ElementAt(indexFormat);
            if (node6 == null) return "";

            string format = Tools.CleanString(node6.InnerText);
            return format;
        }

        private string getAlbumCatalogId()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_left");
            if (node5 == null) return "N/A";

            if (node5.Descendants("dd").Count() < 3) return "N/A";
            HtmlNode node6 = node5.Descendants("dd").ElementAt(2);
            if (node6 == null) return "N/A";

            string catalogId = Tools.CleanString(node6.InnerText);
            return catalogId;
        }

        private string getAlbumLimitation()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_right");
            if (node5 == null) return "";

            // search limitation entry if existing
            int indexLimitation = -1;
            int index = 0;
            foreach (HtmlNode node in node5.Descendants("dt"))
            {
                if (node.InnerText.Equals("Limitation:", StringComparison.OrdinalIgnoreCase))
                {
                    // found
                    indexLimitation = index;
                    break;
                }

                index++;
            }

            if (indexLimitation < 0) return "";

            if (node5.Descendants("dd").Count() <= indexLimitation) return "";
            HtmlNode node6 = node5.Descendants("dd").ElementAt(indexLimitation);
            if (node6 == null) return "";

            string limitation = Tools.CleanString(node6.InnerText);
            return limitation;
        }

        private string getAlbumVersion()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_left");
            if (node5 == null) return "";

            if (node5.Descendants("dd").Count() < 4) return "";
            HtmlNode node6 = node5.Descendants("dd").ElementAt(3);
            if (node6 == null) return "";

            string version = Tools.CleanString(node6.InnerText);
            return version;
        }

        private string getAlbumLabel()
        {
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(nodeAlbumInfo_, "dl", "class", "float_right");
            if (node5 == null) return "N/A";

            HtmlNode node6 = node5.Descendants("dd").FirstOrDefault();
            if (node6 == null) return "N/A";

            string label = Tools.CleanString(node6.InnerText);
            return label;
        }

        private string getAlbumInfo()
        {
            string info = "(no info found)"; // default

            HtmlNode node4 = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "div", "id", "album_tabs_notes");
            if (node4 == null) return info;

            foreach (HtmlNode node in node4.Descendants("div"))
            {
                if (node.Attributes.Contains("class"))
                    if (node.Attributes["class"].Value.StartsWith("ui-tabs-panel-content"))
                    {
                        string infoText = Tools.CleanString(node.InnerText);
                        for (int i = 0; i < 10; i++ )
                            infoText = infoText.Replace("\n ", "\n");

                        infoText = infoText.Replace("\n", "\r\n");
                        info = infoText;
                        break;
                    }
            }

            return info;
        }

        private List<List<string>> getAlbumSongsNames()
        {
            List<List<string>> songsNames = new List<List<string>>();

            HtmlNode node4 = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "table", "class", "display table_lyrics");
            HtmlNode node5 = node4.Descendants("tbody").FirstOrDefault();

            // get number of discs
            List<HtmlNode> nodesDiscs = Tools.NodeListWithAttributeAndValue(node5, "tr", "class", "discRow");
            int nbDiscs = Math.Max(nodesDiscs.Count, 1);

            // get songs per disc
            for (int i = 0; i < nbDiscs; i++)
            {
                List<string> songsDiscNames = new List<string>();
                songsNames.Add(songsDiscNames);
            }

            int curDiscIndex = (nodesDiscs.Count > 0) ? -1 : 0;
            foreach (HtmlNode node in node5.Descendants())
            {
                // new disc found
                if (node.Name == "tr" && node.Attributes.Contains("class") && node.Attributes["class"].Value == "discRow")
                {
                    curDiscIndex++;
                }

                // new song found
                if (node.Name == "td" && node.Attributes.Contains("class") && node.Attributes["class"].Value == "wrapWords")
                {
                    string songName = Tools.CleanString(node.InnerText);
                    songName = songName.Replace("\n", "");
                    songsNames.ElementAt(curDiscIndex).Add(songName);
                }

                // new bonus song found
                if (node.Name == "td" && node.Attributes.Contains("class") && node.Attributes["class"].Value == "wrapWords bonus")
                {
                    string songName = Tools.CleanString(node.InnerText);
                    songName = songName.Replace("\n", "");
                    songName += " (Bonus)";
                    songsNames.ElementAt(curDiscIndex).Add(songName);
                }
            }

            //List<HtmlNode> nodesSongsNames = Tools.NodeListWithAttributeAndValue(node5, "td", "class", "wrapWords");
            //foreach (HtmlNode node in nodesSongsNames)
            //{
            //    string songName = Tools.CleanString(node.InnerText);
            //    songsNames.Add(songName);
            //}

            return songsNames;
        }

        // n + 1 durations per disc, last one is the total time
        private List<List<string>> getAlbumSongsDurations()
        {
            List<List<string>> songsDurations = new List<List<string>>();

            HtmlNode node4 = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "table", "class", "display table_lyrics");
            HtmlNode node5 = node4.Descendants("tbody").FirstOrDefault();

            // get number of discs
            List<HtmlNode> nodesDiscs = Tools.NodeListWithAttributeAndValue(node5, "tr", "class", "discRow");
            int nbDiscs = Math.Max(nodesDiscs.Count, 1);

            // get durations per disc
            for (int i = 0; i < nbDiscs; i++)
            {
                List<string> songsDiscDurations = new List<string>();
                songsDurations.Add(songsDiscDurations);
            }

            int curDiscIndex = (nodesDiscs.Count > 0) ? -1 : 0;
            foreach (HtmlNode node in node5.Descendants())
            {
                // new disc found
                if (Tools.NodeHasAttributeAndValue(node, "tr", "class", "discRow"))
                {
                    curDiscIndex++;
                }

                // new duration found
                if (Tools.NodeHasAttributeAndValue(node, "td", "align", "right"))
                {
                    string songDuration = Tools.CleanString(node.InnerText);

                    // remove first '0' if exiting
                    songDuration = Tools.TrimTime(songDuration);

                    songsDurations.ElementAt(curDiscIndex).Add(songDuration);
                }
            }

            /*
            List<HtmlNode> nodesSongsDurations = Tools.NodeListWithAttributeAndValue(node5, "td", "align", "right");
            foreach (HtmlNode node in nodesSongsDurations)
            {
                string songDuration = Tools.CleanString(node.InnerText);

                // remove first '0' if exiting
                if (songDuration.StartsWith("0"))
                    if (songDuration.Length == 5)
                    {
                        songDuration = songDuration.Substring(1);
                    }

                songsDurations.Add(songDuration);
            }
            */

            return songsDurations;
        }

        private List<string> getAlbumDiscsTotalTimes()
        {
            List<string> totalTimes = new List<string>();

            bool canComputeTotalTime = true; // default
            foreach (List<string> discDuration in durations_)
            {
                foreach (string songDuration in discDuration)
                {
                    if (String.IsNullOrEmpty(songDuration))
                    {
                        canComputeTotalTime = false;
                        break;
                    }
                }

                // disc total time not found
                if (!canComputeTotalTime)
                {
                    totalTimes.Add("?");
                    continue;
                }

                // ok, disc total time is last duration
                string totalTime = discDuration.Last();

                // remove first '0' if exiting
                totalTime = Tools.TrimTime(totalTime);

                totalTimes.Add(totalTime);
            }

            return totalTimes;
        }

        private List<string> getAlbumLineup()
        {
            List<string> lineup = new List<string>();

            int indexBand = 0; // for split and collaborations

            HtmlNode node4 = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "div", "id", "album_tabs_lineup");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "id", "album_members");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "id", "album_all_members_lineup") ?? Tools.NodeWithAttributeAndValue(node5, "div", "id", "album_members_lineup");
            HtmlNode node7 = Tools.NodeWithAttributeAndValue(node6, "div", "class", "ui-tabs-panel-content");
            HtmlNode node8 = Tools.NodeWithAttributeAndValue(node7, "table", "class", "display lineupTable");
            
            // no line-up found
            if (node8 == null)
            {
                lineup.Add("(no line-up found)");
                return lineup;
            }

            foreach (HtmlNode node in node8.Descendants("tr"))
            {
                if (node.Attributes.Contains("class"))
                    if (node.Attributes["class"].Value == "lineupHeaders")
                    {
                        HtmlNode nodeDesc = node.Descendants("td").FirstOrDefault();
                        if (nodeDesc.InnerText.Contains("Guest")
                         || nodeDesc.InnerText.Contains("Session"))
                        {
                            lineup.Add("");
                            string guestLine = "Guest/session musicians:";
                            lineup.Add(guestLine);
                        }
                    }

                // skip if miscellaneous staff
                if (node.Attributes.Contains("class"))
                    if (node.Attributes["class"].Value == "lineupHeaders")
                    {
                        HtmlNode nodeDesc = node.Descendants("td").FirstOrDefault();
                        if (nodeDesc.InnerText.Contains("Miscellaneous staff"))
                            break;
                    }

                // get band if split or collaboration album
                if (type_.Equals("Split", StringComparison.OrdinalIgnoreCase)
                 || type_.Equals("Collaboration", StringComparison.OrdinalIgnoreCase)
                )
                {		
                    if (node.Attributes.Count == 0)
                    {
                        HtmlNode nodeBand = Tools.NodeWithAttributeAndValue(node, "td", "colspan", "2");
                        if (nodeBand != null)
                        {
                            // write band name in line-up
                            string band = Tools.CleanString(nodeBand.InnerText);
                            if (indexBand > 0) lineup.Add("");
                            string bandLine = band + ":";
                            lineup.Add(bandLine);
                            indexBand++;
                        }
                    }
                }

                // get musician + instrument
                bool isLineupNode = false;
                if (node.Attributes.Contains("class"))
                    if (node.Attributes["class"].Value == "lineupRow")
                        isLineupNode = true;

                if (!isLineupNode) continue;

                // get musician
                HtmlNode nodeMusician1 = node.Descendants("td").FirstOrDefault();
                HtmlNode nodeMusician2 = nodeMusician1.Descendants("a").FirstOrDefault();
                string musician = Tools.CleanString(nodeMusician2.InnerText);

                // get instrument(s)
                HtmlNode nodeInstrument = node.Descendants("td").ElementAt(1);
                string instrument = Tools.CleanString(nodeInstrument.InnerText);
                //instrument = instrument.ToLower(); // lower case

                string lineupLine = "- " + musician + " / " + instrument;

                lineup.Add(lineupLine);
            }

            // no line-up found
            if (lineup.Count == 0)
            {
                lineup.Add("(no line-up found)");
                return lineup;
            }

            return lineup;
        }

        private string getAlbumCoverURL()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(htmlDoc_.DocumentNode, "div", "id", "wrapper");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "content_wrapper");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "album_sidebar");
            
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "album_img");
            if (node4 == null) return "";

            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "a", "class", "image");
            if (node5 == null) return "";

            HtmlNode node6 = node5.Descendants("img").FirstOrDefault();
            if (node6 == null) return "";

            string url = "";
            if (node6.Attributes.Contains("src"))
                url = node6.Attributes["src"].Value;

            return url;
        }

        private List<string> getAltVersionsURLs()
        {
            List<string> altVersionsUrls = new List<string>();

            HtmlNode node4 = Tools.NodeWithAttributeAndValue(nodeAlbumContent_, "div", "id", "album_tabs");
            if (node4 == null) return altVersionsUrls;
            
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "ul", "class", "ui-tabs-nav");
            if (node5 == null) return altVersionsUrls;

            // get other versions parent URL
            string urlParentOtherVersions = "";
            foreach (HtmlNode nodeLi in node5.Descendants("li"))
            {
                foreach (HtmlNode node in nodeLi.Descendants("a"))
                {
                    if (node.Attributes.Contains("href") && node.InnerText.Equals("Other versions", StringComparison.OrdinalIgnoreCase))
                    {
                        // found
                        urlParentOtherVersions = node.Attributes["href"].Value;
                        break;
                    }
                }

                if (!String.IsNullOrEmpty(urlParentOtherVersions))
                    break;
            }

            // check if album is parent version
            string albumId = albumURL_.Split('/').Last();
            string curId = urlParentOtherVersions.Split('/').Last();
            bool isParentVersion = albumId.Equals(curId, StringComparison.OrdinalIgnoreCase);
            if (!isParentVersion)
                return altVersionsUrls; // nop if non-parent version

            // get other versions page source code
            string sourceOtherVersionsHTML = Tools.GetWebPageSourceHTML(urlParentOtherVersions);
            if (String.IsNullOrEmpty(sourceOtherVersionsHTML)) return altVersionsUrls;
            HtmlAgilityPack.HtmlDocument htmlDocOtherVersions = new HtmlAgilityPack.HtmlDocument();
            htmlDocOtherVersions.LoadHtml(sourceOtherVersionsHTML);
            
            // get other versions URLs
            foreach (HtmlNode node in htmlDocOtherVersions.DocumentNode.Descendants("div"))
            {
                if (node.Attributes.Contains("class") && node.Attributes["class"].Value.StartsWith("ui-tabs-panel-content"))
                {
                    HtmlNode node6 = Tools.NodeWithAttributeAndValue(node, "table", "class", "display");
                    if (node6 == null) return altVersionsUrls;

                    HtmlNode node7 = node6.Descendants("tbody").FirstOrDefault();
                    if (node7 == null) return altVersionsUrls;

                    foreach (HtmlNode node8 in node7.Descendants("tr"))
                    {
                        foreach (HtmlNode node9 in node8.Descendants("td"))
                        {
                            foreach (HtmlNode node10 in node9.Descendants("a"))
                            {
                                if (node10.Attributes.Contains("href"))
                                {
                                    string url = node10.Attributes["href"].Value;

                                    //string albumId = albumURL_.Split('/').Last();
                                    string altId = url.Split('/').Last();

                                    // add if not main version id
                                    if (!albumId.Equals(altId, StringComparison.OrdinalIgnoreCase))
                                        altVersionsUrls.Add(url);

                                    // add if not main version URL
                                    //if (!url.Equals(albumURL_, StringComparison.OrdinalIgnoreCase))
                                    //    altVersionsUrls.Add(url);
                                }
                            }
                        }
                    }

                    break;
                }
            }


            return altVersionsUrls;
        }

        #endregion
    }
}
