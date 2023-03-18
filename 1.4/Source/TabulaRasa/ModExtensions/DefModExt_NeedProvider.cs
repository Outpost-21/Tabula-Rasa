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
    public class DefModExt_NeedProvider : DefModExtension
    {
        public SoundDef entrySound;
        public SoundDef exitSound;

        public bool guestsAllowed = true;

        public List<NeedProviderOption> needs = new List<NeedProviderOption>();

        public bool storePawnWhenIdle = false;
    }
}
