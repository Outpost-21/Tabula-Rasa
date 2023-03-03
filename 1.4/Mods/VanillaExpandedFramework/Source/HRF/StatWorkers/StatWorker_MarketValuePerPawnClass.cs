using RimWorld;
using System.Text;
using Verse;

namespace HRF
{
    public class StatWorker_MarketValuePerPawnClass : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            var comp = req.Thing?.TryGetComp<CompPawnClass>();
            if (comp != null && comp.HasClass(out var classTrait))
            {
                val += comp.level * classTrait.valuePerLevelOffset;
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var comp = req.Thing?.TryGetComp<CompPawnClass>();
            if (comp != null && comp.HasClassTrait(out var classTrait) && comp.ClassTraitDef.valuePerLevelOffset > 0)
            {
                stringBuilder.AppendLine("ART.MarketValuePerLevel".Translate(comp.level, classTrait.LabelCap, (comp.level * comp.ClassTraitDef.valuePerLevelOffset).ToStringMoney()));
            }
            return stringBuilder.ToString();
        }
    }
}
