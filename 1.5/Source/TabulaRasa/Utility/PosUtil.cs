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
    public static class PosUtil
    {
        public static Rot4 FromAngleFlat2(float angle)
        {
            angle = GenMath.PositiveMod(angle, 360f);
            if (angle <= 45f)
                return Rot4.North;
            if (angle <= 135f)
                return Rot4.East;
            if (angle < 225f)
                return Rot4.South;
            if (angle <= 315f)
                return Rot4.West;
            return Rot4.North;
        }

        public static IntVec3 PositionOffset(this IntVec3 fromCenter, IntVec3 toCenter)
        {
            Rot4 rotation = FromAngleFlat2((fromCenter - toCenter).AngleFlat);
            if (rotation == Rot4.North)
                return IntVec3.North;
            if (rotation == Rot4.East)
                return IntVec3.East;
            if (rotation == Rot4.South)
                return IntVec3.South;
            if (rotation == Rot4.West)
                return IntVec3.West;
            return IntVec3.Zero;
        }
    }
}
