using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Laser
{
    public class Building_LaserGunDef : ThingDef
    {
        public int beamPowerConsumption = 20;
        public Color beamColor = new Color(255, 255, 255);
    }
}
