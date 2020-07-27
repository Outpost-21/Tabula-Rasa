using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public class AnimalApparelUtility
    {
        public static bool IsAnimal(Pawn pawn)
        {
            if(pawn != null)
            {
                if (pawn.RaceProps != null && pawn.RaceProps.Animal)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsColonyAnimal(Pawn pawn)
        {
            if(IsAnimal(pawn) && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                return true;
            }
            return false;
        }

        public static void InitAnimalApparelTrackers(Pawn pawn)
        {
            if(pawn.outfits == null)
            {
                pawn.outfits = new Pawn_OutfitTracker(pawn);
            }
            if(pawn.equipment == null)
            {
                pawn.equipment = new Pawn_EquipmentTracker(pawn);
            }
            if(pawn.apparel == null)
            {
                pawn.apparel = new Pawn_ApparelTracker(pawn);
            }
        }
    }
}
