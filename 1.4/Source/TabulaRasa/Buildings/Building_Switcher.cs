using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TabulaRasa
{
    public class Building_Switcher : Building
    {
        public DefModExt_Switcher modExt;

        public DefModExt_Switcher ModExt
        {
            get
            {
                if(modExt == null)
                {
                    modExt = def.GetModExtension<DefModExt_Switcher>();
                }
                return modExt;
            }
        }

        public CompPowerTrader powerComp;

        public bool Active => powerComp == null || powerComp.PowerOn;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = GetComp<CompPowerTrader>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach(Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            {
                Command_Action switcherGizmo = new Command_Action
                {
                    action = delegate () { SwitchBuilding(); },
                    defaultDesc = ModExt.description,
                    defaultLabel = ModExt.label,
                    activateSound = SoundDef.Named("Click"),
                    disabled = !Active
                };
                if(ModExt.icon != null)
                {
                    switcherGizmo.icon = ContentFinder<Texture2D>.Get(ModExt.icon);
                }
                yield return switcherGizmo;
            }
            yield break;
        }

        public void SwitchBuilding()
        {
            if (Active)
            {
                Thing thing = ThingMaker.MakeThing(ModExt.buildingDef, Stuff);
                thing.SetFactionDirect(Faction);
                thing.HitPoints = HitPoints;
                this.DestroyedOrNull();
                if(ModExt.activateSound != null)
                {
                    SoundStarter.PlayOneShot(ModExt.activateSound, SoundInfo.InMap(this));
                }
                GenSpawn.Spawn(thing, Position, Map, Rotation);
            }
        }
    }
}
