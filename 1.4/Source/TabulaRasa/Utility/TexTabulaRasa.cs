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
    [StaticConstructorOnStartup]
    public static class TexTabulaRasa
    {
        public static readonly Texture2D DebugXenotypeEditor = ContentFinder<Texture2D>.Get("TabulaRasa/UI/XenotypeEditor");
    }
}
