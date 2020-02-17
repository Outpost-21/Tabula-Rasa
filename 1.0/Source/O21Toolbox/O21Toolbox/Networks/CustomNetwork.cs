using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Networks
{
    public class CustomNetwork
    {
        public MapComponent_CustomNetworks manager;

        public CustomNetwork(MapComponent_CustomNetworks manager)
        {
            this.manager = manager;
        }

        public CustomNetwork(Comp_CustomNetwork root, MapComponent_CustomNetworks manager)
        {
            this.manager = manager;
            manager.GenerateNetwork(root, this);
            manager.RegisterNetwork(this);
        }
    }
}
