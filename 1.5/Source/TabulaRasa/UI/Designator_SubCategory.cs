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
    public class Designator_SubCategory : Designator
    {
		public Designator_SubCategory()
		{
			SetDefaultGizmoData();
			defaultDesc = "TabulaRasa.SubCatDesc".Translate();
			soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
			soundDragChanged = null;
			soundSucceeded = SoundDefOf.Designate_ZoneAdd;

			soundDragSustain = SoundDefOf.Designate_DragStandard;
			soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			useMouseIcon = true;
			order = -100f;
		}

		public void SetDefaultGizmoData()
		{
			defaultLabel = "TabulaRasa.SubCatLabel".Translate();
			icon = ContentFinder<Texture2D>.Get("Toolbox/UI/CategoryNone");
		}

		public void UpdateGizmoData()
		{
			SetDefaultGizmoData();
			if (CurrentCategory != null && WorldComp_ArchitectSubCategory.SelectedSubCategory.ContainsKey(CurrentCategory))
			{
				defaultLabel = WorldComp_ArchitectSubCategory.SelectedSubCategory[CurrentCategory].LabelCap;
				if (!WorldComp_ArchitectSubCategory.SelectedSubCategory[CurrentCategory].iconPath.NullOrEmpty())
				{
					icon = WorldComp_ArchitectSubCategory.SelectedSubCategory[CurrentCategory].Icon;
				}
			}
		}

        public override void ProcessInput(Event ev)
		{
			if (CheckCanInteract())
			{
				MakeFloatMenu(delegate (DesignatorSubCategoryDef def)
				{
					WorldComp_ArchitectSubCategory.SetSubCategoryForDesingationCat(CurrentCategory, def);
					UpdateGizmoData();
				});
			}
		}

		public DesignationCategoryDef CurrentCategory => Find.WindowStack.WindowOfType<MainTabWindow_Architect>().selectedDesPanel.def;

		public void MakeFloatMenu(Action<DesignatorSubCategoryDef> selAction)
        {
			List<FloatMenuOption> options = new List<FloatMenuOption>();
			options.Add(new FloatMenuOption("None".Translate(), delegate { selAction(null); }, MenuOptionPriority.High));
			List<DesignatorSubCategoryDef> subCatsAvailable = DefDatabase<DesignatorSubCategoryDef>.AllDefs.Where(sc => sc.designationCategory == CurrentCategory).ToList();
			subCatsAvailable.OrderBy(c => c.LabelCap);
            if (!subCatsAvailable.NullOrEmpty())
			{
				foreach (DesignatorSubCategoryDef subCat in subCatsAvailable)
				{
                    if (subCat.enabled)
                    {
						if (!subCat.iconPath.NullOrEmpty())
						{
							options.Add(new FloatMenuOption(subCat.LabelCap, delegate { selAction(subCat); }, subCat.Icon, Color.white));
						}
						else
						{
							options.Add(new FloatMenuOption(subCat.LabelCap, delegate { selAction(subCat); }));
						}
					}
				}
			}
			Find.WindowStack.Add(new FloatMenu(options));
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            return true;
        }
    }
}
