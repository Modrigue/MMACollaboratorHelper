using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MMACollaboratorHelper
{
    public class MMAParseGenreBands
    {
        List<string> genreBands_;
        List<string> genreBandsURLs_;

        // parse objects
        readonly HtmlAgilityPack.HtmlDocument htmlDoc_;

        public List<string> GenreBands
        {
            get { return genreBands_; }
        }

        public List<string> GenreBandsURLs
        {
            get { return genreBandsURLs_; }
        }

        public MMAParseGenreBands(string sourceHTML)
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute subgenres
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeGenreBands();
        }

        private void computeGenreBands()
        {
            List<string> genreBands = new List<string>();
            List<string> genreBandsURLs = new List<string>();

            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("html").FirstOrDefault();
            HtmlNode node2 = node1.Descendants("body").FirstOrDefault();
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "mainSite");
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "colmask holygrail");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "class", "colmid");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "class", "colleft");
            HtmlNode node7 = Tools.NodeWithAttributeAndValue(node6, "div", "class", "col1wrap");
            HtmlNode node8 = Tools.NodeWithAttributeAndValue(node7, "div", "class", "col1");

            foreach (HtmlNode node in node8.Descendants("ul"))
            {
                foreach (HtmlNode nodeLi in node.Descendants("li"))
                {
                    List<HtmlNode> nodeListSubgenres = nodeLi.Descendants("a").ToList();

                    foreach (HtmlNode nodeBand in nodeListSubgenres)
                    {
                        bool isBandNode = false;
                        if (nodeBand.Attributes.Contains("href"))
                            if (nodeBand.Attributes["href"].Value.StartsWith("/artist/"))
                                isBandNode = true; // ok

                        if (!isBandNode) continue;

                        // get band name
                        string band = Tools.ToTitleCase(nodeBand.InnerText);
                        //band = Tools.CleanString(band);

                        // get band URL
                        string bandURL = nodeBand.Attributes["href"].Value;

                        genreBands.Add(band);
                        genreBandsURLs.Add(bandURL);
                    }
                }
            }

            genreBands_ = genreBands;
            genreBandsURLs_ = genreBandsURLs;
        }
    }
}
