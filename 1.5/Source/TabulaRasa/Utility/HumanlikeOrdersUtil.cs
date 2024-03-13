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
    [StaticConstructorOnStartup]
    public static class HumanlikeOrdersUtil
    {
        public static Dictionary<Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>> floatMenuOptionList;

        public static readonly List<FloatMenuOption> savedList = new List<FloatMenuOption>();

        public static string optsID = "";
        public static string lastOptsID = "1";

        static HumanlikeOrdersUtil()
        {
            LogUtil.LogDebug(" Humanlike Orders :: Initialised Constructor");
            optsID = "";
            lastOptsID = "1";
            foreach (var current in typeof(FloatMenuPatch).AllSubclassesNonAbstract())
            {
                var item = (FloatMenuPatch)Activator.CreateInstance(current);

                LogUtil.LogDebug("Humanlike Orders :: Enter Loop Step");
                var floatMenus = item.GetFloatMenus();
                LogUtil.LogDebug("Humanlike Orders :: Float Menus Variable Declared");

                if (floatMenus != null)
                {
                    LogUtil.LogDebug("Humanlike Orders :: Float Menus Available Check Passed");
                    foreach (var floatMenu in floatMenus)
                    {
                        LogUtil.LogDebug("Humanlike Orders :: Enter Float Menu Check Loop");
                        if (FloatMenuOptionList.ContainsKey(floatMenu.Key))
                        {
                            LogUtil.LogDebug($"Humanlike Orders :: Existing condition found for {floatMenu.Key} adding actions to dictionary.");
                            FloatMenuOptionList[floatMenu.Key].Add(floatMenu.Value);
                        }
                        else
                        {
                            LogUtil.LogDebug($"Humanlike Orders :: Existing condition not found for {floatMenu.Key} adding key and actions to dictionary.");
                            FloatMenuOptionList.Add(floatMenu.Key,
                                new List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>> { floatMenu.Value });
                        }
                    }
                }
            }
        }

        public static Dictionary<Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>> FloatMenuOptionList
        {
            get
            {
                if (floatMenuOptionList == null)
                {
                    LogUtil.LogDebug(" Humanlike Orders :: Initialized List");
                    floatMenuOptionList = new Dictionary<Condition, List<Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>();
                }
                return floatMenuOptionList;
            }
        }

        public abstract class FloatMenuPatch
        {
            public abstract IEnumerable<KeyValuePair<Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>>
                GetFloatMenus();
        }
    }
}
