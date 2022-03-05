using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct LifeTime : IComponentData
    {
        public float Value;
    }
}
