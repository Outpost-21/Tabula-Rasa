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

    public class LaserGun :ThingWithComps, IBeamColorThing, IDrawnWeaponWithRotation
    {
        new public LaserGunDef def => base.def as LaserGunDef ?? LaserGunDef.defaultObj;

        public Color BeamColor => def.beamColor;

        int ticksPreviously = 0;
        public float RotationOffset
        {
            get {
                int ticks = Find.TickManager.TicksGame;
                UpdateRotationOffset(ticks - ticksPreviously);
                ticksPreviously = ticks;

                return rotationOffset;
            }
            set {
                rotationOffset = value;
                rotationSpeed = 0;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref beamColorIndex, "beamColorIndex", -1, false);
        }

        void UpdateRotationOffset(int ticks)
        {
            if (rotationOffset == 0) return;
            if (ticks <= 0) return;
            if (ticks > 30) ticks = 30;

            if (rotationOffset > 0)
            {
                rotationOffset -= rotationSpeed;
                if (rotationOffset < 0) rotationOffset = 0;
            }
            else if (rotationOffset < 0)
            {
                rotationOffset += rotationSpeed;
                if (rotationOffset > 0) rotationOffset = 0;
            }

            rotationSpeed += ticks * 0.01f;
        }

        private int beamColorIndex = -1;
        private float rotationSpeed = 0;
        private float rotationOffset = 0;
    }
}
