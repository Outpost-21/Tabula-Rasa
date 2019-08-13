using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class DefModExt_HeadwearOffset : DefModExtension
    {
        [NoTranslate]
        public string wornGraphicPath = string.Empty;

        public Vector3 offset = new Vector3(0, 0, 0);

        public bool bodyDependant = false;

        public Vector3 offsetThin = new Vector3(0, 0, 0);
        public Vector3 offsetMale = new Vector3(0, 0, 0);
        public Vector3 offsetFemale = new Vector3(0, 0, 0);
        public Vector3 offsetFat = new Vector3(0, 0, 0);
        public Vector3 offsetHulk = new Vector3(0, 0, 0);
    }
}
