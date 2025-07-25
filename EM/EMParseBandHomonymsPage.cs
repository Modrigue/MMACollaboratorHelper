﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace MMACollaboratorHelper
{
    public class EMParseBandHomonymsPage
    {
        readonly string band_;
        readonly //List<string> countries_;
        List<string> homonymsURLs_;

        // parse objects
        readonly HtmlAgilityPack.HtmlDocument htmlDoc_;
        HtmlNode nodeContents_;

        public string Band
        {
            get { return band_; }
        }

        //public List<string> Countries
        //{
        //    get { return countries_; }
        //}

        public List<string> HomonymsURLs
        {
            get { return homonymsURLs_; }
        }

        public EMParseBandHomonymsPage(string sourceHTML)
        {
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute album nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            getBandNodes();

            if (nodeContents_ == null)
                return;

            // get band info
            band_ = getBandName();
            //countries_ = getBandCountries();
            homonymsURLs_ = getBandHomonymsURLs();
        }


        public string GetBandNameFromNonHomonymPage()
        {
            HtmlNode nodeBandContent = Tools.NodeWithAttributeAndValue(nodeContents_, "div", "id", "band_content");

            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeBandContent, "div", "id", "band_info");
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "h1", "class", "band_name");
            if (node2 == null) return "";

            string band = Tools.CleanString(node2.InnerText);
            return band;
        }

        #region Parse info functions

        private void getBandNodes()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(htmlDoc_.DocumentNode, "div", "id", "wrapper");
            nodeContents_ = Tools.NodeWithAttributeAndValue(node1, "div", "id", "content_wrapper");
        }

        private string getBandName()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeContents_, "h1", "class", "page_title");
            if (node1 == null) return "";

            string band = Tools.CleanString(node1.InnerText);
            return band;
        }

        private List<string> getBandHomonymsURLs()
        {
            List<string> urls = new List<string>();

            foreach (HtmlNode nodeLi in nodeContents_.Descendants("li"))
            {
                foreach (HtmlNode node in nodeLi.Descendants("a"))
                {
                    // new disc found
                    if (node.Attributes.Contains("href"))
                    {
                        string url = node.Attributes["href"].Value;
                        if(url.StartsWith("http://www.metal-archives.com/bands/" + band_ + "/")
                        || url.StartsWith("https://www.metal-archives.com/bands/" + band_ + "/")
                            )
                            urls.Add(url);
                    }
                }
            }

            return urls;
        }

        #endregion
    }
}
