using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleExplosionProperties
    {
        public bool doExplosionTimer = false;

        public string explosionTimerLabel;

        public bool showExplosionTimer = false;

        public int explosionTimerTickMax = 600;

        public int changeDrawExplosionTickMax = 60;

        public float explosionDrawScale = 60f;

        public int beginDrawExplosionTick = 600;

        public int explosionDrawRedTick = 300;

        public float explosiveRadius = 1.9f;

        public DamageDef explosiveDamageType;

        public int damageAmountBase = -1;

        public float armorPenetrationBase = -1f;

        public ThingDef postExplosionSpawnThingDef;

        public float postExplosionSpawnChance;

        public int postExplosionSpawnThingCount = 1;

        public bool applyDamageToExplosionCellsNeighbors;

        public ThingDef preExplosionSpawnThingDef;

        public float preExplosionSpawnChance;

        public int preExplosionSpawnThingCount = 1;

        public float chanceToStartFire;

        public bool damageFalloff;

        public bool explodeOnKilled;

        public GasType? postExplosionGasType;

        public float? postExplosionGasRadiusOverride;

        public int postExplosionGasAmount = 255;

        public bool doVisualEffects = true;

        public float propagationSpeed = 1f;

        public SoundDef explosionSound;
    }
}
