using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

using O21Toolbox.Utility;

namespace O21Toolbox.CustomHive
{
    public class HiveProperties : DefModExtension
    {
        public int initialPawnSpawnDelay = 4000;
        
        public int pawnSpawnRadius = 5;
        
        public float maxSpawnedPawnsPoints = 500f;
        
        public int initialPawnsPoints = 200;
        
        public float pawnSpawnIntervalMin = 0.85f;
        
        public float pawnSpawnIntervalMax = 1.1f;
        
        public int pawnSpawnIntervalTicks = 30000;
        
        public List<PawnKindDef> pawnKindDefs;
        
        public List<int> pawnKindCosts;
        
        public DutyDef pawnDutyDef;
        
        public FactionDef pawnFactionDef;
        
        public bool pawnFactionAsHive = true;
        
        public float distToHiveAttackDocile = 16f;
        
        public float distToHiveAttackAgitated = 32f;
        
        public bool usePawnKindCosts = true;
        
        public int maxSpawns = -1;
        
        public bool pawnsDieOnHiveDeath = false;
    }
}
