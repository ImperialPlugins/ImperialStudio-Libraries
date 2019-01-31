namespace ImperialStudio.Core.Entities
{
    public interface ILivingEntity : IEntity
    {
        float Health { get; set; }

        void Kill();

        void Damage(float damage, DamageSource damageSource);
    }
}