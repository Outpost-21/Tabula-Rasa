namespace HRF
{
    public class HealingProperties : GeneralProperties
    {
        public float healPoints;
        public int hediffsToHeal;
        public bool affectPermanent;
        public bool affectChronic;
        public bool affectIllness;
        public bool affectInjuries;
        public bool pointsOverflow;
        public HealPriority healPriority;
        public bool affectMechanical;
        public bool affectOrganic = true;
        public bool affectsAllies = true;
        public bool affectsEnemies;
        public bool healOnApply;
    }
}