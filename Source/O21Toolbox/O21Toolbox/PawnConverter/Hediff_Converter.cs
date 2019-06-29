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

            if(this.Severity >= 0.99)
            {
                AttemptConversion();
            }
        }

        private void AttemptConversion()
        {
            if(Util_PawnConvert.IsViableRace(this.pawn, this.def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe) && Util_PawnConvert.IsRequiredSex(this.pawn, this.def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe))
            {
                if(this.pawn.def.defName == "Human")
                {
                    Pawn producedPawn = Util_PawnConvert.HumanPawnConversion(this.pawn, this.def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe);
                    Pawn truePawn = Util_PawnConvert.PawnConversion(producedPawn, this.def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe);
                    GenPlace.TryPlaceThing(truePawn, this.pawn.Position, this.pawn.Map, ThingPlaceMode.Direct, null, null);
                }
                else
                {
                    Pawn producedPawn = Util_PawnConvert.PawnConversion(this.pawn, this.def.GetModExtension<DefModExt_Hediff_Converter>().conversionRecipe);
                    GenPlace.TryPlaceThing(producedPawn, this.pawn.Position, this.pawn.Map, ThingPlaceMode.Direct, null, null);
                }
                this.pawn.Destroy();
            }
            else
            {
                // Log.Message("Conversion not viable, killing pawn...");
                this.pawn.Kill(null, this);
            }
        }
    }
}
