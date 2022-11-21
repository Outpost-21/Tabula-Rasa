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
    public class DefModExt_HeadTypeStuff : DefModExtension
    {
        /// <summary>
        /// Allows the disabling of skin shader on head types.
        /// </summary>
        public bool useSkinShader = true;

        public Shader shader;
    }
}
