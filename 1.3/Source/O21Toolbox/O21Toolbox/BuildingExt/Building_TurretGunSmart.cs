using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox
{
    public class Building_TurretGunSmart : Building_TurretGun
    {
        /// <summary>
        /// Literally just removed the manned requirement so shit can be targeted manually.
        /// </summary>
        public new bool PlayerControlled
        { 
            get
            {
                return base.Faction == Faction.OfPlayer;
            } 
        }
    }
}
