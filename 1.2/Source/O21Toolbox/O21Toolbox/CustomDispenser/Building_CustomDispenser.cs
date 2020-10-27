using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;
using Verse.Noise;

namespace O21Toolbox.CustomDispenser
{
    public class Building_CustomDispenser : Building_NutrientPasteDispenser
    {
        #region Values
        public new CompPowerTrader powerComp;

        public DefModExt_CustomDispenser dispenserProps;

        public int dispensingTicks = 0;

        public ThingDef currentThing;
        #endregion

        #region Saved Data
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look<ThingDef>(ref currentThing, "currentThing");
            Scribe_Values.Look<int>(ref dispensingTicks, "dispensingTicks", 0);
        }
        #endregion

        #region Getters
        public ThingDef DispensableThing
        {
            get
            {
                if(currentThing != null)
                {
                    return currentThing;
                }
                else
                {
                    DispensableThing = dispenserProps.thingDefs.FirstOrDefault();
                    return DispensableThing;
                }
            }
            set
            {
                currentThing = value;
            }
        }

        public new bool CanDispenseNow
        {
            get
            {
                return (!dispenserProps.requiresPower || powerComp.PowerOn);
            }
        }
        #endregion

        #region Functions

        public override void Tick()
        {
            base.Tick();
        }
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
            this.dispenserProps = this.def.GetModExtension<DefModExt_CustomDispenser>();
        }

        public Thing TryDispenseThing(Pawn eater, Pawn getter)
        {
            if (getter == null)
            {
                getter = eater;
            }
            if (!CanDispenseNow)
            {
                return null;
            }
            if (DispensableThing == null)
            {
                return null;
            }
            dispenserProps.dispenseSound.PlayOneShot(new TargetInfo(Position, Map, false));
            return ThingMaker.MakeThing(DispensableThing);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if(Faction == Faction.OfPlayer)
            {
                if(dispenserProps.thingDefs.Count >= 1)
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "Select Item",
                        defaultDesc = "Choose which item is produced from the dispenser.",
                        icon = DispensableThing.uiIcon,
                        action = () => Find.WindowStack.Add(new Utility.Popup_ListSelector("Dispenser Item:", DispensableThing, dispenserProps.thingDefs, newThing => DispensableThing = newThing))
                    };
                }
            }
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(base.GetInspectString());
            stringBuilder.AppendLine("Dispenses: " + DispensableThing.LabelCap);
            return stringBuilder.ToString().Trim();
        }
        #endregion
    }
}
