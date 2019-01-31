namespace ImperialStudio.Core.Entities
{
    public class DamageSource
    {
        public IEntity DamageDealer { get; set; }

        public DamageType DamageType { get; set; }
    }
}