using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Net;

namespace MMACollaboratorHelper
{
    public class MMAParseExistingBandAlbums
    {
        List<string> albumsNames_;
        List<string> albumsYears_;
        List<string> albumsNamesYears_;

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;

        public List<string> AlbumsNames
        {
            get { return albumsNames_; }
        }

        public List<string> AlbumsYears
        {
            get { return albumsYears_; }
        }

        public List<string> AlbumsNamesYears
        {
            get { return albumsNamesYears_; }
        }

        public MMAParseExistingBandAlbums(string sourceHTML)
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute album nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);

            // get band albums URLs and years
            computeBandAlbumsInfos();
        }

        #region Parse info functions


        private void computeBandAlbumsInfos()
        {
            List<string> names = new List<string>();
            List<string> years = new List<string>();
            List<string> namesyears = new List<string>();

            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "mainSite");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "class", "colmask holygrail");
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "colmid");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "class", "colleft");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "class", "col1wrap");
            HtmlNode node7 = Tools.NodeWithAttributeAndValue(node6, "div", "class", "col1");

            List<HtmlNode> nodesAlbums = Tools.NodeListWithAttributeAndValue(node7, "div", "class", "discographyContainer");

            //foreach(HtmlNode node in node7.Descendants())
            foreach (HtmlNode nodeAlbum in nodesAlbums)
            {
                // get album name
                string name = ""; // default
                foreach (HtmlNode nodeAlbumName in nodeAlbum.Descendants("a"))
                {
                    if (nodeAlbumName.Attributes.Contains("href"))
                    {
                        string nameCandidate = nodeAlbumName.InnerText;
                        if (!String.IsNullOrEmpty(nameCandidate)) // name found
                        {
                            name = nameCandidate;
                            break;
                        }
                    }
                }

                // get album year
                string year = ""; // default
                string spanString = "</span>";
                int indexYear = nodeAlbum.InnerHtml.LastIndexOf(spanString) + spanString.Length;
                if (indexYear >= 0)
                {
                    year = nodeAlbum.InnerHtml.Substring(indexYear);
                    year = Tools.CleanString(year);
                }

                names.Add(name);
                years.Add(year);

                string nameyear = name + "_" + year;
                namesyears.Add(nameyear);
            }

            albumsNames_ = names;
            albumsYears_ = years;
            albumsNamesYears_ = namesyears;
        }

        #endregion
    }
}
