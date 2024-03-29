﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(DebugWindowsOpener), "DrawButtons")]
    public static class Patch_DebugWindowsOpener_DrawButtons
	{
		public static bool patched;

		[HarmonyPrepare]
		public static bool Prepare()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!patched)
				{
					LogUtil.Warning("DebugWindowsOpener_Patch could not be applied.");
				}
			});
			return true;
		}

		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			patched = false;
			CodeInstruction[] instructionsArr = instructions.ToArray();
			FieldInfo widgetRowField = AccessTools.Field(typeof(DebugWindowsOpener), "widgetRow");
			CodeInstruction[] array = instructionsArr;
			foreach (CodeInstruction inst in array)
			{
				if (!patched && widgetRowField != null && inst.opcode == OpCodes.Bne_Un_S)
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);
					yield return new CodeInstruction(OpCodes.Ldfld, widgetRowField);
					yield return new CodeInstruction(OpCodes.Call, new Action<WidgetRow>(DrawToolboxButtons).Method);
					patched = true;
				}
				yield return inst;
			}
		}

		public static void DrawToolboxButtons(WidgetRow widgets)
		{
            if (ModLister.BiotechInstalled && TabulaRasaMod.settings.showXenotypeEditorMenu)
			{
				if (widgets.ButtonIcon(TexTabulaRasa.DebugXenotypeEditor, "Open the Xenotype Editor. \n\nThis lets you edit Xenotypes without having to dive several pages into a new game."))
				{
					WindowStack windowStack = Find.WindowStack;
					if (windowStack.IsOpen<Dialog_CreateXenotype>())
					{
						windowStack.TryRemove(typeof(Dialog_CreateXenotype));
					}
					else
					{
						windowStack.Add(new Dialog_CreateXenotype(-1, delegate { windowStack.TryRemove(typeof(Dialog_CreateXenotype)); }));
					}
				}
			}
		}
	}
}
