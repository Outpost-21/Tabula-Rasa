using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class Projectile_AdvancedExplosive : Projectile_Explosive
    {
        DefModExt_EventCauser modExt_eventCauser;
        DefModExt_StagedExplosive modExt_stagedExplosive;

        public int ticksToDetonation_extra;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            modExt_eventCauser = def.GetModExtension<DefModExt_EventCauser>();
            modExt_stagedExplosive = def.GetModExtension<DefModExt_StagedExplosive>();
        }

        public override void Tick()
        {
            if (ticksToDetonation_extra > 0)
            {
                ticksToDetonation_extra--;
                if(modExt_stagedExplosive != null)
                {
                    foreach (ExplosionStage stage in modExt_stagedExplosive.explosionStages)
                    {
                        if (stage.countdown == ticksToDetonation_extra)
                        {
                            GenExplosion.DoExplosion(Position, Map, stage.radius, stage.damType, launcher, stage.damAmount, stage.armorPenetration ?? -1f, stage.explosionSound, equipmentDef, def, intendedTarget.Thing, stage.postExplosionSpawnThingDef, stage.postExplosionSpawnChance, stage.postExplosionSpawnThingCount, false, stage.preExplosionSpawnThingDef, stage.preExplosionSpawnChance, stage.preExplosionSpawnThingCount, stage.chanceToStartFire);
                        }
                    }
                }
            }
            base.Tick();
        }

        public override void Impact(Thing hitThing)
        {
            ticksToDetonation_extra = def.projectile.explosionDelay;
            if(modExt_eventCauser != null)
            {
                foreach (Condition condition in modExt_eventCauser.conditions)
                {
                    TriggerCondition(condition);
                }
            }
            base.Impact(hitThing);
        }

        public void TriggerCondition(Condition condition)
        {
            if (Rand.Chance(condition.chance))
            {
                if (Map.gameConditionManager.activeConditions.Any(c => c.def == condition.def))
                {
                    return;
                }
                Map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(condition.def, condition.duration.RandomInRange));
            }
        }
    }
}
