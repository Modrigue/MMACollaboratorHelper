using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMACollaboratorHelper
{
    public class EMParseBandPage
    {
        string band_;
        string country_;
        List<string> albumsURLs_;
        List<string> albumsNames_;
        List<string> albumsYears_;
        string genre_;

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;
        HtmlAgilityPack.HtmlDocument htmlDocDisco_;
        HtmlNode nodeBandContent_;

        public string Band
        {
            get { return band_; }
        }

        public string Country
        {
            get { return country_; }
        }

        public List<string> AlbumsURLs
        {
            get { return albumsURLs_; }
        }

        public List<string> AlbumsNames
        {
            get { return albumsNames_; }
        }

        public List<string> AlbumsYears
        {
            get { return albumsYears_; }
        }

        public string Genre
        {
            get { return genre_; }
        }

        public EMParseBandPage(string sourceHTML)
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute album nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            getBandNodes();

            if (nodeBandContent_ == null)
                return;

            // get band info
            band_ = getBandName();
            country_ = getBandCountry();
            genre_ = getBandGenre();

            // get band discography URL
            string discoURL = getBandDiscographyURL();

            // get band discography page source HTML
            string sourceDiscoHTML = Tools.GetWebPageSourceHTML(discoURL);
            htmlDocDisco_ = new HtmlAgilityPack.HtmlDocument();
            htmlDocDisco_.LoadHtml(sourceDiscoHTML);

            // get band albums URLs, names and years
            computeBandAlbumsInfos();
        }

        #region Parse info functions

        private void getBandNodes()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(htmlDoc_.DocumentNode, "div", "id", "wrapper");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "content_wrapper");

            nodeBandContent_ = Tools.NodeWithAttributeAndValue(node2, "div", "id", "band_content");
        }

        private string getBandName()
        {
            string band = ""; // default

            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeBandContent_, "div", "id", "band_info");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "h1", "class", "band_name");
            if (node2 == null) return "";

            band = Tools.CleanString(node2.InnerText);
            return band;
        }

        private string getBandCountry()
        {
            string country = "(no country found)"; // default

            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeBandContent_, "div", "id", "band_info");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "band_stats");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "dl", "class", "float_left");

            HtmlNode node4 = node3.Descendants("dd").FirstOrDefault();
            if (node4 == null) return country;

            country = Tools.CleanString(node4.InnerText);
            return country;
        }

        private string getBandGenre()
        {
            string genre = "(no genre found)"; // default

            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeBandContent_, "div", "id", "band_info");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "band_stats");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "dl", "class", "float_right");

            HtmlNode node4 = node3.Descendants("dd").FirstOrDefault();
            if (node4 == null) return genre;

            genre = Tools.CleanString(node4.InnerText);
            return genre;
        }

        private string getBandDiscographyURL()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeBandContent_, "div", "id", "band_tabs");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "band_tab_discography");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "band_disco");

            HtmlNode node4 = node3.Descendants("ul").FirstOrDefault();

            foreach (HtmlNode node in node4.Descendants("li"))
            {
                HtmlNode node5 = node.Descendants("a").FirstOrDefault();
                if (node5.Attributes.Contains("href"))
                {
                    string url = node5.Attributes["href"].Value;
                    if (url.EndsWith("/all"))
                        return url;
                }
            }

            return ""; // not found
        }

        private void computeBandAlbumsInfos()
        {
            List<string> urls = new List<string>();
            List<string> names = new List<string>();
            List<string> years = new List<string>();

            HtmlNode node1 = htmlDocDisco_.DocumentNode.Descendants("tbody").FirstOrDefault();

            foreach(HtmlNode node in node1.Descendants("tr"))
            {
                // get album URL and year
                HtmlNode node2 = node.Descendants("td").FirstOrDefault();
                HtmlNode node3 = node2.Descendants("a").FirstOrDefault();
                if (node3.Attributes.Contains("href"))
                {
                    // get album URL
                    string url = node3.Attributes["href"].Value;
                    urls.Add(url);

                    // get album name
                    string name = node3.InnerText;
                    names.Add(name);

                    // get album year
                    string year = ""; // default if not found
                    foreach (HtmlNode nodeYear in node.Descendants("td"))
                    {
                        if (!nodeYear.Attributes.Contains("class")) continue;

                        year = nodeYear.InnerText;
                        if (Tools.IsStringNumerical(year)) // ok, year found
                            break;
                    }
                        
                    years.Add(year);
                }   
            }

            albumsURLs_ = urls;
            albumsNames_ = names;
            albumsYears_ = years;
        }

        #endregion
    }
}
