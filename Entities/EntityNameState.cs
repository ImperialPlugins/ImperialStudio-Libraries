using ImperialStudio.Core.Api.Entities;

namespace ImperialStudio.Core.Entities
{
    public class EntityNameState : StringState
    {
        private readonly IEntity m_Entity;

        public EntityNameState(IEntity entity)
        {
            m_Entity = entity;
        }
        protected override string GetCurrentValue()
        {
            return m_Entity.Name;
        }

        protected override void OnUpdateState(string oldValue, string newValue)
        {
            m_Entity.Name = newValue;
        }
    }
}