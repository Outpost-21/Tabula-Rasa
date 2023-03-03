using UnityEngine;
using Verse;

namespace HRF
{
    [StaticConstructorOnStartup]
	public class Gizmo_ResourceStatus : Gizmo
	{
		private HediffResource hediffResource;

		private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
		public Gizmo_ResourceStatus(HediffResource hediffResource)
		{
			order = -100f;
			this.hediffResource = hediffResource;
			resourceStorageCapacityCache = new FloatValueCache(hediffResource.StoragesTotalCapacity);
			resourceCapacityCache = new FloatValueCache(hediffResource.ResourceCapacity);
			resourceAmountCache = new FloatValueCache(hediffResource.ResourceAmountNoStorages);
			resourceBatteryAmountCache = new FloatValueCache(hediffResource.ResourceFromStorages);
		}
		public override float GetWidth(float maxWidth)
		{
			return 140f;
		}

		private Texture2D fullShieldBarTexCache;
		private Texture2D FullShieldBarTex
        {
			get
            {
				if (fullShieldBarTexCache is null)
                {
					Color fullShieldBarColor;

					if (hediffResource.def.progressBarColor.HasValue)
					{
						fullShieldBarColor = hediffResource.def.progressBarColor.Value;
					}
					else
					{
						fullShieldBarColor = hediffResource.def.defaultLabelColor;
					}
					fullShieldBarTexCache = SolidColorMaterials.NewSolidColorTexture(fullShieldBarColor);
				}
				return fullShieldBarTexCache;
			}
        }

		private Color cachedBackgroundColor = Color.clear;
		private Color BackGroundColor
        {
			get
            {
				if (cachedBackgroundColor == Color.clear)
                {
					cachedBackgroundColor = hediffResource.def.backgroundBarColor.HasValue ? hediffResource.def.backgroundBarColor.Value : Widgets.WindowBGFillColor; ;
				}
				return cachedBackgroundColor;
            }
        }

		private Color cachedDrawBox = Color.clear;
		private Color DrawBoxColor
        {
			get
            {
				if (cachedDrawBox == Color.clear)
                {
					cachedDrawBox = new ColorInt(97, 108, 122).ToColor;
				}
				return cachedDrawBox;
            }
        }

		public FloatValueCache resourceStorageCapacityCache;
		public FloatValueCache resourceCapacityCache;
		public FloatValueCache resourceAmountCache;
		public FloatValueCache resourceBatteryAmountCache;

		public static bool updateNow;

		[TweakValue("0ART", 0, 35)] public static float yTest = 23;
		[TweakValue("0ART", 0, 35)] public static float yTest2 = 17;
		[TweakValue("0ART", 0, 35)] public static float yTest3 = 25;
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
			Rect contractedBox = rect.ContractedBy(6f);
			DrawWindowBackground(rect, hediffResource.def);
			Rect rect3 = contractedBox;
			rect3.height = rect.height / 2f;
			Text.Font = GameFont.Tiny;
			var label = hediffResource.def.LabelCap;
			if (hediffResource.def.lifetimeTicks != -1)
			{
				label += " (" + Mathf.CeilToInt((hediffResource.def.lifetimeTicks - hediffResource.duration).TicksToSeconds()) + "s)";
			}
			Widgets.Label(rect3, label);

			if (Find.TickManager.TicksGame > resourceCapacityCache.updateTick + 30 || updateNow)
			{
				resourceCapacityCache.Value = hediffResource.ResourceCapacity;
				resourceAmountCache.Value = hediffResource.ResourceAmountNoStorages;
				resourceBatteryAmountCache.Value = hediffResource.ResourceFromStorages;
				resourceStorageCapacityCache.Value = hediffResource.StoragesTotalCapacity;
				updateNow = false;
			}

			var resourceAmount = resourceAmountCache.Value;
			var resourceCapacity = resourceCapacityCache.Value;
			var resourceStorage = resourceBatteryAmountCache.Value;
			var resourceStorageCapacity = resourceStorageCapacityCache.Value;

			Rect resourceAmountBar = contractedBox;
			resourceAmountBar.height = yTest;
			float fillPercent;
			Text.Anchor = TextAnchor.MiddleCenter;

			if (resourceStorageCapacity > 0)
			{
				resourceAmountBar.y += yTest2;
				fillPercent = resourceStorage / resourceStorageCapacity;
				Widgets.FillableBar(resourceAmountBar, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: true);
				if (hediffResource.def.resourceBarTextColor.HasValue)
				{
					GUI.color = hediffResource.def.resourceBarTextColor.Value;
				}
				Rect labelRect = new Rect(resourceAmountBar.x, resourceAmountBar.y, resourceAmountBar.width, resourceAmountBar.height);
				Widgets.Label(labelRect, "ART.Reserve".Translate() + (resourceStorage).ToString("F0") + " / " + (resourceStorageCapacity).ToString("F0"));
			}

			if (resourceCapacity > 0)
            {
				resourceAmountBar.y += yTest3;
				fillPercent = resourceAmount / resourceCapacity;
				Widgets.FillableBar(resourceAmountBar, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: true);
				if (hediffResource.def.resourceBarTextColor.HasValue)
				{
					GUI.color = hediffResource.def.resourceBarTextColor.Value;
				}
				Rect labelRect = new Rect(resourceAmountBar.x, resourceAmountBar.y, resourceAmountBar.width, resourceAmountBar.height);
				Widgets.Label(labelRect, (resourceAmount).ToString("F0") + " / " + (resourceCapacity).ToString("F0"));
			}

			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
			return new GizmoResult(GizmoState.Clear);
		}

		public void DrawWindowBackground(Rect rect, HediffResourceDef hediffResourceDef)
		{
			GUI.color = BackGroundColor;
			GUI.DrawTexture(rect, BaseContent.WhiteTex);
			GUI.color = DrawBoxColor;
			Widgets.DrawBox(rect);
			GUI.color = Color.white;
		}
	}
}
