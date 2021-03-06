﻿using Newtonsoft.Json;
using PropertyChanged;
using System;

namespace wallabag.Api.Models
{
    /// <summary>
    /// Represents a tag that can be applied to one or more items.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class WallabagTag : IComparable
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the label of a tag.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the slug of the tag.
        /// </summary>
        [JsonProperty("slug")]
        public string Slug { get; set; }

        public override string ToString() => Label;
        public override int GetHashCode() => Id;
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(WallabagTag)))
            {
                var comparedTag = obj as WallabagTag;
                return Label.Equals(comparedTag.Label);
            }
            return false;
        }
        public int CompareTo(object obj) => ((IComparable)Label).CompareTo(obj);
    }
}
