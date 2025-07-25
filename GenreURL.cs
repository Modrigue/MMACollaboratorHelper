﻿namespace MMACollaboratorHelper
{
    // used for genres combo box
    public class GenreURL
    {
        readonly string genre;
        readonly string url;

        public string Genre
        {
            get { return genre; }
        }

        public string Url
        {
            get { return url; }
        }

        public GenreURL(string genre, string url)
        {
            this.genre = genre;
            this.url = url;
        }
    }
}
