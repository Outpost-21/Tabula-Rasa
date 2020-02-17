using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnConverter
{
    public class Hediff_Converter : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            if (base.Part != null && base.Part.coverageAbs <= 0f)
            {
                Log.Error(string.Concat(new object[]
                {
                    "Added injury to ",
                    base.Part.def,
                    " but it should be impossible to hit it. pawn=",
                    this.pawn.ToStringSafe<Pawn>(),
                    " dinfo=",
                    dinfo.ToStringSafe<DamageInfo?>()
                }), false);
            }
        }

        public override void Tick()
        {
            base.Tick();

            if(Severity >= 0.99)
            {
                AttemptConversion();
            }
        }

        private void AttemptConversion()
        {
            if(Util_PawnConvert.IsViableRace(pawn, def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe) && Util_PawnConvert.IsRequiredSex(pawn, def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe))
            {
                Pawn producedPawn = Util_PawnConvert.PawnConversion(pawn, def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe);
                GenPlace.TryPlaceThing(producedPawn, pawn.Position, pawn.Map, ThingPlaceMode.Direct, null, null);
                pawn.Destroy();
            }
            else
            {
                // Log.Message("Conversion not viable, killing pawn...");
                pawn.Kill(null, this);
            }
        }
    }
}
