using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
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
                if (Props.timer > 0)
                {
                    tickToSpawn = Props.timer;
                }
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
                if (tickToSpawn <= 0)
                {
                    SpawnThenDeleteOrRepeat();
                }
            }
            tickToSpawn--;
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
                    tickToSpawn = Props.timer;
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
            if (Props.pawnKind != null)
            {
                pawnKind = Props.pawnKind;
            }
            else
            {
                pawnKind = Props.pawnKinds.RandomElement();
            }

            PawnGenerationRequest request = new PawnGenerationRequest(kind: Props.pawnKind, faction: Faction.OfPlayer, newborn: Props.newborn, forceGenerateNewPawn: true, canGeneratePawnRelations: Props.canGeneratePawnRelations);
            Pawn newThing = PawnGenerator.GeneratePawn(request);
            if (!Props.skillSettings.NullOrEmpty())
            {
                if (Props.purgeSkillsBeforeSetting)
                {
                    newThing.skills.skills.ForEach(s => s.Level = 0);
                }
                foreach (SkillLevelSetting skill in Props.skillSettings)
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
            if (Props.purgeApparel)
            {
                if (newThing.apparel.AnyApparel)
                {
                    newThing.apparel.DestroyAll();
                }
            }

            PreSpawnHook(newThing);

            GenSpawn.Spawn(newThing, parent.Position, parent.Map, WipeMode.Vanish);

            PostSpawnHook(newThing);
        }

        public virtual void PreSpawnHook(Pawn pawn)
        {

        }

        public virtual void PostSpawnHook(Pawn pawn)
        {

        }
    }
}
