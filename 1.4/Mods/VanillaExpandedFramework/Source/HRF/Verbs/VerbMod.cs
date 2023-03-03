using HarmonyLib;
using Verse;

namespace HRF
{
    public class VerbMod : Mod
    {
        public VerbMod(ModContentPack content) : base(content)
        {
            var harm = new Harmony("fradulenteconomics.verbs");
            GraveblossomHelpers.DoPatches(harm);
            IndestructibleHediffs.DoPatches(harm);
        }
    }
}