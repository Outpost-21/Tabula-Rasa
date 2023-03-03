using Verse;

namespace HRF
{
    public static class HRFLog
	{
		[TweakValue("0HRF")] public static bool debug = false;
		public static void Message(string message)
		{
			if (debug) Verse.Log.Message(message);
		}

        public static void Error(string message)
        {
            if (debug) Verse.Log.Error(message);
        }
    }
}