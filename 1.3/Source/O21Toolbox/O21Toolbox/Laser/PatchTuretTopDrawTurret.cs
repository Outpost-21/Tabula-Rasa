using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

namespace O21Toolbox.Laser
{
    //[HarmonyPatch(typeof(TurretTop), "DrawTurret", new Type[] { }), StaticConstructorOnStartup]
    //class PatchTuretTopDrawTurret
    //{

    //    static bool Prefix(TurretTop __instance)
    //    {
    //        if (!(__instance.parentTurret is Building_LaserGun)) { return true; }

    //        Building_LaserGun turret = (Building_LaserGun)__instance.parentTurret;
    //        float rotation = (float)__instance.curRotationInt;
    //        if (turret.TargetCurrentlyAimingAt.HasThing)
    //        {
    //            rotation = (turret.TargetCurrentlyAimingAt.CenterVector3 - turret.TrueCenter()).AngleFlat();
    //        }

    //        IDrawnWeaponWithRotation gunRotation = turret.gun as IDrawnWeaponWithRotation;
    //        if (gunRotation != null) rotation += gunRotation.RotationOffset;

    //        Material material = turret.def.building.turretTopMat;
    //        SpinningLaserGunTurret spinningGun = turret.gun as SpinningLaserGunTurret;
    //        if (spinningGun != null)
    //        {
    //            spinningGun.turret = turret;
    //            material = spinningGun.Graphic.MatSingle;
    //        }
            
    //        Vector3 b = new Vector3(turret.def.building.turretTopOffset.x, 0f, turret.def.building.turretTopOffset.y);
    //        float turretTopDrawSize = turret.def.building.turretTopDrawSize;
    //        Matrix4x4 matrix = default(Matrix4x4);
    //        matrix.SetTRS(turret.DrawPos + Altitudes.AltIncVect + b, rotation.ToQuat(), new Vector3(turretTopDrawSize, 1f, turretTopDrawSize));
    //        Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);

    //        return false;
    //    }
    //}
}
