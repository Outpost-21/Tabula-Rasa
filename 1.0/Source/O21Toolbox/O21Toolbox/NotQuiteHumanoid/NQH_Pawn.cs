using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace O21Toolbox.NotQuiteHumanoid
{
    public class NQH_Pawn : Pawn
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            if(this.story == null)
            {
                this.story = new Pawn_StoryTracker(this);
            }
            if(this.Faction == Faction.OfPlayer)
            {
                if(this.playerSettings == null)
                {
                    this.playerSettings = new Pawn_PlayerSettings(this);
                }
                if(this.drafter == null)
                {
                    this.drafter = new Pawn_DraftController(this);
                }
                if(this.jobs == null)
                {
                    this.jobs = new Pawn_JobTracker(this);
                }
            }
        }

        public override void PostMake()
        {
            base.PostMake();

            if (ownership == null)
            {
                ownership = new Pawn_Ownership(this);
            }
            if (skills == null)
            {
                skills = new Pawn_SkillTracker(this);
                skills.skills.Find(sr => sr.def == SkillDefOf.Construction).Level = 15;
            }
            if (story == null)
            {
                story = new Pawn_StoryTracker(this);
            }
            if (guest == null)
            {
                guest = new Pawn_GuestTracker(this);
            }
            if (guilt == null)
            {
                guilt = new Pawn_GuiltTracker();
            }
            if (workSettings == null)
            {
                workSettings = new Pawn_WorkSettings(this);
                workSettings.EnableAndInitializeIfNotAlreadyInitialized();
                foreach (var workTypeDef in DefDatabase<WorkTypeDef>.AllDefs)
                {
                    workSettings.SetPriority(workTypeDef, 1);
                }
            }
        }
    }
}
