using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using Verse;
using Verse.Sound;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace HRF
{
    public class AbilityData
    {
        public AbilityTreeDef abilityTree;
        public AbilityTier abilityTier;
    }
    public class CompProperties_PawnClass : CompProperties
    {
        public CompProperties_PawnClass()
        {
            this.compClass = typeof(CompPawnClass);
        }
    }

    [HotSwappable]
    public class CompPawnClass : ThingComp
    {
        public int level;
        public float xpPoints;
        public int abilityPoints;
        private float previousXp;
        public float GainedXPSinceLastLevel => xpPoints - previousXp;
        public float RequiredXPtoGain => ClassTraitDef.xpPerLevelRequirement * (level + 1);
        public Pawn pawn => this.parent as Pawn;
        public CompAbilities compAbilities => pawn.GetComp<CompAbilities>();

        public List<AbilityDef> learnedAbilities = new List<AbilityDef>();
        public int MaxLevel => ClassTraitDef.maxLevel;
        public HediffResource HediffResource
        {
            get
            {
                var classTrait = ClassTraitDef;
                if (classTrait.resourceHediff != null)
                {
                    return this.pawn.health.hediffSet.GetFirstHediffOfDef(classTrait.resourceHediff) as HediffResource;
                }
                return null;
            }
        }
        public Ability GetLearnedAbility(AbilityDef abilityDef) => compAbilities.LearnedAbilities.FirstOrDefault(x => x.def == abilityDef);
        public bool HasClassTrait(out Trait classTrait)
        {
            classTrait = ClassTrait;
            return classTrait != null;
        }

        public bool HasClass(out ClassTraitDef classTrait)
        {
            classTrait = ClassTraitDef;
            return classTrait != null;
        }
        public void GainXP(float xp)
        {
            var classTrait = ClassTraitDef;
            if (level < MaxLevel)
            {
                xpPoints += xp;
                while (xpPoints >= previousXp + RequiredXPtoGain)
                {
                    SetLevel(level + 1);
                    if (level > classTrait.initialLevel && PawnUtility.ShouldSendNotificationAbout(pawn) && classTrait.sendMessageOnLevelUp)
                    {
                        Messages.Message((classTrait.levelUpMessageKey ?? "ART.PawnLevelUp").Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.PositiveEvent);
                        if (pawn.Spawned)
                        {
                            if (classTrait.moteOnLevelUp != null)
                            {
                                MoteMaker.MakeStaticMote(pawn.Position, pawn.Map, classTrait.moteOnLevelUp);
                            }
                            if (classTrait.soundOnLevelUp != null)
                            {
                                classTrait.soundOnLevelUp.PlayOneShot(pawn);
                            }
                        }
                    }
                    abilityPoints += ClassTraitDef.abilityPointsPerLevel;
                    if (level == MaxLevel)
                    {
                        break;
                    }
                }
            }
            if (level == MaxLevel && xpPoints > previousXp + RequiredXPtoGain)
            {
                xpPoints = previousXp + RequiredXPtoGain;
            }
        }

        public void SetLevel(int newLevel)
        {
            previousXp += RequiredXPtoGain;
            this.level = newLevel;
            var hediffResouce = HediffResource;
            if (hediffResouce != null)
            {
                hediffResouce.SetResourceAmount(newLevel);
            }
        }
        public void Init(ClassTraitDef trait)
        {
            if (trait.resourceHediff != null)
                pawn.health.AddHediff(trait.resourceHediff);
            if (trait.addHediff != null)
                pawn.health.AddHediff(trait.addHediff);
            learnedAbilities = new List<AbilityDef>();
            UpgradeTo(trait.initialLevel);
        }

        public void UpgradeTo(int levelTarget)
        {
            while (levelTarget > level)
            {
                GainXP(RequiredXPtoGain);
            }
        }

        public bool Learned(AbilityDef abilityDef)
        {
            return compAbilities.LearnedAbilities.Exists(x => x.def == abilityDef);
        }

        public bool FullyLearned(AbilityDef abilityDef)
        {
            if (Learned(abilityDef))
            {
                var abilityData = GetAbilityDataFrom(abilityDef);
                return abilityData.abilityTree.abilityTiers.Last() == abilityData.abilityTier;
            }
            return false;
        }

        public AbilityData GetAbilityDataFrom(AbilityDef abilityDef)
        {
            AbilityTreeDef abilityTree = null;
            AbilityTier abilityTier = null;
            foreach (var tree in ClassTraitDef.classAbilities)
            {
                abilityTier = tree.abilityTiers.FirstOrDefault(x => x.abilityDef == abilityDef);
                if (abilityTier != null)
                {
                    abilityTree = tree;
                    break;
                }
            }
            return new AbilityData { abilityTree = abilityTree, abilityTier = abilityTier };
        }

        public bool CanUnlockAbility(AbilityDef abilityDef)
        {
            var abilityTier = GetAbilityDataFrom(abilityDef).abilityTier;
            if (abilityTier.minimumLevel > level)
            {
                return false;
            }
            if (abilityPoints < abilityTier.abilityPointsToLearn)
            {
                return false;
            }
            return true;
        }

        public void LearnAbility(AbilityDef abilityDef, bool spendAbilityPoints = true)
        {
            var comp = compAbilities;
            var abilityData = GetAbilityDataFrom(abilityDef);
            foreach (var tier in abilityData.abilityTree.abilityTiers)
            {
                learnedAbilities.Remove(tier.abilityDef);
                var ability = GetLearnedAbility(tier.abilityDef);
                if (ability != null)
                {
                    comp.LearnedAbilities.Remove(ability);
                }
            }
            learnedAbilities.Add(abilityDef);
            comp.GiveAbility(abilityDef);
            if (spendAbilityPoints)
            {
                var abilityPointsToSpent = abilityData.abilityTier.abilityPointsToLearn;
                abilityPoints -= abilityPointsToSpent;
            }
        }

        public void Erase(ClassTraitDef classTrait)
        {
            if (classTrait.resourceHediff != null)
            {
                var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(classTrait.resourceHediff);
                if (hediff != null)
                    pawn.health.RemoveHediff(hediff);
            }

            var comp = compAbilities;
            foreach (var abilityDef in learnedAbilities)
            {
                var ability = this.GetLearnedAbility(abilityDef);
                if (ability != null)
                {
                    comp.LearnedAbilities.Remove(ability);
                }
            }
            learnedAbilities.Clear();
            level = 0;
            SetLevel(0);
            xpPoints = 0;
            abilityPoints = 0;
        }
        public Trait ClassTrait
        {
            get
            {
                foreach (var trait in pawn.story.traits.allTraits)
                {
                    if (trait.def is ClassTraitDef classTraitDef)
                    {
                        return trait;
                    }
                }
                return null;
            }
        }
        public ClassTraitDef ClassTraitDef
        {
            get
            {
                foreach (var trait in pawn.story.traits.allTraits)
                {
                    if (trait.def is ClassTraitDef classTraitDef)
                    {
                        return classTraitDef;
                    }
                }
                return null;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref level, "CompPawnClass_" + nameof(level));
            Scribe_Values.Look(ref xpPoints, "CompPawnClass_" + nameof(xpPoints));
            Scribe_Values.Look(ref previousXp, "CompPawnClass_" + nameof(previousXp));
            Scribe_Values.Look(ref abilityPoints, "CompPawnClass_" + nameof(abilityPoints));
            Scribe_Collections.Look(ref learnedAbilities, "CompPawnClass_" + nameof(learnedAbilities), LookMode.Def);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (learnedAbilities is null)
                    learnedAbilities = new List<AbilityDef>();
            }
        }
    }

    public class ClassTraitDef : TraitDef
    {
        public int initialLevel;
        public SimpleCurve randomLevelAtPawnGeneration;
        public int maxLevel;
        public int abilityPointsPerLevel;
        public float xpPerLevelRequirement;
        public float xpPerHumanlikeValueWhenKilling;
        public float xpPerNonHumanValueWhenKilling;
        public float xpPerSkillGain;
        public bool sendMessageOnLevelUp;
        public string levelUpMessageKey;
        public ThingDef moteOnLevelUp;
        public SoundDef soundOnLevelUp;
        public float valuePerLevelOffset;
        public HediffResourceDef resourceHediff;
        public List<AbilityTreeDef> classAbilities;
        public HediffDef addHediff;

        [NoTranslate]
        public string iconPath;

        public Texture2D uiIcon = BaseContent.BadTex;
        public override void PostLoad()
        {
            if (!string.IsNullOrEmpty(iconPath))
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    uiIcon = ContentFinder<Texture2D>.Get(iconPath);
                });
            }
        }
    }

    public class AbilityTreeDef : Def
    {
        public List<AbilityTier> abilityTiers;
    }

    public class AbilityTier
    {
        public AbilityDef abilityDef;
        public int minimumLevel;
        public int abilityPointsToLearn;
    }
}
