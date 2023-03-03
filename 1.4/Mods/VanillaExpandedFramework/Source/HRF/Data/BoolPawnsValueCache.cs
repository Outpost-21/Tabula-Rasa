using System.Collections.Generic;
using System.Threading.Tasks;
using Verse;

namespace HRF
{

	public class BoolPawnsValueCache
	{
		public BoolPawnsValueCache(bool value, IEnumerable<Pawn> pawns)
		{
			this.value = value;
			this.pawns = pawns;
		}
		public bool value;

		public IEnumerable<Pawn> pawns;
		public bool Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				updateTick = Find.TickManager.TicksGame;
			}
		}
		public int updateTick;
	}
}