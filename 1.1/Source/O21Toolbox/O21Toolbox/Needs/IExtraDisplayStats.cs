using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Interface for showing extra display stats on stuff.
    /// </summary>
    public interface IExtraDisplayStats
    {
        IEnumerable<StatDrawEntry> SpecialDisplayStats();
    }
}
