using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Tracks stored energy for use in case of death.
    /// </summary>
    public class Comp_EnergyTracker : ThingComp
    {
        /// <summary>
        /// Last known energy amount.
        /// </summary>
        public float energy;

        /// <summary>
        /// Convenience access.
        /// </summary>
        Pawn pawn;

        Need_Energy energyNeed;

        public CompProperties_EnergyTracker EnergyProperties
        {
            get
            {
                return props as CompProperties_EnergyTracker;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            pawn = parent as Pawn;

            if(pawn != null)
            {
                energyNeed = pawn.needs.TryGetNeed<Need_Energy>();
            }
        }

        public override void CompTick()
        {
            if (energyNeed != null)
                energy = energyNeed.CurLevel;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref energy, "energy");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            // Debug Stuff
            if(Prefs.DevMode && DebugSettings.godMode && energyNeed != null)
            {
                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = "DEBUG: Set Energy to 100%";
                    gizmo.action = () => energyNeed.CurLevelPercentage = 1.0f;
                    yield return gizmo;
                }

                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = "DEBUG: Set Energy to 50%";
                    gizmo.action = () => energyNeed.CurLevelPercentage = 0.5f;
                    yield return gizmo;
                }

                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = "DEBUG: Set Energy to 20%";
                    gizmo.action = () => energyNeed.CurLevelPercentage = 0.2f;
                    yield return gizmo;
                }
            }

            // Self detonation button.
            if (EnergyProperties.canSelfDestruct)
            {
                {
                    Command_Action gizmo = new Command_Action();
                    gizmo.defaultLabel = EnergyProperties.selfDestructLabel.Translate();
                    gizmo.defaultDesc = EnergyProperties.selfDestructDesc.Translate();
                    gizmo.icon = ContentFinder<Texture2D>.Get(EnergyProperties.selfDestructIcon, true);
                    gizmo.action = delegate ()
                    {
                        Log.Warning("Self Destruct not Implemented Yet.");
                    };
                    yield return gizmo;
                };
            }

            yield break;
        }
    }
}
