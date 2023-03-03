using Verse;

namespace HRF
{
    public class FloatValueCache
	{
		public FloatValueCache(float value)
		{
			this.value = value;
		}
		private float value;
		public float Value
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
