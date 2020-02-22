using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponExt
{
    public class CompProperties_OversizedWeapon : CompProperties
    {
        public CompProperties_OversizedWeapon()
        {
            compClass = typeof(Comp_OversizedWeapon);
        }

        //public SoundDef soundMiss;
        //public SoundDef soundHitPawn;
        //public SoundDef soundHitBuilding;
        //public SoundDef soundExtra;
        //public SoundDef soundExtraTwo;

        public Vector3 offset = new Vector3(0, 0, 0); //No longer in-use.
        public Vector3 northOffset = new Vector3(0, 0, 0);
        public Vector3 eastOffset = new Vector3(0, 0, 0);
        public Vector3 southOffset = new Vector3(0, 0, 0);
        public Vector3 westOffset = new Vector3(0, 0, 0);
        public bool verticalFlipOutsideCombat = false;
        public bool verticalFlipNorth = false;
        public bool isDualWeapon = false;
        public float angleAdjustmentEast = 0f;
        public float angleAdjustmentWest = 0f;
        public float angleAdjustmentNorth = 0f;
        public float angleAdjustmentSouth = 0f;

        public GraphicData groundGraphic = null;
    }
}
