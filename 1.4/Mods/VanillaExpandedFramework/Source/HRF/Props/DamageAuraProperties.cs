using Verse;

namespace HRF
{
    public class DamageAuraProperties : GeneralProperties
    {
        public DamageDef damageDef;
        public bool damageFromEquippedWeapon;
        public int damageAmount;
        public bool affectsAllies;
        public bool affectsEnemies = true;
        public ThingDef otherDamageMote;
        public ThingDef selfDamageMote;
        public GraphicData auraGraphic;
    }
}