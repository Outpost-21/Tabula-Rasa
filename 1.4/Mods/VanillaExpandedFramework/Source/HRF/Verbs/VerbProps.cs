using System.Collections.Generic;
using RimWorld;
using Verse;

// ReSharper disable InconsistentNaming

namespace HRF
{
    public class VerbProps : VerbResourceProps
    {
        public bool allowCrushingRocks;
        public bool allowRepeat;
        public bool alwaysGetChunks = true;
        public HediffDef applyHediff;
        public int avatarDuration = 12000;
        public float blinkDuration;
        public int bounceCount;
        public float bounceDamageMultiplier = 1f;
        public int bounceDelay;
        public BouncePriority bouncePriority;
        public float bounceRange;
        public bool canHarvestTrees;
        public DamageDef cauterizeDamageDef;
        public float chargeDamageAmount;
        public DamageDef chargeDamageDef;
        public GraphicData chargeGraphic;
        public float chargeWidth;
        public Behavior effectBehavior;
        public List<MinCountDef> effectCount;
        public float effectDamageAmount;
        public DamageDef effectDamageDef;
        public int effectDuration;
        public HediffDef effectHediff;
        public float effectRadius;
        public float effectRate;
        public float extinguishRadius;
        public HediffDef feedbackHediff;
        public float impactDamageAmount;
        public DamageDef impactDamageDef;
        public float impactRadius;
        public float inwardSpeed;
        public DamageDef landingDamageDef;
        public float landingEffectRadius;
        public float leatherYield = 0.0f;
        public int maxTargets;
        public float meatYield = 0.5f;
        public float meteorDamageAmount;
        public DamageDef meteorDamageDef;
        public List<ThingDef> meteorMaterial;
        public float meteorSize;
        public float multishotRadius;
        public int multishotShots;
        public bool multishotTargetFriendly = false;
        public bool removeRoofs = true;
        public RotStage? requireRotStage;
        public float rotationSpeed;
        public PawnKindDef servantDef;
        public int servantDuration;
        public bool spawnRocks;
        public bool targetFriendly;
        public GraphicData terminusChainGraphic;
        public float yieldMultiplier = 1f;
    }

    public enum BouncePriority
    {
        Near,
        Far,
        Random
    }

    public struct MinCountDef
    {
        public int minCount;
        public string spawnDef;
        public string pawnKindDef;
    }

    public enum Behavior
    {
        Pull,
        Spin,
        Hold
    }
}