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
    public class Comp_SlotLoadable : ThingComp
    {
        private SlotLoadable colorChangingSlot;
        public bool GizmosOnEquip = true;

        public bool isGathering;

        private bool isInitialized;
        public bool IsInitialized => isInitialized;


        private SlotLoadable secondColorChangingSlot;

        private List<SlotLoadable> slots = new List<SlotLoadable>();
        public List<SlotLoadable> Slots => slots;

        public SlotLoadable ColorChangingSlot
        {
            get
            {
                if (colorChangingSlot != null) return colorChangingSlot;
                if (slots != null)
                    colorChangingSlot = slots.FirstOrDefault(x => ((SlotLoadableDef)x.def).doesChangeColor);
                return colorChangingSlot;
            }
        }

        public SlotLoadable SecondColorChangingSlot
        {
            get
            {
                if (secondColorChangingSlot != null) return secondColorChangingSlot;
                if (slots != null)
                    secondColorChangingSlot = slots.FirstOrDefault(x => ((SlotLoadableDef)x.def).doesChangeSecondColor);
                return secondColorChangingSlot;
            }
        }

        public List<SlotLoadableDef> SlotDefs
        {
            get
            {
                var result = new List<SlotLoadableDef>();
                if (slots != null)
                {
                    foreach (var slot in slots)
                    {
                        result.Add(slot.def as SlotLoadableDef);
                    }
                }
                return result;
            }
        }

        public Map GetMap
        {
            get
            {
                var map = parent.Map;
                if (map == null)
                {
                    if (GetPawn != null)
                    {
                        map = GetPawn.Map;
                    }
                }
                return map;
            }
        }

        public CompEquippable GetEquippable => parent.GetComp<CompEquippable>();

        private Pawn GetPawn => GetEquippable.verbTracker.PrimaryVerb.CasterPawn;


        public CompProperties_SlotLoadable Props => (CompProperties_SlotLoadable)props;

        public void Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;
                if (Props?.slots != null)
                {
                    foreach (var slot in Props.slots)
                    {
                        var newSlot = new SlotLoadable(slot, parent);
                        LogUtil.LogMessage("Added Slot");
                        slots.Add(newSlot);
                    }
                }
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();

            if (slots.NullOrEmpty() && Props?.slots != null)
            {
                Initialize();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        private void TryCancel(string reason = "")
        {
            var pawn = GetPawn;
            if (pawn != null)
            {
                if (pawn.CurJob.def == Utility.JobDefOf.O21_GatherSlotItem)
                {
                    pawn.jobs.StopAll();
                }
                isGathering = false;
            }
        }

        private void TryGiveLoadSlotJob(Thing itemToLoad)
        {
            if (GetPawn != null)
            {
                if (!GetPawn.Drafted)
                {
                    isGathering = true;

                    var job = JobMaker.MakeJob(Utility.JobDefOf.O21_GatherSlotItem, itemToLoad);
                    job.count = 1;
                    GetPawn.jobs.TryTakeOrderedJob(job);
                    //GetPawn.jobs.jobQueue.EnqueueFirst(job);
                    //GetPawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
                else
                {
                    Messages.Message(string.Format("{ 0} is drafted.", new object[]
                    {
                        GetPawn.Label
                    }), MessageTypeDefOf.RejectInput);
                }
            }
        }

        public virtual bool TryLoadSlot(Thing thing)
        {
            //LogUtil.LogMessage("TryLoadSlot Called");
            isGathering = false;
            if (slots != null)
            {
                var loadSlot = slots.FirstOrDefault(x => x.IsEmpty() && x.CanLoad(thing.def));
                if (loadSlot == null)
                {
                    loadSlot = slots.FirstOrDefault(y => y.CanLoad(thing.def));
                }
                if (loadSlot != null)
                {
                    if (loadSlot.TryLoadSlot(thing, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ProcessInput(SlotLoadable slot)
        {
            var floatList = new List<FloatMenuOption>();
            if (!isGathering)
            {
                var map = GetMap;
                if (slot.SlotOccupant == null && slot.SlottableTypes is List<ThingDef> loadTypes)
                {
                    if (loadTypes.Count > 0)
                    {
                        foreach (var current in loadTypes)
                        {
                            var thingToLoad = map.listerThings.ThingsOfDef(current).FirstOrDefault(x => map.reservationManager.CanReserve(GetPawn, x));
                            if (thingToLoad != null)
                            {
                                var text = "Load".Translate() + " " + thingToLoad.def.label;
                                floatList.Add(new FloatMenuOption(text, delegate { TryGiveLoadSlotJob(thingToLoad); },  MenuOptionPriority.Default, null, null, 29f, null, null));
                            }
                            else
                            {
                                FloatMenuOption option = new FloatMenuOption(string.Format("{0} unavailable", new object[] { current.label }), delegate { }, MenuOptionPriority.Default);
                                option.Disabled = true;
                                floatList.Add(option);
                            }
                        }
                    }
                    else
                    {
                        FloatMenuOption option = new FloatMenuOption("No load options available.", delegate { }, MenuOptionPriority.Default);
                        option.Disabled = true;
                        floatList.Add(option);
                    }
                }
            }
            if (!slot.IsEmpty())
            {
                var text = string.Format("Unload {0}", new object[] { slot.SlotOccupant.Label });
                floatList.Add(new FloatMenuOption(text, delegate { TryEmptySlot(slot); }, MenuOptionPriority.Default, null, null, 29f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(floatList));
        }

        public virtual void TryEmptySlot(SlotLoadable slot)
        {
            slot.TryEmptySlot();
        }

        public virtual IEnumerable<Gizmo> EquippedGizmos()
        {
            if (!slots.NullOrEmpty() && GetPawn.Faction.IsPlayer)
            {
                if (isGathering)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Designator_Cancel".Translate(),
                        defaultDesc = "Designator_CancelDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel", true),
                        action = delegate { TryCancel(); }
                    };
                }
                foreach (var slot in slots)
                {
                    if (slot.IsEmpty())
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = slot.LabelNoCount,
                            icon = Command.BGTex,
                            defaultDesc = SlotDesc(slot),
                            action = delegate { ProcessInput(slot); }
                        };
                    }
                    else
                    {
                        yield return new Command_Action
                        {
                            defaultLabel = slot.LabelNoCount,
                            icon = slot.SlotIcon(),
                            defaultDesc = SlotDesc(slot),
                            defaultIconColor = slot.SlotColor(),
                            action = delegate { ProcessInput(slot); }
                        };
                    }
                }
            }
        }

        public virtual string SlotDesc(SlotLoadable slot)
        {
            var s = new StringBuilder();
            s.AppendLine(slot.def.description); //TODO
            if (!slot.IsEmpty())
            {
                s.AppendLine();
                s.AppendLine(string.Format("Loaded {0}", new object[] { slot.SlotOccupant.LabelCap }));
                if (((SlotLoadableDef)slot.def).doesChangeColor)
                {
                    s.AppendLine();
                    s.AppendLine("Effects:");
                    s.AppendLine("  " + "Changes Primary Color");
                }
                if (((SlotLoadableDef)slot.def).doesChangeStats)
                {
                    var slotBonus = slot.SlotOccupant.TryGetComp<Comp_SlottedBonus>();
                    if (slotBonus?.Props != null)
                    {
                        if (!slotBonus.Props.statModifiers.NullOrEmpty())
                        {
                            s.AppendLine();
                            s.AppendLine("Stat Modifiers");

                            foreach (var mod in slotBonus.Props.statModifiers)
                            {
                                var v = Utility.SlotLoadableUtility.DetermineSlottableStatAugment(slot.SlotOccupant, mod.stat);
                                var modstring = mod.stat.ValueToString(v, ToStringNumberSense.Offset);
                                //LogUtil.LogMessage("Determined slot stat augment "+v+" and made string "+modstring);
                                s.AppendLine("  " + mod.stat.LabelCap + " " + modstring);
                                //s.AppendLine("\t" + mod.stat.LabelCap + " " + mod.ToStringAsOffset);
                            }
                            /*
                            //LogUtil.LogMessage("fix this to display statModifiers");
                            List<StatModifier> statMods = slot.SlotOccupant.def.statBases.FindAll(
                                (StatModifier z) => z.stat.category == StatCategoryDefOf.Weapon ||
                                                    z.stat.category == StatCategoryDefOf.EquippedStatOffsets);
                            if (statMods.Count > 0)
                            {
                                s.AppendLine();
                                s.AppendLine("StatModifiers".Translate() + ":");
                                foreach (StatModifier mod in statMods)
                                {
                                    s.AppendLine("\t" + mod.stat.LabelCap + " " + mod.ToStringAsOffset);
                                }
                            }
                            */
                        }
                        var damageDef = slotBonus.Props.damageDef;
                        if (damageDef != null)
                        {
                            s.AppendLine();
                            s.AppendLine(string.Format("Damage Type: {0}", new object[] { damageDef.LabelCap }));
                        }
                    }
                }
            }
            return s.ToString();
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref isInitialized, "isInitialized", false);
            Scribe_Values.Look(ref isGathering, "isGathering", false);
            Scribe_Collections.Look(ref slots, "slots", LookMode.Deep);
            base.PostExposeData();
            if (slots == null)
                slots = new List<SlotLoadable>();
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                //Scribe.writingForDebug = false;
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                //Scribe.writingForDebug = true;
            }
        }
    }
}
