using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMACollaboratorHelper
{
    public class MMAParseGenres
    {
        List<string> subGenres_;
        List<string> subGenresURLs_;

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;

        public List<string> SubGenres
        {
            get { return subGenres_; }
        }

        public List<string> SubGenresURLs
        {
            get { return subGenresURLs_; }
        }

        public MMAParseGenres(string sourceHTML)
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute subgenres
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeSubgenres();
        }

        private void computeSubgenres()
        {
            List<string> subGenres = new List<string>();
            List<string> subGenresURLs = new List<string>();

            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("html").FirstOrDefault();
            HtmlNode node2 = node1.Descendants("body").FirstOrDefault();
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "mainSite");
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "colmask holygrail");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "class", "colmid");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "class", "colleft");
            HtmlNode node7 = Tools.NodeWithAttributeAndValue(node6, "div", "class", "col2");
            
            List<HtmlNode> nodeList1 = Tools.NodeListWithAttributeAndValue(node7, "ul", "class", "unpadded-list");

            foreach (HtmlNode node in nodeList1)
            {
                foreach (HtmlNode nodeLi in node.Descendants("li"))
                {
                    //List<HtmlNode> nodeListSubgenres = Tools.NodeListWithAttributeAndValue(nodeLi, "a", "class", "MainSubgenreItem");
                    List<HtmlNode> nodeListSubgenres = nodeLi.Descendants("a").ToList();
                    
                    foreach (HtmlNode nodeSubgenre in nodeListSubgenres)
                    {
                        bool isSubGenreNode = false;
                        if (nodeSubgenre.Attributes.Contains("href"))
                            if (nodeSubgenre.Attributes["href"].Value.StartsWith("/subgenre/"))
                                isSubGenreNode = true; // ok

                        if (!isSubGenreNode) continue;

                        // get subgenre name
                        string subGenre = nodeSubgenre.InnerText;
                        //subGenre = Tools.CleanString(subGenre);
                        string TAB_RC = "&#8627; "; // sub-subgenre
                        if (subGenre.StartsWith(TAB_RC))
                            subGenre = subGenre.Replace(TAB_RC, "  ↳  ");

                        // get subgenre URL
                        string subGenreURL = nodeSubgenre.Attributes["href"].Value;

                        subGenres.Add(subGenre);
                        subGenresURLs.Add(subGenreURL);
                    }
                }
            }

            subGenres_ = subGenres;
            subGenresURLs_ = subGenresURLs;
        }
    }
}
