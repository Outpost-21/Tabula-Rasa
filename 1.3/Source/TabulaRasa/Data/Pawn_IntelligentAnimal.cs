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
    public class Pawn_IntelligentAnimal : Pawn
	{
		DefModExt_IntelligentAnimal modExt => def.GetModExtension<DefModExt_IntelligentAnimal>();

        public override void TickRare()
        {
            base.TickRare();
			UpdateWorkPriorities();
        }

		public void UpdateWorkPriorities()
        {
			InitialisePawnData();
			if(training != null && Faction != null && Faction.IsPlayer)
			{
                for (int i = 0; i < modExt.enabledWorkTypeDefs.Count; i++)
                {
					workSettings.SetPriority(modExt.enabledWorkTypeDefs[i], 3);
                }
			}
        }

		public void InitialisePawnData()
        {
			if(training != null && modExt.automaticTraining)
            {
                foreach(TrainableDef trainable in DefDatabase<TrainableDef>.AllDefs)
                {
                    if (training.CanAssignToTrain(trainable))
                    {
                        training.SetWanted(trainable, true);
                        training.Train(trainable, null, true);
                    }
                }
            }
			if(skills == null)
            {
				skills = new Pawn_SkillTracker(this);
				foreach(SkillRecord skill in skills.skills)
                {
                    skill.Level = 0;
                }
                for (int i = 0; i < modExt.skillSettings.Count; i++)
                {
                    skills.skills.Find(sk => sk.def == modExt.skillSettings[i].skill).Level = modExt.skillSettings[i].level;
                }
            }
			if(story == null)
            {
				story = new Pawn_StoryTracker(this)
				{
					bodyType = BodyTypeDefOf.Thin,
					crownType = CrownType.Average
                };
            }
			if(workSettings == null)
            {
				workSettings = new Pawn_WorkSettings(this);
				workSettings.EnableAndInitialize();
				workSettings.DisableAll();
            }
        }
    }
}
