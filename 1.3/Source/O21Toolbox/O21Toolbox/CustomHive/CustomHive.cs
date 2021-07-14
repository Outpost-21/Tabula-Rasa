using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

using O21Toolbox.Utility;

namespace O21Toolbox.CustomHive
{
    public class CustomHive : Building
    {
        public int InitialPawnSpawnDelay = 960;
        
        public int PawnSpawnRadius = 5;
        
        public float MaxSpawnedPawnsPoints = 500f;
        
        public int InitialPawnsPoints = 260;
        
        public bool active = true;
        
        public bool canSpawnPawns = true;
        
        public int nextPawnSpawnTick = -1;
        
        public List<Pawn> spawnedPawns = new List<Pawn>();
        
        public int ticksToSpawnInitialPawns = -1;
        
        public FloatRange PawnSpawnIntervalDays = new FloatRange(0.85f, 1.1f);
        
        public Lord Lord
        {
            get
            {
                Predicate<Pawn> hasDefendHiveLord = delegate (Pawn x)
                {
                    Lord lord = LordUtility.GetLord(x);
                    return lord != null && lord.LordJob is LordJob_DefendHive;
                };
                Pawn foundPawn = this.spawnedPawns.Find(hasDefendHiveLord);
                bool spawned = base.Spawned;
                if (spawned)
                {
                    bool flag = foundPawn == null;
                    if (flag)
                    {
                        HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
                        bool pawnFactionAsHive = hiveProperties.pawnFactionAsHive;
                        Faction faction;
                        if (pawnFactionAsHive)
                        {
                            faction = base.Faction;
                        }
                        else
                        {
                            faction = FactionUtility.DefaultFactionFrom(hiveProperties.pawnFactionDef);
                        }
                        RegionTraverser.BreadthFirstTraverse(RegionAndRoomQuery.GetRegion(this, RegionType.Set_Passable), (Region from, Region to) => true, delegate (Region r)
                        {
                            List<Thing> list = r.ListerThings.ThingsOfDef(this.def);
                            for (int i = 0; i < list.Count; i++)
                            {
                                bool flag3 = list[i] != this;
                                if (flag3)
                                {
                                    bool flag4 = list[i].Faction == faction;
                                    if (flag4)
                                    {
                                        foundPawn = ((CustomHive)list[i]).spawnedPawns.Find(hasDefendHiveLord);
                                        bool flag5 = foundPawn != null;
                                        if (flag5)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            return false;
                        }, 20, RegionType.Set_Passable);
                    }
                    bool flag2 = foundPawn != null;
                    if (flag2)
                    {
                        return LordUtility.GetLord(foundPawn);
                    }
                }
                return null;
            }
        }

        public float SpawnedPawnsPoints
        {
            get
            {
                this.FilterOutUnspawnedPawns();
                float num = 0f;
                for (int i = 0; i < this.spawnedPawns.Count; i++)
                {
                    HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
                    bool flag = hiveProperties != null;
                    if (flag)
                    {
                        num += (float)hiveProperties.pawnKindCosts[hiveProperties.pawnKindDefs.IndexOf(this.spawnedPawns[i].kindDef)];
                    }
                    else
                    {
                        num += this.spawnedPawns[i].kindDef.combatPower;
                    }
                }
                return num;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            bool flag = base.Faction == null;
            if (flag)
            {
                this.SetFaction(Faction.OfInsects, null);
            }
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool flag2 = hiveProperties != null;
            if (flag2)
            {
                this.InitialPawnSpawnDelay = hiveProperties.initialPawnSpawnDelay;
                this.PawnSpawnRadius = hiveProperties.pawnSpawnRadius;
                this.MaxSpawnedPawnsPoints = hiveProperties.maxSpawnedPawnsPoints;
                this.InitialPawnsPoints = hiveProperties.initialPawnsPoints;
                this.PawnSpawnIntervalDays = new FloatRange(hiveProperties.pawnSpawnIntervalMin, hiveProperties.pawnSpawnIntervalMax);
            }
            this.StartInitialPawnSpawnCountdown();
        }
        
        public override void PostMake()
        {
            base.PostMake();
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool flag = hiveProperties != null;
            if (flag)
            {
                this.InitialPawnSpawnDelay = hiveProperties.initialPawnSpawnDelay;
                this.PawnSpawnRadius = hiveProperties.pawnSpawnRadius;
                this.MaxSpawnedPawnsPoints = hiveProperties.maxSpawnedPawnsPoints;
                this.InitialPawnsPoints = hiveProperties.initialPawnsPoints;
                this.PawnSpawnIntervalDays = new FloatRange(hiveProperties.pawnSpawnIntervalMin, hiveProperties.pawnSpawnIntervalMax);
            }
        }
        
        public void StartInitialPawnSpawnCountdown()
        {
            this.ticksToSpawnInitialPawns = this.InitialPawnSpawnDelay;
        }
        
        public void SpawnInitialPawnsNow()
        {
            this.ticksToSpawnInitialPawns = -1;
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool flag = hiveProperties != null;
            if (flag)
            {
                bool usePawnKindCosts = hiveProperties.usePawnKindCosts;
                if (usePawnKindCosts)
                {
                    while (this.SpawnedPawnsPoints < (float)this.InitialPawnsPoints)
                    {
                        Pawn pawn;
                        bool flag2 = !this.TrySpawnPawn(out pawn);
                        if (flag2)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    bool flag3 = hiveProperties.maxSpawns <= -1;
                    if (flag3)
                    {
                        Pawn pawn2;
                        bool flag4 = !this.TrySpawnPawn(out pawn2);
                        if (flag4)
                        {
                            return;
                        }
                    }
                    else
                    {
                        int num = 0;
                        while (num < hiveProperties.maxSpawns && this.spawnedPawns.Count < hiveProperties.maxSpawns)
                        {
                            Pawn pawn3;
                            bool flag5 = !this.TrySpawnPawn(out pawn3);
                            if (flag5)
                            {
                                return;
                            }
                            num++;
                        }
                    }
                }
            }
            this.CalculateNextPawnSpawnTick();
        }
        
        public override void Tick()
        {
            base.Tick();
            bool spawned = base.Spawned;
            if (spawned)
            {
                this.FilterOutUnspawnedPawns();
                bool flag = !this.active && !GridsUtility.Fogged(base.Position, base.Map);
                if (flag)
                {
                    this.Activate();
                }
                bool flag2 = this.active;
                if (flag2)
                {
                    bool flag3 = this.ticksToSpawnInitialPawns > 0;
                    if (flag3)
                    {
                        this.ticksToSpawnInitialPawns -= 250;
                        bool flag4 = this.ticksToSpawnInitialPawns <= 0;
                        if (flag4)
                        {
                            this.SpawnInitialPawnsNow();
                        }
                    }
                    bool flag5 = Find.TickManager.TicksGame >= this.nextPawnSpawnTick;
                    if (flag5)
                    {
                        HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
                        bool flag6 = hiveProperties != null;
                        if (flag6)
                        {
                            bool usePawnKindCosts = hiveProperties.usePawnKindCosts;
                            if (usePawnKindCosts)
                            {
                                bool flag7 = this.SpawnedPawnsPoints < this.MaxSpawnedPawnsPoints;
                                if (flag7)
                                {
                                    Pawn pawn;
                                    bool flag8 = this.TrySpawnPawn(out pawn);
                                    bool flag9 = flag8 && pawn.caller != null;
                                    if (flag9)
                                    {
                                        pawn.caller.DoCall();
                                    }
                                }
                            }
                            else
                            {
                                bool flag10 = this.spawnedPawns.Count < hiveProperties.maxSpawns;
                                if (flag10)
                                {
                                    Pawn pawn2;
                                    bool flag11 = this.TrySpawnPawn(out pawn2);
                                    bool flag12 = flag11 && pawn2.caller != null;
                                    if (flag12)
                                    {
                                        pawn2.caller.DoCall();
                                    }
                                }
                                else
                                {
                                    bool flag13 = hiveProperties.maxSpawns <= -1;
                                    if (flag13)
                                    {
                                        Pawn pawn3;
                                        bool flag14 = this.TrySpawnPawn(out pawn3);
                                        bool flag15 = flag14 && pawn3.caller != null;
                                        if (flag15)
                                        {
                                            pawn3.caller.DoCall();
                                        }
                                    }
                                }
                            }
                        }
                        this.CalculateNextPawnSpawnTick();
                    }
                }
            }
        }
        
        public override void DeSpawn(DestroyMode mode = 0)
        {
            Map map = base.Map;
            base.DeSpawn(mode);
            List<Lord> lords = map.lordManager.lords;
            for (int i = 0; i < lords.Count; i++)
            {
                lords[i].ReceiveMemo(Hive.MemoDeSpawned);
            }
        }
        
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            bool flag = dinfo.Def.ExternalViolenceFor(this) && dinfo.Instigator != null && dinfo.Instigator.Faction != null;
            if (flag)
            {
                Lord lord = this.Lord;
                bool flag2 = lord != null;
                if (flag2)
                {
                    lord.ReceiveMemo(Hive.MemoAttackedByEnemy);
                }
            }
            bool flag3 = dinfo.Def == DamageDefOf.Flame && (float)this.HitPoints < (float)base.MaxHitPoints * 0.3f;
            if (flag3)
            {
                Lord lord2 = this.Lord;
                bool flag4 = lord2 != null;
                if (flag4)
                {
                    lord2.ReceiveMemo(Hive.MemoBurnedBadly);
                }
            }
            base.PostApplyDamage(dinfo, totalDamageDealt);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.active, "active", false, false);
            Scribe_Values.Look<int>(ref this.nextPawnSpawnTick, "nextPawnSpawnTick", 0, false);
            Scribe_Collections.Look<Pawn>(ref this.spawnedPawns, "spawnedPawns", LookMode.Reference, new object[0]);
            Scribe_Values.Look<int>(ref this.ticksToSpawnInitialPawns, "ticksToSpawnInitialPawns", 0, false);
            Scribe_Values.Look<bool>(ref this.canSpawnPawns, "canSpawnPawns", true, false);
            bool flag = Scribe.mode == LoadSaveMode.PostLoadInit;
            if (flag)
            {
                this.spawnedPawns.RemoveAll((Pawn x) => x == null);
            }
        }
        
        public void Activate()
        {
            this.active = true;
            this.nextPawnSpawnTick = Find.TickManager.TicksGame + Rand.Range(200, 400);
        }
        
        public void CalculateNextPawnSpawnTick()
        {
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool flag = hiveProperties != null;
            if (flag)
            {
                this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(this.PawnSpawnIntervalDays.RandomInRange * (float)hiveProperties.pawnSpawnIntervalTicks);
            }
            else
            {
                this.nextPawnSpawnTick = Find.TickManager.TicksGame + (int)(this.PawnSpawnIntervalDays.RandomInRange * 60000f);
            }
        }
        
        public void FilterOutUnspawnedPawns()
        {
            this.spawnedPawns.RemoveAll((Pawn x) => !x.Spawned);
        }
        
        public bool TrySpawnPawn(out Pawn pawn)
        {
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool flag = hiveProperties != null;
            if (flag)
            {
                bool flag2 = hiveProperties.usePawnKindCosts && hiveProperties.pawnKindDefs.Count == hiveProperties.pawnKindCosts.Count;
                PawnKindDef pawnKindDef;
                if (flag2)
                {
                    float spawnedPawnsPoints = this.SpawnedPawnsPoints;
                    List<PawnKindDef> list = new List<PawnKindDef>();
                    for (int i = 0; i < hiveProperties.pawnKindDefs.Count; i++)
                    {
                        bool flag3 = spawnedPawnsPoints + (float)hiveProperties.pawnKindCosts[i] <= this.MaxSpawnedPawnsPoints;
                        if (flag3)
                        {
                            list.Add(hiveProperties.pawnKindDefs[i]);
                        }
                    }
                    bool flag4 = !GenCollection.TryRandomElement<PawnKindDef>(list, out pawnKindDef);
                    if (flag4)
                    {
                        pawn = null;
                        return false;
                    }
                }
                else
                {
                    bool flag5 = !GenCollection.TryRandomElement<PawnKindDef>(hiveProperties.pawnKindDefs, out pawnKindDef);
                    if (flag5)
                    {
                        pawn = null;
                        return false;
                    }
                }
                bool flag6 = pawnKindDef != null;
                if (flag6)
                {
                    bool pawnFactionAsHive = hiveProperties.pawnFactionAsHive;
                    Faction faction;
                    if (pawnFactionAsHive)
                    {
                        faction = base.Faction;
                    }
                    else
                    {
                        faction = FactionUtility.DefaultFactionFrom(hiveProperties.pawnFactionDef);
                    }
                    pawn = PawnGenerator.GeneratePawn(pawnKindDef, faction);
                    GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(base.Position, base.Map, this.PawnSpawnRadius, null), base.Map, 0);
                    pawn.SetFactionDirect(faction);
                    pawn.ageTracker.AgeBiologicalTicks = 0L;
                    pawn.ageTracker.AgeChronologicalTicks = 0L;
                    this.spawnedPawns.Add(pawn);
                    Lord lord = this.Lord;
                    bool flag7 = lord == null;
                    if (flag7)
                    {
                        lord = this.CreateNewLord();
                    }
                    lord.AddPawn(pawn);
                    return true;
                }
            }
            pawn = null;
            return false;
        }
        
        public Lord CreateNewLord()
        {
            HiveProperties hiveProperties = this.def.TryGetModExtension<HiveProperties>();
            bool pawnFactionAsHive = hiveProperties.pawnFactionAsHive;
            Faction faction;
            if (pawnFactionAsHive)
            {
                faction = base.Faction;
            }
            else
            {
                faction = FactionUtility.DefaultFactionFrom(hiveProperties.pawnFactionDef);
            }
            return LordMaker.MakeNewLord(faction, new LordJob_DefendHive(this.def), base.Map, null);
        }
    }
}
