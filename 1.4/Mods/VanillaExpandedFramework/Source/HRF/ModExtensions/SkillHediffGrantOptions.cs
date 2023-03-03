using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace HRF
{
	public class SkillBonusRequirement
	{
		public SkillDef skill;

		public int minLevel;

		public Passion minPassion;

		public HediffDef hediff;

		public List<SkillBonusRequirement> requiredSkills;
		public bool PawnSatisfies(Pawn pawn)
		{
			if (pawn.skills == null)
			{
				return false;
			}
			var skillRecord = pawn.skills.GetSkill(skill);
			return skillRecord.Level >= minLevel && skillRecord.passion >= minPassion;
		}
	}
	public class SkillHediffGrantOptions : DefModExtension
    {
		public List<SkillBonusRequirement> hediffGrantRequirements;

	}
}
