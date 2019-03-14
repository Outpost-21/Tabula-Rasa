using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI.Group;

using O21Toolbox.Utility;

namespace O21Toolbox.CustomHive
{
    public class LordJob_DefendHive : LordJob
    {
        public ThingDef hiveDef;

        public LordJob_DefendHive()
        {
        }
        
        public LordJob_DefendHive(ThingDef hive_def)
        {
            this.hiveDef = hive_def;
        }
        
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            float distToHiveToAttack = 10f;
            float distToHiveToAttack2 = 32f;
            bool flag = this.hiveDef != null;
            if (flag)
            {
                HiveProperties hiveProperties = this.hiveDef.TryGetModExtension<HiveProperties>();
                bool flag2 = hiveProperties != null;
                if (flag2)
                {
                    distToHiveToAttack = hiveProperties.distToHiveAttackDocile;
                    distToHiveToAttack2 = hiveProperties.distToHiveAttackAgitated;
                }
            }
            LordToil_DefendHive lordToil_DefendHive = new LordToil_DefendHive(this.hiveDef);
            lordToil_DefendHive.distToHiveToAttack = distToHiveToAttack;
            stateGraph.StartingToil = lordToil_DefendHive;
            LordToil_DefendHive lordToil_DefendHive2 = new LordToil_DefendHive(this.hiveDef);
            lordToil_DefendHive2.distToHiveToAttack = distToHiveToAttack2;
            stateGraph.AddToil(lordToil_DefendHive2);
            Transition transition = new Transition(lordToil_DefendHive, lordToil_DefendHive2, false, true);
            transition.AddTrigger(new Trigger_PawnHarmed(1f, false, null));
            transition.AddTrigger(new Trigger_Memo("HiveAttacked"));
            stateGraph.AddTransition(transition, false);
            Transition transition2 = new Transition(lordToil_DefendHive, lordToil_DefendHive2, false, true);
            transition2.canMoveToSameState = true;
            transition2.AddSource(lordToil_DefendHive2);
            transition2.AddTrigger(new Trigger_Memo("HiveDestroyed"));
            stateGraph.AddTransition(transition2, false);
            Transition transition3 = new Transition(lordToil_DefendHive2, lordToil_DefendHive, false, true);
            transition3.AddTrigger(new Trigger_TicksPassedWithoutHarm(500));
            stateGraph.AddTransition(transition3, false);
            return stateGraph;
        }
    }
}
