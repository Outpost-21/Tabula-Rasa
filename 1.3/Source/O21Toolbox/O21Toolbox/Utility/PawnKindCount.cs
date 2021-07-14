using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public struct PawnKindCount : IEquatable<PawnKindCount>, IExposable
    {
        public PawnKindDef pawnKind;

        public int count;

		public PawnKindDef ThingDef
		{
			get
			{
				return this.pawnKind;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public PawnKindCount(PawnKindDef pawnKind, int count)
		{
			if (count < 0)
			{
				Log.Warning(string.Concat(new object[]
				{
					"Tried to set ThingDefCount count to ",
					count,
					". pawnKind=",
					pawnKind
				}));
				count = 0;
			}
			this.pawnKind = pawnKind;
			this.count = count;
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<PawnKindDef>(ref this.pawnKind, "pawnKind");
			Scribe_Values.Look<int>(ref this.count, "count", 1, false);
		}

		public PawnKindCount WithCount(int newCount)
		{
			return new PawnKindCount(this.pawnKind, newCount);
		}

		public override bool Equals(object obj)
		{
			return obj is PawnKindCount && this.Equals((PawnKindCount)obj);
		}

		public bool Equals(PawnKindCount other)
		{
			return this == other;
		}

		public static bool operator ==(PawnKindCount a, PawnKindCount b)
		{
			return a.pawnKind == b.pawnKind && a.count == b.count;
		}

		public static bool operator !=(PawnKindCount a, PawnKindCount b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return Gen.HashCombine<PawnKindDef>(this.count, this.pawnKind);
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

        public static implicit operator PawnKindCount(PawnKindCountClass t)
		{
			if (t == null)
			{
				return new PawnKindCount(null, 0);
			}
			return new PawnKindCount(t.pawnKind, t.count);
		}
	}
}
