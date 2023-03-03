using RimWorld;
using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class UseProps
    {
        public HediffResourceDef hediff;
        public bool hediffRequired;
        public string cannotUseMessageKey;

        public bool toggleResourceUse;
        public string toggleResourceGizmoTexPath;
        public string toggleResourceLabel;
        public string toggleResourceDesc;
        public string texPathToggledOn;
        public GlowerOptions glowerOptions;
        public bool glowOnlyPowered;
        public float resourcePerSecond = -1f;
        public float resourceOnComplete = -1f;
        public BodyPartDef applyToPart;
        public bool addHediffIfMissing;
        public bool qualityScalesResourcePerSecond;
        public List<StatModifier> statOffsets;
        public List<StatModifier> statFactors;

        public int increaseQuality = -1;
        public QualityCategory increaseQualityCeiling = QualityCategory.Legendary;

        public List<StatBonus> outputStatOffsets;
        public List<StatBonus> outputStatFactors;
        public bool defaultToggleState;

        public float resourceOnSow;
        public float resourceOnHarvest;
        public bool scaleWithGrowthRate;
        public string cannotSowMessageKey;
        public string cannotHarvestMessageKey;

        public float resourceOnTaming;
        public float resourceOnTraining;
        public float resourceOnGather;

        public string cannotTameMessageKey;
        public string cannotTrainMessageKey;
        public string cannotGatherMessageKey;

        public float resourceOnStrike;
        public string cannotMineMessageKey;
    }
}