using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class UpdateDef : Def
    {
        /// <summary>
        /// Year, Month, Day. Used for Sorting.
        /// </summary>
        public string date = "0000/00/00";
        /// <summary>
        /// Banner image, should be 500x40.
        /// </summary>
        public string banner;
        /// <summary>
        /// The text content of the update.
        /// </summary>
        public string content;
        public List<UpdateItem> contentList = new List<UpdateItem>();
        /// <summary>
        /// Link opened when clicking on the update.
        /// </summary>
        public string linkUrl;
        public List<UpdateLink> links;
        /// <summary>
        /// If True, makes the update stand out in the list.
        /// Only use for VITAL information, like a new dependency or major update.
        /// </summary>
        public bool important = false;
    }

    public class UpdateItem
    {
        public string header;
        public string image;
        public string text;
    }

    public class UpdateLink
    {
        public string linkLabel;
        public string linkUrl;
        public string linkTex;
    }
}
