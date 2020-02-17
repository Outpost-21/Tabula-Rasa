using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    /// <summary>
    /// Defines a new network.
    /// </summary>
    public class NetworkDef : Def
    {
        /// <summary>
        /// Measurement used to describe the network units.
        /// </summary>
        public string networkMeasurement = "";

        /// <summary>
        /// Whether or not a network requires connections.
        /// </summary>
        public bool wirelessNetwork = false;

        /// <summary>
        /// Type of material transferred.
        /// </summary>
        public enum networkType
        {
            energy,
            gas,
            liquid,
            item
        }
    }
}
