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
    public class Comp_Renameable : ThingComp
    {
        CompProperties_Renameable Props => (CompProperties_Renameable)props;

        public string customLabel = null;

        public string CustomLabel
        {
            get
            {
                if (customLabel == null)
                {
                    return parent.Label.ToString();
                }

                return customLabel;
            }
            set
            {
                customLabel = value;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref customLabel, "customLabel");
        }

        public override string TransformLabel(string label)
        {
            if (customLabel != null)
            {
                return customLabel;
            }
            return base.TransformLabel(label);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (!respawningAfterLoad)
            {
                Rename();
            }
        }

        public void Rename()
        {
            Find.WindowStack.Add(new Dialog_NameThing(parent));
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            yield return new Command_Action
            {
                defaultLabel = "Rename",
                defaultDesc = "Changes the name of this " + parent.def.label.ToString(),
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true),
                action = delegate
                {
                    Rename();
                }
            };
            yield break;
        }
    }
}
