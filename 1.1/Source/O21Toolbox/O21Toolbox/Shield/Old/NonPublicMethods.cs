using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

using HarmonyLib;

namespace O21Toolbox.Shield
{
	[StaticConstructorOnStartup]
    public static class NonPublicMethods
	{
		// Token: 0x04000086 RID: 134
		public static Action<Building_TurretGun> Building_TurretGun_BurstComplete = (Action<Building_TurretGun>)Delegate.CreateDelegate(typeof(Action<Building_TurretGun>), null, AccessTools.Method(typeof(Building_TurretGun), "BurstComplete", null, null));

		// Token: 0x04000087 RID: 135
		public static Action<Building_TurretGun> Building_TurretGun_ResetCurrentTarget = (Action<Building_TurretGun>)Delegate.CreateDelegate(typeof(Action<Building_TurretGun>), null, AccessTools.Method(typeof(Building_TurretGun), "ResetCurrentTarget", null, null));

		// Token: 0x04000088 RID: 136
		public static Action<Building_TurretGun> Building_TurretGun_ResetForcedTarget = (Action<Building_TurretGun>)Delegate.CreateDelegate(typeof(Action<Building_TurretGun>), null, AccessTools.Method(typeof(Building_TurretGun), "ResetForcedTarget", null, null));

		// Token: 0x04000089 RID: 137
		public static Action<DefeatAllEnemiesQuestComp> DefeatAllEnemiesQuestComp_GiveRewardsAndSendLetter = (Action<DefeatAllEnemiesQuestComp>)Delegate.CreateDelegate(typeof(Action<DefeatAllEnemiesQuestComp>), null, AccessTools.Method(typeof(DefeatAllEnemiesQuestComp), "GiveRewardsAndSendLetter", null, null));

		// Token: 0x0400008A RID: 138
		public static Func<Explosion, IntVec3, int> Explosion_GetCellAffectTick = (Func<Explosion, IntVec3, int>)Delegate.CreateDelegate(typeof(Func<Explosion, IntVec3, int>), null, AccessTools.Method(typeof(Explosion), "GetCellAffectTick", null, null));

		// Token: 0x0400008B RID: 139
		public static Action<Projectile> Projectile_ImpactSomething = (Action<Projectile>)Delegate.CreateDelegate(typeof(Action<Projectile>), null, AccessTools.Method(typeof(Projectile), "ImpactSomething", null, null));
	}
}
