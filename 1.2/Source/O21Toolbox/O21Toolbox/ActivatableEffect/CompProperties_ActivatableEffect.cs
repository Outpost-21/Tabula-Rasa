using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ActivatableEffect
{
    public class CompProperties_ActivatableEffect : CompProperties
    {
        public CompProperties_ActivatableEffect()
        {
            compClass = typeof(Comp_ActivatableEffect);
        }

        public string ActivateLabel;

        public SoundDef activateSound;

        public AltitudeLayer altitudeLayer;

        public bool autoActivateOnDraft = true;
        public string DeactivateLabel;
        public SoundDef deactivateSound;

        public bool draftToUseGizmos = true;

        public bool gizmosOnEquip = false;
        public GraphicData graphicData;
        public SoundDef sustainerSound;

        public string uiIconPathActivate;
        public string uiIconPathDeactivate;

        public float Altitude => Altitudes.AltitudeFor(altitudeLayer);
    }
}
