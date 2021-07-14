using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Animation
{
    public class CompProperties_AnimationOverlay : CompProperties
    {
        public CompProperties_AnimationOverlay()
        {
            this.compClass = typeof(Comp_AnimationOverlay);
        }

        public List<AnimationOverlaySetting> settings = new List<AnimationOverlaySetting>();

        public bool idleWithFuel = false;

        public float damageThreshold = 0f;
    }

    public class AnimationOverlaySetting
    {
        public AnimationStatus type = AnimationStatus.inactive;

        public string graphicPath;

        public Graphic[] graphics;

        public int fps;

        public int frameCount;
    }

    public enum AnimationStatus
    {
        inactive,
        idle,
        inUse,
        damaged
    }
}
