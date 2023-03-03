using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class GeneralProperties
    {
        public int ticksPerEffect;
        public float effectRadius = -1f;
        public bool worksThroughWalls;
        public List<string> blacklistTradeTags;
        public List<string> whitelistTradeTags;
        public bool affectsSelf;
        public SoundDef soundOnEffect;
    }
}