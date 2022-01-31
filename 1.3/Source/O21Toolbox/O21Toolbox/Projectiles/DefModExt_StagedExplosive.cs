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
    public class DefModExt_StagedExplosive : DefModExtension
    {
        public List<ExplosionStage> explosionStages = new List<ExplosionStage>();
    }

    public class ExplosionStage
    {
        public int countdown;
        public float radius;
        public DamageDef damType;
        public int damAmount;
        public float? armorPenetration = null;
        public SoundDef explosionSound = null;

        public ThingDef preExplosionSpawnThingDef = null;
        public float preExplosionSpawnChance = 0f;
        public int preExplosionSpawnThingCount = 0;

        public ThingDef postExplosionSpawnThingDef = null;
        public float postExplosionSpawnChance = 0f;
        public int postExplosionSpawnThingCount = 0;

        public float chanceToStartFire = 0f;
    }
}
