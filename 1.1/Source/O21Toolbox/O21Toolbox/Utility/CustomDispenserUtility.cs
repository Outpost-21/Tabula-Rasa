using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.CustomDispenser;

namespace O21Toolbox.Utility
{
	[StaticConstructorOnStartup]
	public static class CustomDispenserUtility
	{
		public static bool IsFoodSourceOnMapSociallyProper(Thing t, Pawn getter, Pawn eater, bool allowSociallyImproper)
		{
			if (!allowSociallyImproper)
			{
				bool animalsCare = !getter.RaceProps.Animal;
				if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
				{
					return false;
				}
			}
			return true;
		}

		public static bool DispenserWillEat(Pawn p, ThingDef food, Pawn getter = null)
		{
			if (!p.IsPrisoner)
			{
				return p.WillEat(food, getter, true);
			}
			if (!p.RaceProps.CanEverEat(food))
			{
				return false;
			}
			if (p.foodRestriction != null)
			{
				if (!p.foodRestriction.Configurable)
				{
					return true;
				}
				if (p.foodRestriction.pawn.InMentalState)
				{
					return true;
				}
				FoodRestriction currentFoodRestriction = p.foodRestriction.CurrentFoodRestriction;
				if (currentFoodRestriction != null && !currentFoodRestriction.Allows(food) && (food.IsWithinCategory(ThingCategoryDefOf.Foods) || food.IsWithinCategory(ThingCategoryDefOf.Corpses)))
				{
					return false;
				}
			}
			return true;
		}
	}
}
