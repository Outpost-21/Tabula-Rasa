using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Dialog_BillConfig), "GeneratePawnRestrictionOptions")]
    public static class Patch_Dialog_BillConfig_GeneratePawnRestrictionOptions
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Dialog_BillConfig __instance, ref IEnumerable<Widgets.DropdownMenuElement<Pawn>> __result)
        {
            DefModExt_RecipeExtender modExt = __instance.bill.recipe.GetModExtension<DefModExt_RecipeExtender>();
            if (modExt != null)
            {
                if (modExt.requiredHediff != null)
                {
                    __result = PassThroughHediffRequirement(__instance.bill, modExt);
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<Widgets.DropdownMenuElement<Pawn>> PassThroughHediffRequirement(Bill bill, DefModExt_RecipeExtender modExt)
        {
            yield return new Widgets.DropdownMenuElement<Pawn>
            {
                option = new FloatMenuOption(modExt.requiredHediffAnyPawnMsg.Translate(), delegate
                {
                    bill.SetAnyPawnRestriction();
                }),
                payload = null
            };
            foreach (Widgets.DropdownMenuElement<Pawn> item in BillDialogUtility.GetPawnRestrictionOptionsForBill(bill, (Pawn p) => p.health.hediffSet.HasHediff(modExt.requiredHediff)))
            {
                yield return item;
            }
            yield break;
        }
    }
}
