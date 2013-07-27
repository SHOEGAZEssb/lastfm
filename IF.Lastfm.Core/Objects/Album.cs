﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IF.Lastfm.Core.Objects
{
    public class Album : ILastFmObject
    {
        #region Properties

        public string Name { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        
        public string ArtistName { get; set; }
        public string ArtistId { get; set; }
        
        public DateTime ReleaseDateUtc { get; set; }

        public int ListenerCount { get; set; }
        public int TotalPlayCount { get; set; }

        public string Mbid { get; set; }

        public IEnumerable<Tag> TopTags { get; set; }

        public Uri Url { get; set; }
        
        #endregion

        /// <summary>
        /// TODO datetime parsing
        /// TODO images
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static Album ParseJToken(JToken token)
        {
            var a = new Album();

            a.ArtistName = token.Value<string>("artist");
            a.ArtistId = token.Value<string>("id");
            a.ListenerCount = token.Value<int>("listeners");
            a.Mbid = token.Value<string>("mbid");
            a.Name = token.Value<string>("name");
            a.TotalPlayCount = token.Value<int>("playcount");

            a.Url = new Uri(token.Value<string>("url"), UriKind.Absolute);

            var tracksToken = token.SelectToken("tracks").SelectToken("track");
            if (tracksToken != null)
            {
                a.Tracks = tracksToken.Children().Select(trackToken => Track.ParseJToken(trackToken, a.Name));
            }

            var tagsToken = token.SelectToken("toptags").SelectToken("tag");
            if (tagsToken != null)
            {
                a.TopTags = tagsToken.Children().Select(Tag.ParseJToken);
            }

            return a;
        }

        internal static string GetNameFromJToken(JToken albumToken)
        {
            var name = albumToken.Value<string>("name");

            if (string.IsNullOrEmpty(name))
            {
                name = albumToken.Value<string>("#text");
            }

            return name;
        }
    }
}
