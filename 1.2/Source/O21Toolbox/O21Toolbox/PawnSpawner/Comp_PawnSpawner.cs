using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Drones;

namespace O21Toolbox
{
    public class Comp_PawnSpawner : ThingComp
    {
        public CompProperties_PawnSpawner Props => props as CompProperties_PawnSpawner;

        public int tickToSpawn = -1;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                tickToSpawn = Current.Game.tickManager.TicksGame + Props.timer;
            }
        }

        public override void CompTick()
        {
            if (parent.def.plant != null)
            {
                Plant plant = parent as Plant;
                if (plant.HarvestableNow)
                {
                    SpawnPawn();
                }
            }
            else
            {
                if (tickToSpawn >= Current.Game.tickManager.TicksGame)
                {
                    SpawnPawn();
                }
            }
            parent.Destroy(DestroyMode.Vanish);
        }
        
        public void SpawnPawn()
        {
            PawnKindDef pawnKind;
            if (Props.pawnKind == null)
            {
                pawnKind = Props.pawnKinds.RandomElement();
            }
            else
            {
                pawnKind = Props.pawnKind;
            }

            PawnGenerationRequest request = new PawnGenerationRequest(kind:Props.pawnKind, faction:Faction.OfPlayer, newborn:Props.newborn, forceGenerateNewPawn:true, canGeneratePawnRelations: false);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            if (!Props.skillSettings.NullOrEmpty())
            {
                foreach(SkillLevelSetting skill in Props.skillSettings)
                {
                    newThing.skills.skills.Find(sr => sr.def == skill.skill).Level = skill.level;
                }
            }
            GenSpawn.Spawn(newThing, parent.Position, parent.Map, WipeMode.Vanish);
        }
    }
}
