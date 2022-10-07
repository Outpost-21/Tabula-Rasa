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
    public class Building_TurretGunSmart : Building_TurretGun
    {
        /// <summary>
        /// Literally just removed the manned requirement so shit can be targeted manually.
        /// Really wonder why this wasn't set up for modders to be able to decide.
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
