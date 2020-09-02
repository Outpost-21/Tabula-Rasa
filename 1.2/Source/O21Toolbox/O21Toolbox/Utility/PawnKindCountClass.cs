using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
	public sealed class PawnKindCountClass : IExposable
	{

		public PawnKindDef pawnKind;

		public int count;

		public string Summary
		{
			get
			{
				return this.count + "x " + ((this.pawnKind != null) ? this.pawnKind.label : "null");
			}
		}

		public PawnKindCountClass()
		{
		}

		public PawnKindCountClass(PawnKindDef pawnKindDef, int count)
		{
			if (count < 0)
			{
				Log.Warning(string.Concat(new object[]
				{
				"Tried to set PawnKindCountClass count to ",
				count,
				". pawnKindDef=",
				pawnKindDef
				}), false);
				count = 0;
			}
			this.pawnKind = pawnKindDef;
			this.count = count;
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<PawnKindDef>(ref this.pawnKind, "pawnKindDef");
			Scribe_Values.Look<int>(ref this.count, "count", 1, false);
		}

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			if (xmlRoot.ChildNodes.Count != 1)
			{
				Log.Error("Misconfigured PawnKindCountClass: " + xmlRoot.OuterXml, false);
				return;
			}
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "pawnKindDef", xmlRoot.Name, null, null);
			this.count = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
			"(",
			this.count,
			"x ",
			(this.pawnKind != null) ? this.pawnKind.defName : "null",
			")"
			});
		}

		public override int GetHashCode()
		{
			return (int)this.pawnKind.shortHash + this.count << 16;
		}

		public static implicit operator PawnKindCountClass(PawnKindCount t)
		{
			return new PawnKindCountClass(t.pawnKind, t.Count);
		}
	}
}
