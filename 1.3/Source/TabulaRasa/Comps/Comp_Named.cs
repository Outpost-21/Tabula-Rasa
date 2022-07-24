using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace TabulaRasa
{
    public class Comp_Named : ThingComp
    {
        private string name;
        public CompProperties_Named Props => (CompProperties_Named)props;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<string>(ref name, "name");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            name = GenText.CapitalizeAsTitle(GrammarResolver.Resolve("name", new GrammarRequest
            {
                Includes =
                {
                    this.Props.nameMaker
                }
            }));
        }

        public override string TransformLabel(string label)
        {
            string result;
            if (Props.nameFormat == ThingNameFormat.Prefix)
            {
                result = name + " " + label;
            }
            else if (Props.nameFormat == ThingNameFormat.Suffix)
            {
                result = label + " " + name;
            }
            else if (Props.nameFormat == ThingNameFormat.Bracketed)
            {
                result = label + " (" + name + ")";
            }
            else
            {
                result = name;
            }
            return result;
        }
    }
}
