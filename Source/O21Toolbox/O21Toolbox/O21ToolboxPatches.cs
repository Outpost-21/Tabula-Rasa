using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
using UnityEngine;
using RimWorld;
using Verse;
using System.Reflection;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public static class O21ToolboxPatches
    {
        static O21ToolboxPatches()
        {
            HarmonyInstance O21Toolbox = HarmonyInstance.Create("com.o21toolbox.rimworld.mod");

            O21Toolbox.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
