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
    public class LaserBeamDecoration
    {
        public ThingDef mote;
        public float spacing = 1.0f;
        public float initialOffset = 0;
        public float speed = 1.0f;
        public float speedJitter;
        public float speedJitterOffset;

    }

    public class LaserBeamDef : ThingDef
    {
        public float capSize = 1.0f;
        public float capOverlap = 1.1f / 64;

        public int lifetime = 30;
        public float impulse = 4.0f;

        public float beamWidth = 1.0f;
        public float shieldDamageMultiplier = 0.5f;
        public float seam = -1f;

        public List<LaserBeamDecoration> decorations;

        public EffecterDef explosionEffect;
        public EffecterDef hitLivingEffect;
        public ThingDef beamGraphic;

        public List<string> textures;

        public Material GetBeamMaterial(Color colour)
        {
            return MaterialPool.MatFrom(textures.RandomElement(), ShaderDatabase.TransparentPostLight, colour);
        }

        public bool IsWeakToShields
        {
            get { return shieldDamageMultiplier < 1f; }
        }

    }
}
