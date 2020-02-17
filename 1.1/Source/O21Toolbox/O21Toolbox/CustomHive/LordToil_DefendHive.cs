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
    public class LordToil_DefendHive : LordToil
    {
        public ThingDef hiveDef;

        public float distToHiveToAttack = 10f;

        public LordToilData_DefendHive Data
        {
            get
            {
                return (LordToilData_DefendHive)this.data;
            }
        }
        
        public LordToil_DefendHive()
        {
            this.data = new LordToilData_DefendHive();
        }
        
        public LordToil_DefendHive(ThingDef hive_def)
        {
            this.data = new LordToilData_DefendHive();
            this.Data.hiveDef = hive_def;
            this.hiveDef = hive_def;
        }
        
        public override void UpdateAllDuties()
        {
            this.FilterOutUnspawnedHives();
            for (int i = 0; i < this.lord.ownedPawns.Count; i++)
            {
                CustomHive hiveFor = this.GetHiveFor(this.lord.ownedPawns[i]);
                bool flag = hiveFor != null;
                if (flag)
                {
                    HiveProperties hiveProperties = hiveFor.def.TryGetModExtension<HiveProperties>();
                    bool flag2 = hiveProperties != null;
                    if (flag2)
                    {
                        PawnDuty duty = new PawnDuty(hiveProperties.pawnDutyDef, hiveFor, this.distToHiveToAttack);
                        this.lord.ownedPawns[i].mindState.duty = duty;
                    }
                    else
                    {
                        PawnDuty duty2 = new PawnDuty(DutyDefOf.DefendHiveAggressively, hiveFor, this.distToHiveToAttack);
                        this.lord.ownedPawns[i].mindState.duty = duty2;
                    }
                }
            }
        }
        
        public void FilterOutUnspawnedHives()
        {
            GenCollection.RemoveAll<Pawn, CustomHive>(this.Data.assignedHives, (KeyValuePair<Pawn, CustomHive> x) => x.Value == null || !x.Value.Spawned);
        }
        
        public CustomHive GetHiveFor(Pawn pawn)
        {
            CustomHive customHive;
            bool flag = this.Data.assignedHives.TryGetValue(pawn, out customHive);
            CustomHive result;
            if (flag)
            {
                result = customHive;
            }
            else
            {
                customHive = this.FindClosestHive(pawn);
                bool flag2 = customHive != null;
                if (flag2)
                {
                    this.Data.assignedHives.Add(pawn, customHive);
                }
                result = customHive;
            }
            return result;
        }
        
        public CustomHive FindClosestHive(Pawn pawn)
        {
            return (CustomHive)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(this.Data.hiveDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, 0, false), 30f, (Thing x) => x.Faction == pawn.Faction || x.Faction.GoodwillWith(pawn.Faction) >= 100, null, 30, -1, false, RegionType.Set_Passable, false);
        }

        public class LordToilData_DefendHive : LordToilData
        {
            public Dictionary<Pawn, CustomHive> assignedHives = new Dictionary<Pawn, CustomHive>();
            
            public ThingDef hiveDef;
            
            public override void ExposeData()
            {
                bool flag = Scribe.mode == LoadSaveMode.Saving;
                if (flag)
                {
                    GenCollection.RemoveAll<Pawn, CustomHive>(this.assignedHives, (KeyValuePair<Pawn, CustomHive> x) => x.Key.Destroyed);
                }
                Scribe_Collections.Look<Pawn, CustomHive>(ref this.assignedHives, "assignedHives", LookMode.Reference, LookMode.Reference);
                Scribe_Defs.Look<ThingDef>(ref this.hiveDef, "hiveDef");
                bool flag2 = Scribe.mode == LoadSaveMode.PostLoadInit;
                if (flag2)
                {
                    GenCollection.RemoveAll<Pawn, CustomHive>(this.assignedHives, (KeyValuePair<Pawn, CustomHive> x) => x.Value == null);
                }
            }
        }
    }
}
