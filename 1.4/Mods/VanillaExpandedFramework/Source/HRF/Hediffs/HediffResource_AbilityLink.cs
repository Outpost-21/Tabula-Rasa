namespace HRF
{
    public class HediffResource_AbilityLink : HediffResource
    {
        public override bool ShouldRemove
        {
            get
            {
                if (this.ability.pawn is null)
                {
                    return true;
                }
                var compAbilities = this.ability.Comp;
                if (compAbilities is null || compAbilities.LearnedAbilities.Contains(this.ability) is false)
                {
                    return true;
                }
                return base.ShouldRemove;
            }
        }
    }
}
