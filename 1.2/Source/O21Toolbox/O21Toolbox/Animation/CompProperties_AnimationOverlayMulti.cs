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
    public class CompProperties_AnimationOverlayMulti : CompProperties
    {
        public CompProperties_AnimationOverlayMulti()
        {
            this.compClass = typeof(Comp_AnimationOverlayMulti);
        }

        public AnimationWorker Worker
        {
            get
            {
                if(workerInt == null)
                {
                    workerInt = (AnimationWorker)Activator.CreateInstance(animWorker);
                }
                return this.workerInt;
            }
        }

        public List<AnimationOverlaySettingMulti> settings = new List<AnimationOverlaySettingMulti>();

        public bool idleWithFuel = false;

        public float damageThreshold = 0f;

        public Type animWorker = typeof(AnimationWorker);

        private AnimationWorker workerInt;

        public List<int> animWorkerFrames = new List<int>();
    }

    public class AnimationOverlaySettingMulti
    {
        public AnimationStatus type = AnimationStatus.inactive;

        public string graphicPath;

        public Graphic[] graphics;

        public int ticksPerFrame;

        public int frameCount;
    }
}
