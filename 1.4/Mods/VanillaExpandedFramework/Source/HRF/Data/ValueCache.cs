using Verse;

namespace HRF
{
    public class ValueCache<T> 
	{
		public ValueCache(T value)
		{
			this.value = value;
		}
		public T value;

		public T Value
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