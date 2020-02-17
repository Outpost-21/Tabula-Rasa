using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class MapComponent_CustomNetworks : MapComponent
    {
        public List<CustomNetwork> customNetworks = new List<CustomNetwork>();

        public Dictionary<CustomNetwork, List<IntVec3>> customNetworkCells = new Dictionary<CustomNetwork, List<IntVec3>>();

        public bool[] customNetworkGrid;

        public static bool showCustomNetworks = true;

        public int masterKey = -1;

        public MapComponent_CustomNetworks(Map map) : base(map)
        {
        }

        public CustomNetwork GenerateNetwork(Comp_CustomNetwork root, CustomNetwork forNetwork = null)
        {
            CustomNetwork customNetwork = forNetwork ?? new CustomNetwork(this);

            return customNetwork;
        }

        public void RegisterNetwork(CustomNetwork network)
        {

        }

        public void UnregisterNetwork()
        {

        }
    }
}
