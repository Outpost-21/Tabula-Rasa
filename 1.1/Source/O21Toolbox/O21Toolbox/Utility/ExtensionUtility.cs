using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public static class ExtensionUtility
    {
        public static T TryGetModExtension<T>(this Def def) where T : DefModExtension
        {
            bool flag = def.HasModExtension<T>();
            T result;
            if (flag)
            {
                result = def.GetModExtension<T>();
            }
            else
            {
                result = default(T);
            }
            return result;
        }
    }
}
