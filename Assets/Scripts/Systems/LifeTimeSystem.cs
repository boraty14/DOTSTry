using Components;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;

namespace Systems
{
    public class LifeTimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            
            Entities.ForEach((Entity entity,int entityInQueryIndex, ref LifeTime lifeTime) =>
            {
                lifeTime.Value -= deltaTime;
                if (lifeTime.Value <= 0f)
                {
                    ecb.DestroyEntity(entityInQueryIndex,entity);
                }
            }).ScheduleParallel();
            _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
