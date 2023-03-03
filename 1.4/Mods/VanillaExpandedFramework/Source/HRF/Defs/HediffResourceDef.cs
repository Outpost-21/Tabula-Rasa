using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace HRF
{
	public class HediffResourceDef : HediffDef
    {
        public float maxResourceCapacity;
        public float initialResourceAmount;
		public bool keepWhenEmpty;
		public int lifetimeTicks = -1;
		public bool hideResourceAmount;
		public bool isResource = true;

		public bool showResourceBar;
		public Color? backgroundBarColor;
		public Color? progressBarColor;
		public Color? resourceBarTextColor;

		public bool sendLetterWhenGained;
		public LetterDef letterType;
		public string letterTitleKey;
		public string letterMessageKey;

		public List<RequiredHediff> requiredHediffs;

		public bool showInResourceTab;
		public float resourcePerSecondFactor = 1f;

		public bool ignoreTerrain;
		public bool useAbsoluteSeverity;
		public bool restrictResourceCap = true;

		public GraphicData fallbackTogglingGraphicData;

		public List<AbilityDef> randomAbilitiesPool;
		public bool retainRandomLearnedAbilities;
	}
}
