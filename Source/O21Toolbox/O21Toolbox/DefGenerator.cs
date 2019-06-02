using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Spaceship;

namespace O21Toolbox
{
    public static class DefGenerator
    {
        public static void GenerateImpliedDefs_PreResolve()
        {
            foreach (ThingDef def in SpaceshipDef_Generator.ImpliedSpaceshipDefs())
            {
                DefGenerator.AddImpliedDef<ThingDef>(def);
            }

            return;
        }

        public static void AddImpliedDef<T>(T def) where T : Def, new()
        {
            def.generated = true;
            if (def.modContentPack == null)
            {
                Log.Error(string.Format("Added def {0}:{1} without an associated modContentPack", def.GetType(), def.defName), false);
            }
            else
            {
                def.modContentPack.AddImpliedDef(def);
            }
            def.PostLoad();
            DefDatabase<T>.Add(def);
        }
    }
}
