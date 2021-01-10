using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MMACollaboratorHelper
{
    public class Tools
    {
        private static string logEventsPath_;
        private static string logErrorsPath_;

        private static HttpWebRequest request_;

        #region Web functions

        public static void Initialize()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12
                | SecurityProtocolType.Ssl3;
        }

        public static string GetWebPageSourceHTML(string url)
        {
            string sourceHTML = "";

            try
            {
                request_ = (HttpWebRequest)WebRequest.Create(url);
                request_.Timeout = 2 * 60 * 1000; // in ms
                HttpWebResponse response = (HttpWebResponse)request_.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        //readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    }

                    //string data = readStream.ReadToEnd();
                    sourceHTML = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception /*ex*/)
            {
                //
            }

            return sourceHTML;
        }

        public static void AbortWebRequest()
        {
            try
            {
                if (request_ != null)
                    request_.Abort();
            }
            catch { }
        }

        // search for first of default node with format "<type ...attribute = value..."
        public static HtmlNode NodeWithAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            if (node == null)
                return null;

            HtmlNode resNode = node.Descendants(type)
                .Where(n => n.Attributes.Contains(attribute) && n.Attributes[attribute].Value == value)
                .FirstOrDefault();

            return resNode;
        }

        // return list of nodes with format "<type ...attribute = value..."
        public static List<HtmlNode> NodeListWithAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();

            if (node == null)
                return nodes;

            IEnumerable<HtmlNode> resNodes = node.Descendants(type)
                .Where(n => n.Attributes.Contains(attribute) && n.Attributes[attribute].Value == value);

            nodes = resNodes.ToList();
            return nodes;
        }

        // returns true iff node has format "<type ...attribute = value..."
        public static bool NodeHasAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            bool res = false;

            if (node == null)
                return false;

            if (node.Name == type && node.Attributes.Contains(attribute) && node.Attributes[attribute].Value == value)
                res = true;

            return res;
        }

        #endregion

        #region String functions

        public static string ToTitleCase(string text)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }

        public static string CleanString(string text)
        {
            string res = text.Trim();
            res = res.Replace("\t", "");
            res = res.Replace("\n", "");
            res = HtmlEntity.DeEntitize(res);

            return res;
        }

        public static bool IsStringNumerical(string text)
        {
            int myInt;
            bool isNumerical = int.TryParse(text, out myInt);

            return isNumerical;
        }

        public static bool IsStringURL(string text)
        {
            return text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || text.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public static string TrimTime(string time)
        {
            if (time.StartsWith("0"))
                if (time.Length == 5 || time.Length == 8)
                {
                    time = time.Substring(1);
                }

            return time;
        }

        public static string RemoveWindowsForbiddenCharacters(string text)
        {
            string newText = text;

            newText = newText.Replace("\\", "");
            newText = newText.Replace("/", "");
            newText = newText.Replace(":", "");
            newText = newText.Replace("*", "");
            newText = newText.Replace("?", "");
            newText = newText.Replace("\"", "");
            newText = newText.Replace("<", "");
            newText = newText.Replace(">", "");
            newText = newText.Replace("|", "");

            return newText;
        }

        #endregion

        public static string DownloadDirectory()
        {
            string dirUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dirDownload = Path.Combine(dirUser, "Downloads", "MMA_ALBUMS_DATA");

            return dirDownload;
        }

        public static string DownloadFilePath(string band, string album, string year)
        {
            string bandString = Tools.RemoveWindowsForbiddenCharacters(band);
            string albumString = Tools.RemoveWindowsForbiddenCharacters(album);

            // create directory if not existing
            //if (!Directory.Exists(dirBand))
            //    Directory.CreateDirectory(dirBand);

            // create file name
            //string filename = year + "_" + albumString;
            string filename = bandString.ToUpper() + "_" + year + "_" + albumString;

            //string dirBand = Path.Combine(DownloadDirectory(), bandString);
            //return Path.Combine(dirBand, filename);

            return Path.Combine(DownloadDirectory(), filename);
        }


        #region Log functions

        public static void InitLogsFiles()
        {
            // init download directory
            if (!Directory.Exists(DownloadDirectory()))
                Directory.CreateDirectory(DownloadDirectory());

            string datetime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string logEventsFile = "_log_events" + "_" + datetime + ".txt";
            string logErrorsFile = "_log_errors" + "_" + datetime + ".txt";

            logEventsPath_ = Path.Combine(DownloadDirectory(), logEventsFile);
            logErrorsPath_ = Path.Combine(DownloadDirectory(), logErrorsFile);

            //if (!File.Exists(logEventsPath_))
            //    File.Create(logEventsPath_);

            //if (!File.Exists(logErrorsPath_))
            //    File.Create(logErrorsPath_);
        }

        public static void LogEvent(string text)
        {
            // init download directory
            if (!Directory.Exists(DownloadDirectory()))
                Directory.CreateDirectory(DownloadDirectory());

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@logEventsPath_, true))
            {
                 file.WriteLine(DateTime.Now.ToString() + ": " + text);
                 //file.Write(text);
            }
        }

        public static void LogError(string text)
        {
            // init download directory
            if (!Directory.Exists(DownloadDirectory()))
                Directory.CreateDirectory(DownloadDirectory());

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@logErrorsPath_, true))
            {
                file.WriteLine(DateTime.Now.ToString() + ": " + text);
                //file.Write(text);
            }
        }

        #endregion
    }
}
