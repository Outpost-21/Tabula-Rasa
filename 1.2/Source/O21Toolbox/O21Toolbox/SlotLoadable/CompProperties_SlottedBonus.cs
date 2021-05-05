using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.SlotLoadable
{
    public class CompProperties_SlottedBonus : CompProperties
    {
        public CompProperties_SlottedBonus()
        {
            compClass = typeof(Comp_SlottedBonus);
        }

        public GraphicData graphicData;

        public List<ThingDef> additionalProjectiles = new List<ThingDef>();

        public Color color = Color.white;

        public DamageDef damageDef = null;

        public float armorPenetration = 0f;

        public float muzzleFlashMod = 0.0f;

        public ThingDef projectileReplacer = null;

        public SoundDef soundCastReplacer = null;
        public List<StatModifier> statModifiers = null;

        public float weaponRangeMod = 0.0f;

        public string pathSuffix;

        public string pathPrefix;
    }
}
