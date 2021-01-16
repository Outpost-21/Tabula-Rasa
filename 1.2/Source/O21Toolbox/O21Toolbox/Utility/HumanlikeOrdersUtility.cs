﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace O21Toolbox.Utility
{
    public class HumanlikeOrdersUtility
    {
        [StaticConstructorOnStartup]
        public static class _HumanlikeOrdersUtility
        {
            private static Dictionary<_Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
                floatMenuOptionList;

            private static readonly bool debugMode = false;

            //private static int floatMenuID = 0;
            //private static int lastSavedFloatMenuID = -1;
            private static readonly List<FloatMenuOption> savedList = new List<FloatMenuOption>();

            public static string optsID = "";
            public static string lastOptsID = "1";

            static _HumanlikeOrdersUtility()
            {
                DebugMessage("FMOL :: Initialized Constructor");
                optsID = "";
                lastOptsID = "1";
                foreach (var current in typeof(FloatMenuPatch).AllSubclassesNonAbstract())
                {
                    var item = (FloatMenuPatch)Activator.CreateInstance(current);

                    DebugMessage("FMOL :: Enter Loop Step");
                    var floatMenus = item.GetFloatMenus();
                    DebugMessage("FMOL :: Float Menus Variable Declared");

                    if (floatMenus != null)
                    {
                        DebugMessage("FMOL :: Float Menus Available Check Passed");
                        foreach (var floatMenu in floatMenus)
                        {
                            DebugMessage("FMOL :: Enter Float Menu Check Loop");
                            if (FloatMenuOptionList.ContainsKey(floatMenu.Key))
                            {
                                DebugMessage("FMOL :: Existing condition found for " + floatMenu.Key +
                                             " adding actions to dictionary.");
                                FloatMenuOptionList[floatMenu.Key].Add(floatMenu.Value);
                            }
                            else
                            {
                                DebugMessage("FMOL :: Existing condition not found for " + floatMenu.Key +
                                             " adding key and actions to dictionary.");
                                FloatMenuOptionList.Add(floatMenu.Key,
                                    new List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>> { floatMenu.Value });
                            }
                        }
                    }
                }

                var harmony = new Harmony("o21toolbox.neronix17.humanlikeorders");
                harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders"), null,
                    new HarmonyMethod(typeof(_HumanlikeOrdersUtility), nameof(AddHumanlikeOrders_PostFix)));
                //harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "CanTakeOrder"), null, new HarmonyMethod(typeof(_HumanlikeOrdersUtility), nameof(CanTakeOrder_PostFix)));
            }

            public static Dictionary<_Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
                FloatMenuOptionList
            {
                get
                {
                    if (floatMenuOptionList == null)
                    {
                        DebugMessage("FMOL :: Initialized List");
                        floatMenuOptionList =
                            new Dictionary<_Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>();
                    }
                    return floatMenuOptionList;
                }
            }

            public static void DebugMessage(string s)
            {
                if (debugMode) Log.Message(s);
            }


            //private static void CanTakeOrder_PostFix(Pawn pawn, ref bool __result)
            //{
            //    if (__result)
            //    {
            //        floatMenuID = Rand.Range(1000, 9999);
            //        DebugMessage("FMOL :: ID Set to " + floatMenuID);
            //    }
            //}


            // RimWorld.FloatMenuMakerMap
            public static void AddHumanlikeOrders_PostFix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
            {
                var c = IntVec3.FromVector3(clickPos);
                optsID = "";
                optsID += pawn.ThingID;
                optsID += c.ToString();
                if (opts != null)
                    for (var i = 0; i < opts.Count; i++)
                        optsID += opts[i].Label;
                if (optsID == lastOptsID)
                {
                    opts?.AddRange(savedList);
                    return;
                }
                DebugMessage("FMOL :: New list constructed");
                DebugMessage(optsID);
                lastOptsID = optsID;
                savedList.Clear();
                var things = c.GetThingList(pawn.Map);
                if (things.Count > 0)
                    foreach (var pair in FloatMenuOptionList)
                        if (!pair.Value.NullOrEmpty())
                        {
                            var passers = things.FindAll(x => pair.Key.Passes(x));
                            foreach (var passer in passers)
                                foreach (var func in pair.Value)
                                    if (func.Invoke(clickPos, pawn, passer) is List<FloatMenuOption> newOpts &&
                                        newOpts.Count > 0)
                                    {
                                        opts?.AddRange(newOpts);
                                        savedList.AddRange(newOpts);
                                    }
                        }
            }
        }


        public enum _ConditionType
        {
            IsType,
            IsTypeStringMatch,
            ThingHasComp,
            HediffHasComp
        }

        public struct _Condition : IEquatable<_Condition>
        {
            public _ConditionType Condition;
            public object Data;

            public _Condition(_ConditionType condition, object data)
            {
                Condition = condition;
                Data = data;
            }

            public override string ToString()
            {
                return "Condition_" + Condition + "_" + Data;
            }

            public bool Equals(_Condition other)
            {
                return Data == other.Data && Condition == other.Condition;
            }

            public bool Passes(object toCheck)
            {
                //Log.Message(toCheck.GetType().ToString());
                //Log.Message(Data.ToString());

                switch (Condition)
                {
                    case _ConditionType.IsType:
                        //////////////////////////
                        ///PSYCHOLOGY SPECIAL CASE
                        if (toCheck.GetType().ToString() == "Psychology.PsychologyPawn" && Data.ToString() == "Verse.Pawn")
                            return true;
                        //////////////////////////
                        if (toCheck.GetType() == Data.GetType() || Equals(toCheck.GetType(), Data) ||
                            toCheck.GetType() == Data as Type || toCheck.GetType().ToString() == Data.ToString() ||
                            Data.GetType().IsInstanceOfType(toCheck))
                            return true;
                        break;
                    case _ConditionType.IsTypeStringMatch:
                        if (toCheck.GetType().ToString() == (string)toCheck)
                            return true;
                        break;
                    case _ConditionType.ThingHasComp:
                        var dataTypeStr = Data?.ToString();
                        if (toCheck is ThingWithComps t && t.AllComps.Any(comp =>
                                comp?.props?.compClass is Type compClass &&
                                (compClass.ToString() == dataTypeStr ||
                                 compClass.BaseType?.ToString() == dataTypeStr)))
                            return true;
                        break;
                }
                return false;
            }
        }

        public abstract class FloatMenuPatch
        {
            public abstract IEnumerable<KeyValuePair<_Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
                GetFloatMenus();
        }
    }
}
