using RimWorld;
using System.Net.Mail;
using Verse;
using Verse.AI;

namespace HRF
{
    public static class UseHelper
    {
        public static bool CanUseIt(this Pawn pawn, Thing t, out string cannotUseMessage)
        {
            if (CompThingInUse.things.TryGetValue(t, out var comp))
            {
                foreach (var useProps in comp.Props.useProperties)
                {
                    if (comp.UseIsEnabled(useProps) && useProps.hediffRequired)
                    {
                        var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(useProps.hediff) as HediffResource;
                        if (hediffResource is null)
                        {
                            var noResourceReason = "ART.NoResource".Translate(pawn.Named("PAWN"), useProps.hediff.label);
                            cannotUseMessage = useProps.cannotUseMessageKey.Translate(pawn.Named("PAWN"), t.Label, noResourceReason);
                            return false;
                        }
                        else if (!hediffResource.CanUse(useProps, out string failReason))
                        {
                            cannotUseMessage = useProps.cannotUseMessageKey.Translate(pawn.Named("PAWN"), t.Label, failReason);
                            return false;
                        }
                    }
                }
            }
            else
            {
                var extension = t.def.GetModExtension<Extension_ThingInUse>();
                if (extension != null)
                {
                    foreach (var useProps in extension.useProperties)
                    {
                        if (useProps.hediffRequired)
                        {
                            var hediffResource = pawn.health.hediffSet.GetFirstHediffOfDef(useProps.hediff) as HediffResource;
                            if (hediffResource is null)
                            {
                                var noResourceReason = "ART.NoResource".Translate(pawn.Named("PAWN"), useProps.hediff.label);
                                cannotUseMessage = useProps.cannotUseMessageKey.Translate(pawn.Named("PAWN"), t.Label, noResourceReason);
                                return false;
                            }
                            else if (!hediffResource.CanUse(useProps, out string failReason))
                            {
                                cannotUseMessage = useProps.cannotUseMessageKey.Translate(pawn.Named("PAWN"), t.Label, failReason);
                                return false;
                            }
                        }
                    }
                }
            }
            cannotUseMessage = null;
            return true;
        }

        public static bool CanUseIt(this Pawn pawn, string thingLabel, UseProps useProps, float resourceAmount, string cannotUseMessageKey, out string failMessage)
        {
            failMessage = null;
            if (useProps.hediffRequired &&  resourceAmount < 0 && !pawn.HasResource(useProps.hediff, -resourceAmount))
            {
                if (!cannotUseMessageKey.NullOrEmpty())
                {
                    failMessage = cannotUseMessageKey.Translate(pawn.Named("PAWN"), thingLabel, "ART.NotEnoughResource".Translate());
                }
                return false;
            }
            return true;
        }
    }
}
