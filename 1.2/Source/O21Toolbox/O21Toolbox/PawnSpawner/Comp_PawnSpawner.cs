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

        public int spawnMax = -1;

        public int spawnTotal = 0;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                tickToSpawn = Current.Game.tickManager.TicksGame + Props.timer;
                spawnMax = Props.repeatCount.RandomInRange;
            }
        }

        public override void CompTick()
        {
            if (parent.def.plant != null)
            {
                Plant plant = parent as Plant;
                if (plant.HarvestableNow)
                {
                    SpawnThenDeleteOrRepeat(true);
                }
            }
            else
            {
                if (tickToSpawn >= Current.Game.tickManager.TicksGame)
                {
                    SpawnThenDeleteOrRepeat();
                }
            }
        }

        public void SpawnThenDeleteOrRepeat(bool isPlant = false)
        {
            SpawnPawn();
            spawnTotal++;
            if (Props.repeatSpawn && spawnTotal < spawnMax)
            {
                if (isPlant)
                {
                    Plant plant = parent as Plant;
                    plant.Age = 0;
                }
                else
                {
                    tickToSpawn = Current.Game.tickManager.TicksGame + Props.timer;
                }
            }
            else if (Props.deleteWhenDone)
            {
                parent.Destroy(DestroyMode.Vanish);
            }
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

            PawnGenerationRequest request = new PawnGenerationRequest(kind:Props.pawnKind, faction:Faction.OfPlayer, newborn:Props.newborn, forceGenerateNewPawn:true, canGeneratePawnRelations: Props.canGeneratePawnRelations);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            if (!Props.skillSettings.NullOrEmpty())
            {
                if (Props.purgeSkillsBeforeSetting)
                {
                    newThing.skills.skills.ForEach(s => s.Level = 0);
                }
                foreach(SkillLevelSetting skill in Props.skillSettings)
                {
                    newThing.skills.skills.Find(sr => sr.def == skill.skill).Level = skill.level;
                }
            }
            if (Props.purgeTraits)
            {
                if (!newThing.story.traits.allTraits.NullOrEmpty())
                {
                    newThing.story.traits.allTraits.RemoveAll(t => t is Trait);
                }
            }
            if(!Props.enforcedBackstoriesChild.NullOrEmpty())
            {
                newThing.story.childhood = Props.enforcedBackstoriesChild.RandomElement();
            }
            if (!Props.enforcedBackstoriesAdult.NullOrEmpty())
            {
                newThing.story.adulthood = Props.enforcedBackstoriesAdult.RandomElement();
            }
            GenSpawn.Spawn(newThing, parent.Position, parent.Map, WipeMode.Vanish);
        }
    }
}
