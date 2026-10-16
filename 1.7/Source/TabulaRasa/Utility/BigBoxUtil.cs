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
    public static class BigBoxUtil
    {
        public static DefModExt_BigBox GetModExtensionBigBox(this Def def)
        {
            var modExtensions = def.modExtensions;
            if (modExtensions == null)
            {
                return null;
            }
            for (int i = 0, count = modExtensions.Count; i < count; i++)
            {
                if (modExtensions[i] is DefModExt_BigBox modExtension)
                {
                    return modExtension;
                }
            }
            return null;
        }
    }
}
