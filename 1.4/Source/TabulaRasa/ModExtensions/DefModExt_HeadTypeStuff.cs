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

        /// <summary>
        /// If Skin Shader is disabled, then this can be set so it uses a shader intended.
        /// If this isn't set but useSkinShader is false, it'll default to Cutout.
        /// </summary>
        public ShaderTypeDef shaderType;
    }
}
