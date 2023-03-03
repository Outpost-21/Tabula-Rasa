using HarmonyLib;
using MVCF.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace HRF
{
	public class ARTMod : Mod
	{
		public ARTMod(ModContentPack content) : base(content)
		{
			Harmony harmony = new Harmony("Fraudecon.ART");
			harmony.PatchAll();
		}
	}

	//[HarmonyPatch(typeof(GenTypes), "AllTypes", MethodType.Getter)]
	//public static class test
    //{
	//	static void Postfix()
    //    {
	//		foreach (Assembly allActiveAssembly in GenTypes.AllActiveAssemblies)
	//		{
	//			try
	//			{
	//				var array = allActiveAssembly.GetTypes();
	//			}
	//			catch (ReflectionTypeLoadException ex)
	//			{
	//				StringBuilder sb = new StringBuilder();
	//				foreach (Exception exSub in ex.LoaderExceptions)
	//				{
	//					sb.AppendLine(exSub.Message);
	//					FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
	//					if (exFileNotFound != null)
	//					{
	//						if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
	//						{
	//							sb.AppendLine("Fusion Log:");
	//							sb.AppendLine(exFileNotFound.FusionLog);
	//						}
	//					}
	//					sb.AppendLine();
	//				}
	//				string errorMessage = sb.ToString();
	//				Log.Error("Exception getting types in assembly " + allActiveAssembly.ToString() + " - " + ex + " - " + new StackTrace() + " - " + errorMessage);
	//			}
	//		}
	//	}
    //}

	//[HarmonyPatch(typeof(JobDriver), nameof(JobDriver.DriverTick))]
	//public static class JobDriverPatch
    //{
	//	public static void Prefix(JobDriver __instance)
    //    {
	//		if (__instance.pawn.jobs.debugLog)
    //        {
	//			Log.Message("Pre Ticking " + __instance + " - " + __instance.pawn.CurJob);
    //        }
    //    }
	//
	//	public static void Postfix(JobDriver __instance)
	//	{
	//		if (__instance.pawn.jobs.debugLog)
	//		{
	//			Log.Message("Post Ticking " + __instance + " - " + __instance.pawn.CurJob);
	//			Log.ResetMessageCount();
	//		}
	//	}
	//}
}
