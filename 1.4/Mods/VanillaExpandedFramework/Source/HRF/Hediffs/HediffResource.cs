using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Noise;
using Verse.Sound;
using VFECore.Abilities;
using AbilityDef = VFECore.Abilities.AbilityDef;

namespace HRF
{
    public class HediffResource : Hediff_Ability, IAdjustResource, IDrawable
    {
        public new HediffResourceDef def => base.def as HediffResourceDef;
        private float resourceAmount;
        public int duration;
        public int delayTicks;
        public int lastHealingEffectTick;
        public int lastTendingEffectTick;
        public int lastDamagingEffectTick;

        public int lastStageSwitchTick;
        public int lastStageActivatedTick;
        public int stageSwitchTickToActivate;
        public int stageIndexToActivate = -1;
        public int curCooldownPeriod;
        public int curChangeTime;
        public int previousStageIndex = -1;
        public List<AbilityDef> grantedRandomAbilities;
        public List<AbilityDef> grantedStaticAbilities;

        public Dictionary<int, SavedSkillRecordCollection> savedSkillRecordsByStages;
        public List<Thing> amplifiers = new List<Thing>();
        public HediffStageResource CurStageResource => this.CurStage as HediffStageResource;
        public IEnumerable<Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>> GetResourceStorages()
        {
            foreach (var adjustResource in this.pawn.GetAllAdjustResources())
            {
                if (adjustResource is CompAdjustHediffs comp)
                {
                    foreach (var pair in comp.GetResourceStoragesFor(this.def))
                    {
                        yield return new Tuple<CompAdjustHediffs, ResourceProperties, ResourceStorage>(comp, pair.Item2, pair.Item3);
                    }
                }
            }
        }
        public float ResourceFromStorages
        {
            get
            {
                var amount = 0f;
                foreach (var tuple in GetResourceStorages())
                {
                    amount += tuple.Item3.ResourceAmount;
                }
                return amount;
            }
        }
        public float StoragesTotalCapacity
        {
            get
            {
                var amount = 0f;
                foreach (var tuple in GetResourceStorages())
                {
                    amount += tuple.Item3.ResourceCapacity;
                }
                return amount;
            }
        }
        public float ResourceAmountNoStorages => resourceAmount;
        public float ResourceAmount => resourceAmount + ResourceFromStorages;
        public void SetResourceAmount(float value, ResourceProperties source = null)
        {
            ChangeResourceAmount(value - this.ResourceAmount, source);
        }
        public void ChangeResourceAmount(float offset, ResourceProperties source = null)
        {
            if (this.def.isResource is false)
            {
                return;
            }
            var hediffDefnameToCheck = "";// "FE_FuelPackHediff";
            var resourceCapacity = this.ResourceCapacity;
            var resourceFromStorages = this.ResourceFromStorages;
            if (this.def.defName == hediffDefnameToCheck)
            {
                //resourceCapacity = 10;
                //resourceAmount = 10;
                //offset = -15;
            }

            var storages = GetResourceStorages();
            var totalValue = resourceAmount + resourceFromStorages;

            if (this.def.defName == hediffDefnameToCheck)
            {
                HRFLog.Message("START");
                HRFLog.Message("1 offset: " + offset);
                HRFLog.Message("resourceAmount: " + this.resourceAmount);
                HRFLog.Message("resourceFromStorages: " + resourceFromStorages);
                HRFLog.Message("resourceCapacity: " + resourceCapacity);
                HRFLog.Message("totalValue: " + totalValue);
                HRFLog.Message("-----------------");
            }
            if (offset > 0)
            {
                var toAdd = Mathf.Min(offset, resourceCapacity - resourceAmount);
                if (this.def.restrictResourceCap && resourceAmount + toAdd > resourceCapacity)
                {
                    toAdd = resourceCapacity - resourceAmount;
                }
                offset -= toAdd;
                if (this.def.defName == hediffDefnameToCheck)
                {
                    HRFLog.Message("toAdd: " + toAdd);
                    HRFLog.Message("2 offset: " + offset);
                }
                resourceAmount += toAdd;
            }
            else if (offset < 0)
            {
                var toSubtract = -Mathf.Min(-offset, resourceAmount);
                offset -= toSubtract;
                if (this.def.defName == hediffDefnameToCheck)
                {
                    HRFLog.Message("toSubtract: " + toSubtract);
                    HRFLog.Message("2 offset: " + offset);
                }
                resourceAmount += toSubtract;
            }
            if (this.def.defName == hediffDefnameToCheck)
            {
                HRFLog.Message("this.resourceAmount: " + this.resourceAmount);
                HRFLog.Message("END");
            }

            if (source is null || source.canRefillStorage)
            {
                while (offset > 0)
                {
                    bool changed = false;
                    foreach (var storage in storages)
                    {
                        if (storage.Item2.hediff == this.def)
                        {
                            var toAdd = Mathf.Min(offset, storage.Item3.ResourceCapacity - storage.Item3.ResourceAmount);
                            if (toAdd > 0)
                            {
                                changed = true;
                                storage.Item3.ResourceAmount += toAdd;
                                offset -= toAdd;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (!changed)
                    {
                        break;
                    }
                }
            }

            while (offset < 0)
            {
                if (this.def.defName == hediffDefnameToCheck)
                    HRFLog.Message("WHILE: offset: " + offset);

                bool changed = false;
                foreach (var storage in storages)
                {
                    if (this.def.defName == hediffDefnameToCheck)
                        HRFLog.Message("STORAGE: " + storage.Item2.hediff);

                    if (storage.Item2.hediff == this.def)
                    {
                        var toSubtract = -Mathf.Min(-offset, storage.Item3.ResourceAmount);
                        if (this.def.defName == hediffDefnameToCheck)
                            HRFLog.Message("toSubtract: " + toSubtract);

                        if (toSubtract < 0)
                        {
                            changed = true;
                            if (this.def.defName == hediffDefnameToCheck)
                            {
                                HRFLog.Message("storage.Item3: " + storage.Item3.parent + " - " + storage.Item3.resourceProperties.hediff);
                                HRFLog.Message("1 storage.Item3.ResourceAmount: " + storage.Item3.ResourceAmount);
                                HRFLog.Message("1 offset: " + offset);
                            }

                            storage.Item3.ResourceAmount += toSubtract;
                            offset -= toSubtract;
                            if (this.def.defName == hediffDefnameToCheck)
                            {
                                HRFLog.Message("2 storage.Item3.ResourceAmount: " + storage.Item3.ResourceAmount);
                                HRFLog.Message("2 offset: " + offset);
                            }
                        }
                    }
                }
                if (!changed)
                {
                    break;
                }
            }

            var storagesToDestroy = new List<CompAdjustHediffs>();
            foreach (var storage in storages)
            {
                if (this.def.defName == hediffDefnameToCheck)
                    HRFLog.Message(storage.Item2.hediff + " - storage.Item3.ResourceAmount: " + storage.Item3.ResourceAmount);

                if (storage.Item2.destroyWhenEmpty && storage.Item3.ResourceAmount <= 0 ||
                    storage.Item2.destroyWhenFull && storage.Item3.ResourceAmount >= storage.Item3.ResourceCapacity)
                {
                    storagesToDestroy.Add(storage.Item1);
                }
            }
            foreach (var comp in storagesToDestroy)
            {
                comp.parent.Destroy();
            }

            var storagesToDrop = new List<CompAdjustHediffs>();
            foreach (var storage in storages)
            {
                if (storage.Item2.dropWhenEmpty && storage.Item3.ResourceAmount <= 0 ||
                    storage.Item2.dropWhenFull && storage.Item3.ResourceAmount >= storage.Item3.ResourceCapacity)
                {
                    storagesToDrop.Add(storage.Item1);
                }
            }

            foreach (var comp in storagesToDrop)
            {
                comp.Drop();
            }

            var storagesToUnforbid = new List<CompAdjustHediffs>();
            foreach (var storage in storages)
            {
                if (storage.Item2.unforbidWhenEmpty && storage.Item3.ResourceAmount <= 0 ||
                    storage.Item2.unforbidWhenFull && storage.Item3.ResourceAmount >= storage.Item3.ResourceCapacity)
                {
                    storagesToUnforbid.Add(storage.Item1);
                }
            }

            foreach (var comp in storagesToUnforbid)
            {
                comp.parent.SetForbidden(false);
            }

            if (resourceAmount <= 0 && ResourceFromStorages <= 0 && !this.def.keepWhenEmpty)
            {
                this.pawn.health.RemoveHediff(this);
            }
            else
            {
                UpdateResourceData();
            }
            Gizmo_ResourceStatus.updateNow = true;
        }

        public void UpdateResourceData()
        {
            if (this.def.isResource)
            {
                if (this.def.useAbsoluteSeverity)
                {
                    this.Severity = ResourceAmount / ResourceCapacity;
                }
                else
                {
                    this.Severity = ResourceAmount;
                }
            }
        }

        public override float Severity
        {
            get
            {
                return base.Severity;
            }
            set
            {
                base.Severity = value;
            }
        }

        public bool CanGainResource => Find.TickManager.TicksGame > this.delayTicks;
        public float ResourceCapacity => this.def.maxResourceCapacity + Utils.GetHediffResourceCapacityGainFor(this.pawn, def, out _) + GetHediffResourceCapacityGainFromAmplifiers(out _);

        public CompAbilities compAbilities;
        private void PreInit()
        {
            compAbilities = pawn?.GetComp<CompAbilities>();
            if (this.amplifiers is null)
            {
                this.amplifiers = new List<Thing>();
            }
            if (this.savedSkillRecordsByStages is null)
            {
                this.savedSkillRecordsByStages = new Dictionary<int, SavedSkillRecordCollection>();
            }
            if (this.grantedStaticAbilities is null)
            {
                this.grantedStaticAbilities = new List<AbilityDef>();
            }
            if (this.grantedRandomAbilities is null)
            {
                this.grantedRandomAbilities = new List<AbilityDef>();
            }
            this.Register();
        }
        public bool CanGainCapacity(float newCapacity)
        {
            return ResourceCapacity > newCapacity || ResourceCapacity > 0;
        }

        public void AddDelay(int newDelayTicks)
        {
            this.delayTicks = Find.TickManager.TicksGame + newDelayTicks;
        }
        public bool CanHaveDelay(int newDelayTicks)
        {
            if (Find.TickManager.TicksGame > delayTicks || newDelayTicks > Find.TickManager.TicksGame - delayTicks)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanUse(UseProps useProps, out string failReason)
        {
            failReason = "";
            if (useProps.resourceOnComplete != -1f && this.ResourceAmount < -useProps.resourceOnComplete)
            {
                failReason = "ART.ConsumesOnComplete".Translate(-useProps.resourceOnComplete, this.def.label);
                return false;
            }
            else if (useProps.resourcePerSecond != -1 && this.ResourceAmount < -useProps.resourcePerSecond)
            {
                failReason = "ART.ConsumesPerSecond".Translate(-useProps.resourcePerSecond, this.def.label);
                return false;
            }
            return true;
        }

        public Dictionary<Thing, IAdjustResouceInArea> cachedAmplifiers = new Dictionary<Thing, IAdjustResouceInArea>();
        public float GetHediffResourceCapacityGainFromAmplifiers(out StringBuilder explanation)
        {
            explanation = new StringBuilder();
            float num = 0;
            foreach (var compAmplifier in Amplifiers)
            {
                var gain = compAmplifier.GetResourceCapacityGainFor(this.def);
                explanation.AppendLine("ART.CapacityAmplifier".Translate(compAmplifier.Parent, gain));
                num += gain;
            }
            return num;
        }
        public IAdjustResouceInArea GetCompAmplifierFor(Thing thing)
        {
            if (!cachedAmplifiers.TryGetValue(thing, out IAdjustResouceInArea comp))
            {
                comp = thing.TryGetComp<CompAdjustHediffsArea>();
                cachedAmplifiers[thing] = comp;
            }
            return comp;
        }
        public IEnumerable<IAdjustResouceInArea> Amplifiers
        {
            get
            {
                for (int num = amplifiers.Count - 1; num >= 0; num--)
                {
                    var amplifier = amplifiers[num];
                    if (amplifier is null || amplifier.Destroyed)
                    {
                        amplifiers.RemoveAt(num);
                    }
                    else
                    {
                        var comp = GetCompAmplifierFor(amplifier);
                        if (comp != null && comp.InRadiusFor(this.pawn.Position, this.def) && HRFManager.Instance.resourceAdjusters.Contains(comp))
                        {
                            yield return comp;
                        }
                        else
                        {
                            amplifiers.RemoveAt(num);
                        }
                    }
                }
            }
        }

        public void TryAddAmplifier(IAdjustResouceInArea comp)
        {
            if (!amplifiers.Contains(comp.Parent))
            {
                amplifiers.Add(comp.Parent);
                cachedAmplifiers[comp.Parent] = comp;
            }
        }

        public IEnumerable<IAdjustResouceInArea> GetAmplifiersFor(HediffResourceDef hediffResourceDef)
        {
            foreach (var amplifier in Amplifiers)
            {
                foreach (var option in amplifier.ResourceSettings)
                {
                    if (option.hediff == hediffResourceDef)
                    {
                        yield return amplifier;
                    }
                }
            }
        }
        public override string Label
        {
            get 
            {
                var label = base.Label;
                if (!def.hideResourceAmount)
                {
                    label += ": " + this.resourceAmount.ToStringDecimalIfSmall() + "/" + this.ResourceCapacity.ToStringDecimalIfSmall();
                    if (StoragesTotalCapacity > 0)
                    {
                        label += " (" + ResourceFromStorages.ToStringDecimalIfSmall() + "/" + StoragesTotalCapacity.ToStringDecimalIfSmall() + " " + "ART.Stored".Translate() + ")";
                    }
                }
                if (this.def.lifetimeTicks != -1)
                {
                    label += " (" + Mathf.CeilToInt((this.def.lifetimeTicks - this.duration)).ToStringTicksToPeriod() + ")";
                }
                if (this.CurStage is HediffStageResource hediffStageResource && hediffStageResource.effectWhenDowned != null && hediffStageResource.effectWhenDowned.ticksBetweenActivations > 0)
                {
                    if (HRFManager.Instance.pawnDownedStates.TryGetValue(pawn, out var state))
                    {
                        if (state.lastDownedEffectTicks.TryGetValue(this.def, out var value))
                        {
                            var enabledInTick = value + hediffStageResource.effectWhenDowned.ticksBetweenActivations;
                            if (enabledInTick > Find.TickManager.TicksGame)
                            {
                                label += " (" + "ART.WillBeActiveIn".Translate((enabledInTick - Find.TickManager.TicksGame).ToStringTicksToPeriod()) + ")";
                            }
                        }
                    }
                }
                return label;
            }
        }

        public override string TipStringExtra
        {
            get
            {
                var baseText = base.TipStringExtra;
                if (this.def.isResource)
                {
                    var allCapacityAdjusters = "ART.NativeCapacity".Translate(this.def.maxResourceCapacity);
                    Utils.GetHediffResourceCapacityGainFor(this.pawn, def, out var sbExplanation);
                    var explanation = sbExplanation.ToString().TrimEndNewlines();
                    if (!explanation.NullOrEmpty())
                    {
                        allCapacityAdjusters += "\n" + explanation;
                    }
                    GetHediffResourceCapacityGainFromAmplifiers(out sbExplanation);
                    explanation = sbExplanation.ToString().TrimEndNewlines();
                    if (!explanation.NullOrEmpty())
                    {
                        allCapacityAdjusters += "\n" + explanation;
                    }
                    return baseText + allCapacityAdjusters;
                }
                return baseText;
            }
        }

        public override bool ShouldRemove
        {
            get
            {
                if (this.def.lifetimeTicks != -1 && duration > this.def.lifetimeTicks)
                {
                    return true;
                }
                if (SourceOnlyAmplifiers())
                {
                    return true;
                }
                if (this.def.keepWhenEmpty)
                {
                    if (comps != null)
                    {
                        for (int i = 0; i < comps.Count; i++)
                        {
                            if (comps[i].CompShouldRemove)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                var value = base.ShouldRemove;
                if (value)
                {
                    Log.Message("Removing: " + this + " this.ResourceAmount: " + this.ResourceAmount + " - this.Severity: " + this.Severity);
                }
                return value;
            }
        }
        
        public bool SourceOnlyAmplifiers()
        {
            var amplifiers = Amplifiers;
            if (!this.def.keepWhenEmpty && amplifiers.Any())
            {
                foreach (var amplifier in amplifiers)
                {
                    var comp = GetCompAmplifierFor(amplifier.Parent);
                    if (comp != null && !comp.InRadiusFor(this.pawn.Position, this.def))
                    {
                        var option = comp.GetFirstResourcePropertiesFor(this.def);
                        if (option.removeOutsideArea)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public float TotalResourceGainAmount()
        {
            float num = 0;

            var comps = Utils.GetAllAdjustResources(this.pawn);
            foreach (var comp in comps)
            {
                var resourceSettings = comp.ResourceSettings;
                if (resourceSettings != null)
                {
                    foreach (var option in resourceSettings)
                    {
                        if (option.hediff == def)
                        {
                            var num2 = option.GetResourceGain(comp);
                            num += num2;
                        }
                    }
                }
            }

            var hediffCompResourcePerDay = this.TryGetComp<HediffComp_ResourcePerSecond>();
            if (hediffCompResourcePerDay != null)
            {
                num += hediffCompResourcePerDay.Props.resourcePerSecond;
            }
            return num;
        }
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if (this.CurStage is HediffStageResource hediffStageResource)
            {
                if (hediffStageResource.resourceAdjustPerDamages != null)
                {
                    foreach (var value in hediffStageResource.resourceAdjustPerDamages)
                    {
                        if (value.damageDef is null || dinfo.Def != null && value.damageDef == dinfo.Def)
                        {
                            var resourceToGain = value.GetResourceGain(totalDamageDealt);
                            var hediff = value.hediff != null ? this.pawn.health.hediffSet.GetFirstHediffOfDef(value.hediff) as HediffResource : this;
                            if (hediff != null)
                            {
                                hediff.ChangeResourceAmount(resourceToGain);
                            }
                        }
                    }
                }
            }
        }
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            PreInit();
            this.resourceAmount = this.def.initialResourceAmount;
            UpdateResourceData();
            this.duration = 0;
            if (this.def.sendLetterWhenGained && this.pawn.Faction == Faction.OfPlayerSilentFail)
            {
                Find.LetterStack.ReceiveLetter(this.def.letterTitleKey.Translate(this.pawn.Named("PAWN"), this.def.Named("RESOURCE")),
                    this.def.letterMessageKey.Translate(this.pawn.Named("PAWN"), this.def.Named("RESOURCE")), this.def.letterType, this.pawn);
            }
            var hediffStageResource = this.CurStageResource;
            if (hediffStageResource != null)
            {
                if (hediffStageResource.healingProperties != null && hediffStageResource.healingProperties.healOnApply)
                {
                    DoHeal(hediffStageResource.healingProperties);
                }
                if (hediffStageResource.tendingProperties != null && hediffStageResource.tendingProperties.tendOnApply)
                {
                    DoTend(hediffStageResource.tendingProperties);
                }
            }
            HRFLog.Message("PostAdd");
            OnStageSwitch(hediffStageResource);
            this.Register();
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            Notify_Removed();
            this.Deregister();
            var comps = Utils.GetAllAdjustResources(this.pawn);
            foreach (var comp in comps)
            {
                var resourceSettings = comp.ResourceSettings;
                if (resourceSettings != null)
                {
                    foreach (var option in resourceSettings)
                    {
                        if (option.dropIfHediffMissing && option.hediff == def)
                        {
                            comp.Drop();
                        }
                    }
                }
            }
        }
        public override void Tick()
        {
            base.Tick();
            this.duration++;
            if (this.pawn.IsHashIntervalTick(30))
            {
                UpdateResourceData();
                if (ResourceCapacity < 0)
                {
                    Utils.TryDropExcessHediffGears(this.pawn);
                }
            }

            var hediffStageResource = this.CurStage as HediffStageResource;
            if (this.previousStageIndex != this.CurStageIndex)
            {
                OnStageSwitch(hediffStageResource);
            }

            if (hediffStageResource != null)
            {
                if (hediffStageResource.needAdjustProperties != null && this.pawn.IsHashIntervalTick(hediffStageResource.needAdjustProperties.tickRate))
                {
                    foreach (var needToAdjust in hediffStageResource.needAdjustProperties.needsToAdjust)
                    {
                        var need = this.pawn.needs.TryGetNeed(needToAdjust.need);
                        if (need != null)
                        {
                            need.CurLevelPercentage += needToAdjust.adjustValue;
                        }
                    }
                }
                if (hediffStageResource.healingProperties != null && Find.TickManager.TicksGame >= lastHealingEffectTick + hediffStageResource.healingProperties.ticksPerEffect)
                {
                    DoHeal(hediffStageResource.healingProperties);
                }
                if (hediffStageResource.tendingProperties != null && Find.TickManager.TicksGame >= lastTendingEffectTick + hediffStageResource.tendingProperties.ticksPerEffect)
                {
                    DoTend(hediffStageResource.tendingProperties);
                }
                if (hediffStageResource.damageAuraProperties != null && Find.TickManager.TicksGame >= lastDamagingEffectTick + hediffStageResource.damageAuraProperties.ticksPerEffect)
                {
                    DoDamage(hediffStageResource.damageAuraProperties);
                }
            }

            if (stageIndexToActivate != -1 && Find.TickManager.TicksGame >= stageSwitchTickToActivate)
            {
                SwitchToStage(stageIndexToActivate);
            }
        }

        private void OnStageSwitch(HediffStageResource hediffStageResource)
        {
            HRFLog.Message(pawn + " switching stage " + this.CurStageIndex + " - " + this.previousStageIndex);
            var previousStage = this.previousStageIndex > -1 ? def.stages[this.previousStageIndex] as HediffStageResource : null;
            if (previousStage != null)
            {
                if (previousStage.skillAdjustProperties != null)
                {
                    foreach (var skillAdjust in previousStage.skillAdjustProperties)
                    {
                        RemoveSkillAdjust(this.previousStageIndex, skillAdjust);
                    }
                }
            }

            this.previousStageIndex = this.CurStageIndex;
            if (hediffStageResource != null && hediffStageResource.skillAdjustProperties != null)
            {
                foreach (var skillAdjust in hediffStageResource.skillAdjustProperties)
                {
                    AddSkillAdjust(this.CurStageIndex, skillAdjust);
                }
            }

            if (this.compAbilities != null)
            {
                if (hediffStageResource != null)
                {
                    if (this.def.randomAbilitiesPool != null)
                    {
                        var amount = hediffStageResource.randomAbilitiesAmountToGain.RandomInRange;
                        var abilityCandidates = this.def.randomAbilitiesPool.Where(x => !this.compAbilities.HasAbility(x)).Take(amount);
                        if (!def.retainRandomLearnedAbilities)
                        {
                            var abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedRandomAbilities.Contains(x.def));
                            foreach (var ability in abilitiesToRemove)
                            {
                                HRFLog.Message(pawn + " - removing random ability: " + ability);
                                this.compAbilities.LearnedAbilities.Remove(ability);
                            }
                        }

                        foreach (var ability in abilityCandidates)
                        {
                            HRFLog.Message(pawn + " - gaining random ability: " + ability);
                            this.compAbilities.GiveAbility(ability);
                            this.grantedRandomAbilities.Add(ability);
                        }
                    }
                    else
                    {
                        var abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedRandomAbilities.Contains(x.def));
                        foreach (var ability in abilitiesToRemove)
                        {
                            HRFLog.Message(pawn + " - removing random ability: " + ability);
                            this.compAbilities.LearnedAbilities.Remove(ability);
                        }
                    }

                    if (hediffStageResource.staticAbilitiesToGain != null)
                    {
                        var abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedStaticAbilities.Contains(x.def)
                            && !hediffStageResource.staticAbilitiesToGain.Contains(x.def));
                        foreach (var ability in abilitiesToRemove)
                        {
                            HRFLog.Message(pawn + " - removing static ability: " + ability);
                            this.compAbilities.LearnedAbilities.Remove(ability);
                        }

                        var abilityCandidates = hediffStageResource.staticAbilitiesToGain.Where(x => !this.compAbilities.HasAbility(x));
                        foreach (var ability in abilityCandidates)
                        {
                            HRFLog.Message(pawn + " - gaining static ability: " + ability);
                            this.compAbilities.GiveAbility(ability);
                            this.grantedStaticAbilities.Add(ability);
                        }
                    }
                    else
                    {
                        var abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedStaticAbilities.Contains(x.def));
                        foreach (var ability in abilitiesToRemove)
                        {
                            HRFLog.Message(pawn + " - removing static ability: " + ability);
                            this.compAbilities.LearnedAbilities.Remove(ability);
                        }
                    }
                }
                else
                {
                    var abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedRandomAbilities.Contains(x.def));
                    foreach (var ability in abilitiesToRemove)
                    {
                        HRFLog.Message(pawn + " - removing random ability: " + ability);
                        this.compAbilities.LearnedAbilities.Remove(ability);
                    }
                    abilitiesToRemove = this.compAbilities.LearnedAbilities.Where(x => this.grantedStaticAbilities.Contains(x.def));
                    foreach (var ability in abilitiesToRemove)
                    {
                        HRFLog.Message(pawn + " - removing static ability: " + ability);
                        this.compAbilities.LearnedAbilities.Remove(ability);
                    }

                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (this.def.stages != null)
            {
                var toggleableStages = this.def.stages.OfType<HediffStageResource>().Where(x => x.togglingProperties != null);
                if (toggleableStages.Any())
                {
                    yield return new Command_SwitchHediffStageResource(this)
                    {
                        defaultLabel = this.def.label + " - " + this.CurStage.label,
                        icon = GetIcon(),
                        action = delegate
                        {
                            var options = new List<FloatMenuOption>();
                            var otherStages = toggleableStages.Where(x => x != this.CurStage).ToList();
                            foreach (var otherStage in otherStages)
                            {
                                options.Add(new FloatMenuOption(this.def.label + " - " + otherStage.label, delegate
                                {
                                    lastStageSwitchTick = Find.TickManager.TicksGame;
                                    stageSwitchTickToActivate = Find.TickManager.TicksGame + otherStage.togglingProperties.changeTime;
                                    curChangeTime = otherStage.togglingProperties.changeTime;
                                    curCooldownPeriod = otherStage.togglingProperties.cooldownTime;
                                    stageIndexToActivate = this.def.stages.IndexOf(otherStage);
                                }));
                            }
                            Find.WindowStack.Add(new FloatMenu(options));
                        },
                        disabled = !IsActive()
                    };
                    Texture2D GetIcon()
                    {
                        if (this.CurStage is HediffStageResource stageResource && stageResource.togglingProperties.graphicData != null)
                        {
                            return ContentFinder<Texture2D>.Get(stageResource.togglingProperties.graphicData.texPath);
                        }
                        return ContentFinder<Texture2D>.Get(this.def.fallbackTogglingGraphicData.texPath);
                    }

                    bool IsActive()
                    {
                        if (this.lastStageActivatedTick > 0)
                        {
                            var cooldownTicksRemaining = Find.TickManager.TicksGame - this.lastStageActivatedTick;
                            if (cooldownTicksRemaining < this.curCooldownPeriod)
                            {
                                return false;
                            }
                        }
                        if (this.lastStageSwitchTick > 0)
                        {
                            var cooldownTicksRemaining = Find.TickManager.TicksGame - this.lastStageSwitchTick;
                            if (cooldownTicksRemaining < this.curChangeTime)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
        }

        private void SwitchToStage(int stageIndex)
        {
            var stage = this.def.stages[stageIndex] as HediffStageResource;
            if (this.def.useAbsoluteSeverity)
            {
                this.SetResourceAmount(this.ResourceCapacity * stage.minSeverity);
                this.Severity = ResourceAmount;
            }
            else
            {
                this.SetResourceAmount(stage.minSeverity);
                this.Severity = this.ResourceAmount;
            }
            if (stage.togglingProperties.soundOnToggle != null)
            {
                stage.togglingProperties.soundOnToggle.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
            }
            this.lastStageActivatedTick = Find.TickManager.TicksGame;
            stageIndexToActivate = -1;
        }
        private void DoDamage(DamageAuraProperties damagingProperties)
        {
            lastDamagingEffectTick = Find.TickManager.TicksGame;
            foreach (var victim in Utils.GetPawnsAround(pawn, damagingProperties.effectRadius))
            {
                if (CanDamage(victim, damagingProperties))
                {
                    var damageDef = damagingProperties.damageFromEquippedWeapon 
                        ? this.pawn.equipment.PrimaryEq.PrimaryVerb.GetDamageDef() 
                        : damagingProperties.damageDef;
                    var damageInfo = new DamageInfo(damageDef, damagingProperties.damageAmount, 1f, instigator: this.pawn, 
                        weapon: damagingProperties.damageFromEquippedWeapon ? this.pawn.equipment.Primary.def : this.pawn.def);
                    victim.TakeDamage(damageInfo);
                    if (victim.MapHeld != null)
                    {
                        if (damagingProperties.selfDamageMote != null && this.pawn == victim)
                        {
                            MoteMaker.MakeStaticMote(this.pawn.Position, this.pawn.Map, damagingProperties.selfDamageMote);
                        }
                        else if (damagingProperties.otherDamageMote != null && this.pawn != victim)
                        {
                            MoteMaker.MakeStaticMote(victim.PositionHeld, victim.MapHeld, damagingProperties.otherDamageMote);
                        }

                        if (damagingProperties.soundOnEffect != null)
                        {
                            damagingProperties.soundOnEffect.PlayOneShot(new TargetInfo(victim.PositionHeld, victim.MapHeld));
                        }
                    }
                }
            }
        }

        public bool CanApplyOn(GeneralProperties properties, Thing thing)
        {
            if (thing.def.tradeTags.NullOrEmpty() is false)
            {
                if (properties.blacklistTradeTags.NullOrEmpty() is false && properties.blacklistTradeTags.Any(x => thing.def.tradeTags.Contains(x)))
                {
                    return false;
                }
                if (properties.whitelistTradeTags.NullOrEmpty() is false && !properties.whitelistTradeTags.Any(x => thing.def.tradeTags.Contains(x)))
                {
                    return false;
                }
            }
            if (!properties.affectsSelf && this.pawn == thing)
            {
                return false;
            }
            if (!properties.affectsSelf && !properties.worksThroughWalls && !GenSight.LineOfSight(this.pawn.Position, thing.Position, this.pawn.Map))
            {
                return false;
            }
            return true;
        }

        private bool CanDamage(Pawn pawn, DamageAuraProperties damagingProperties)
        {
            if (!CanApplyOn(damagingProperties, pawn))
            {
                return false;
            }
            if (!damagingProperties.affectsSelf)
            {
                bool isAllyOrColonist = pawn.Faction != null && !pawn.HostileTo(this.pawn);
                if (!damagingProperties.affectsAllies && isAllyOrColonist)
                {
                    return false;
                }
                if (!damagingProperties.affectsEnemies && isAllyOrColonist == false)
                {
                    return false;
                }
            }
            return true;
        }
        private void AddSkillAdjust(int stageIndex, SkillAdjustProperties skillAdjust)
        {
            if (pawn.skills != null)
            {
                var skillRecord = pawn.skills.GetSkill(skillAdjust.skill);
                if (skillRecord != null && !skillRecord.TotallyDisabled)
                {
                    if (!savedSkillRecordsByStages.TryGetValue(stageIndex, out var list))
                    {
                        savedSkillRecordsByStages[stageIndex] = list = new SavedSkillRecordCollection();
                    }
                    list.savedSkillRecords.Add(new SavedSkillRecord
                    {
                        def = skillAdjust.skill,
                        levelInt = skillRecord.levelInt,
                        passion = skillRecord.passion
                    });
                    skillRecord.levelInt += skillAdjust.skillLevelOffset;
                    skillRecord.passion = skillAdjust.forcedPassion;
                    if (skillRecord.levelInt > skillAdjust.maxSkillLevel)
                    {
                        skillRecord.levelInt = skillAdjust.maxSkillLevel;
                    }
                    skillRecord.levelInt = Mathf.Clamp(skillRecord.levelInt, 0, 20);
                }
            }
        }

        private void RemoveSkillAdjust(int stageIndex, SkillAdjustProperties skillAdjust)
        {
            var skillRecord = pawn.skills.GetSkill(skillAdjust.skill);
            if (skillRecord != null && !skillRecord.TotallyDisabled)
            {
                if (savedSkillRecordsByStages.ContainsKey(stageIndex))
                {
                    var savedSkillRecord = savedSkillRecordsByStages[stageIndex].savedSkillRecords.FirstOrDefault(x => x.def == skillAdjust.skill);
                    if (savedSkillRecord != null)
                    {
                        savedSkillRecordsByStages[stageIndex].savedSkillRecords.Remove(savedSkillRecord);
                        skillRecord.levelInt = Mathf.Max(skillRecord.levelInt - skillAdjust.skillLevelOffset,
                            savedSkillRecord.levelInt);
                        skillRecord.levelInt = Mathf.Clamp(skillRecord.levelInt, 0, 20);
                        skillRecord.passion = savedSkillRecord.passion;
                    }
                }
            }
        }

        public void DoTend(TendingProperties tendingProperties)
        {
            lastTendingEffectTick = Find.TickManager.TicksGame;
            var hediffs = tendingProperties.affectConditions 
                ? pawn.health.hediffSet.hediffs.Where(x => x.TendableNow()).ToList() 
                : pawn.health.hediffSet.hediffs.Where(x => x is Hediff_Injury && x.TendableNow()).ToList();
            if (hediffs.Any())
            {
                hediffs = hediffs.Take(tendingProperties.tendCount).ToList();
                TendUtility_DoTend_Patch.hardCodedTendData = new TendUtility_DoTend_Patch.HardCodedTendData
                {
                    quality = tendingProperties.tendQuality.RandomInRange,
                    maxQuality = tendingProperties.tendQuality.max
                };
                foreach (var hediff in hediffs)
                {
                    TendUtility.DoTend(null, pawn, null);
                }
                TendUtility_DoTend_Patch.hardCodedTendData = null;
            }
        }

        public void DoHeal(HealingProperties healingProperties)
        {
            lastHealingEffectTick = Find.TickManager.TicksGame;
            float totalSpentPoints = healingProperties.healPoints;
            foreach (var pawn in Utils.GetPawnsAround(this.pawn, healingProperties))
            {
                if (CanHeal(pawn, healingProperties))
                {
                    var hediffs = GetHediffsToHeal(pawn, healingProperties).ToList();
                    if (hediffs.Any())
                    {
                        var hediffsToHeal = healingProperties.hediffsToHeal > 0 ? hediffs.Take(healingProperties.hediffsToHeal).ToList() : hediffs;
                        Utils.HealHediffs(this.pawn, ref totalSpentPoints, hediffsToHeal, healingProperties.pointsOverflow,
                            healingProperties.healPriority, healingProperties.hediffsToHeal > 0, healingProperties.soundOnEffect);
                    }
                }
            }
        }

        private bool CanHeal(Pawn pawn, HealingProperties healingProperties)
        {
            if (!CanApplyOn(healingProperties, pawn))
            {
                return false;
            }
            if (pawn.health?.hediffSet?.hediffs is null)
            {
                return false;
            }
            bool isFlesh = pawn.RaceProps.IsFlesh;
            if (!healingProperties.affectMechanical && isFlesh == false)
            {
                return false;
            }
            if (!healingProperties.affectOrganic && isFlesh)
            {
                return false;
            }
            if (!healingProperties.affectsSelf)
            {
                bool isAllyOrColonist = pawn.Faction != null && !pawn.HostileTo(this.pawn);
                if (!healingProperties.affectsAllies && isAllyOrColonist)
                {
                    return false;
                }
                if (!healingProperties.affectsEnemies && isAllyOrColonist == false)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<Hediff> GetHediffsToHeal(Pawn pawn, HealingProperties healingProperties)
        {
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (healingProperties.affectIllness && !(hediff is Hediff_Injury) && !(hediff is Hediff_MissingPart) 
                    && (hediff.def.PossibleToDevelopImmunityNaturally() && !hediff.FullyImmune() || hediff.def.makesSickThought && hediff.def.tendable))
                {
                    yield return hediff;
                }
                else if (healingProperties.affectInjuries && hediff is Hediff_Injury)
                {
                    yield return hediff;
                }
                else if (healingProperties.affectPermanent && hediff.IsPermanent())
                {
                    yield return hediff;
                }
                else if (healingProperties.affectChronic && hediff.def.chronic)
                {
                    yield return hediff;
                }
            }
        }

        private Vector3 impactAngleVect;

        private int lastAbsorbDamageTick = -9999;
        public void AbsorbedDamage(DamageInfo dinfo)
        {
            SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.pawn.Position, base.pawn.Map));
            impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            Vector3 loc = base.pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            FleckMaker.Static(loc, base.pawn.Map, FleckDefOf.ExplosionFlash, num);
            int num2 = (int)num;
            for (int i = 0; i < num2; i++)
            {
                FleckMaker.ThrowDustPuff(loc, base.pawn.Map, Rand.Range(0.8f, 1.2f));
            }
            lastAbsorbDamageTick = Find.TickManager.TicksGame;
        }

        private Material bubbleMat;

        public Material BubbleMat
        {
            get
            {
                if (bubbleMat is null)
                {
                    var stage = CurStageResource;
                    bubbleMat = MaterialPool.MatFrom(stage.shieldProperties.texPath, ShaderDatabase.Transparent, stage.shieldProperties.shieldColor);
                }
                return bubbleMat;
            }
        }
        public Dictionary<HediffResource, HediffResouceDisable> PostUseDelayTicks => null;
        public Thing Parent => pawn;
        public Pawn PawnHost => pawn;
        public List<ResourceProperties> ResourceSettings => this.CurStageResource?.resourceSettings ?? new List<ResourceProperties>();
        public string DisablePostUse => "";

        private static Dictionary<GraphicData, Material> auraGraphics = new Dictionary<GraphicData, Material>();
        public static Material GetAuraMaterial(GraphicData graphicData)
        {
            if (!auraGraphics.TryGetValue(graphicData, out Material material))
            {
                auraGraphics[graphicData] = material = MaterialPool.MatFrom(graphicData.texPath, graphicData.shaderType?.Shader ?? ShaderDatabase.Mote, graphicData.color);
            }
            return material;
        }
        public virtual void Draw()
        {
            if (this.CurStage is HediffStageResource hediffStageResource)
            {
                if (hediffStageResource.ShieldIsActive(pawn) && this.ResourceAmount > 0)
                {
                    float num = Mathf.Lerp(1.2f, 1.55f, this.def.lifetimeTicks != -1 ? (this.def.lifetimeTicks - duration) / this.def.lifetimeTicks : 1);
                    Vector3 drawPos = base.pawn.Drawer.DrawPos;
                    drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                    int num2 = Find.TickManager.TicksGame - lastAbsorbDamageTick;
                    if (num2 < 8)
                    {
                        float num3 = (float)(8 - num2) / 8f * 0.05f;
                        drawPos += impactAngleVect * num3;
                        num -= num3;
                    }
                    float angle = Rand.Range(0, 360);
                    Vector3 s = new Vector3(num, 1f, num);
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
                }
                if (hediffStageResource.damageAuraProperties?.auraGraphic != null)
                {
                    Vector3 drawPos = base.pawn.Drawer.DrawPos;
                    drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
                    Vector3 s = new Vector3(hediffStageResource.damageAuraProperties.auraGraphic.drawSize.x, 1f, hediffStageResource.damageAuraProperties.auraGraphic.drawSize.y);
                    Matrix4x4 matrix = default(Matrix4x4);
                    matrix.SetTRS(drawPos, Quaternion.identity, s);
                    var auraMaterial = GetAuraMaterial(hediffStageResource.damageAuraProperties.auraGraphic);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, auraMaterial, 0);
                }
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref resourceAmount, "resourceAmount");
            Scribe_Values.Look(ref duration, "duration");
            Scribe_Values.Look(ref delayTicks, "delayTicks");
            Scribe_Values.Look(ref lastHealingEffectTick, "lastHealingEffectTick");
            Scribe_Values.Look(ref lastTendingEffectTick, "lastTendingEffectTick");
            Scribe_Values.Look(ref lastDamagingEffectTick, "lastDamagingEffectTick");

            Scribe_Values.Look(ref stageIndexToActivate, "stageIndexToActivate", -1);
            Scribe_Values.Look(ref lastStageActivatedTick, "lastStageActivatedTick");
            Scribe_Values.Look(ref stageSwitchTickToActivate, "stageSwitchTickToActivate");
            Scribe_Values.Look(ref curCooldownPeriod, "curCooldownPeriod");
            Scribe_Values.Look(ref curChangeTime, "curChangeTime");
            Scribe_Values.Look(ref lastStageSwitchTick, "lastStageSwitchTick");

            Scribe_Values.Look(ref previousStageIndex, "previousStageIndex", -1);
            Scribe_Collections.Look(ref amplifiers, "amplifiers", LookMode.Reference);
            Scribe_Collections.Look(ref savedSkillRecordsByStages, "savedSkillRecordsByStages", LookMode.Value, LookMode.Deep);

            Scribe_Collections.Look(ref grantedStaticAbilities, "grantedStaticAbilities", LookMode.Def);
            Scribe_Collections.Look(ref grantedRandomAbilities, "grantedRandomAbilities", LookMode.Def);
            PreInit();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (var amplifier in amplifiers)
                {
                    cachedAmplifiers[amplifier] = amplifier.TryGetComp<CompAdjustHediffsArea>();
                }
            }
        }

        public void Register()
        {
            HRFManager.Instance.RegisterAdjuster(this);
        }

        public void Deregister()
        {
            HRFManager.Instance.DeregisterAdjuster(this);
        }
        public bool TryGetQuality(out QualityCategory qc)
        {
            qc = QualityCategory.Normal;
            return false;
        }

        public void Drop()
        {
            this.PawnHost.health.RemoveHediff(this);
            Notify_Removed();
        }
        public void Notify_Removed()
        {
            Deregister();
            if (this.PawnHost != null)
            {
                Utils.RemoveExcessHediffResources(this.PawnHost, this);
            }
        }
        public void ResourceTick()
        {
            var pawn = PawnHost;
            if (pawn != null)
            {
                foreach (var resourceProperties in ResourceSettings)
                {
                    resourceProperties.AdjustResource(pawn, this, PostUseDelayTicks);
                }
            }
        }

        public void Update()
        {
        }

        public ThingDef GetStuff()
        {
            return null;
        }

        public bool IsStorageFor(ResourceProperties resourceProperties, out ResourceStorage resourceStorages)
        {
            resourceStorages = null;
            return false;
        }
    }
}
