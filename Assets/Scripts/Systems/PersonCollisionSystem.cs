using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public class PersonCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        struct PersonCollisionJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<PersonTag> PersonGroup;
            public ComponentDataFromEntity<URPMaterialPropertyBaseColor> ColorGroup;
            public float Seed;

            public void Execute(TriggerEvent triggerEvent)
            {
                bool isEntityAPerson = PersonGroup.HasComponent(triggerEvent.EntityA);
                bool isEntityBPerson = PersonGroup.HasComponent(triggerEvent.EntityB);

                if (!isEntityAPerson || !isEntityBPerson) return;
                float newRandom = (1f + Seed) + (triggerEvent.BodyIndexA * triggerEvent.BodyIndexB);
                if (newRandom <= 0.001f)
                {
                    Debug.Log(1f + Seed);
                    Debug.Log(triggerEvent.BodyIndexA * triggerEvent.BodyIndexB);
                }
                var random =
                    new Random((uint)(newRandom));
                
                random = ChangeMaterialColor(random,triggerEvent.EntityA);
                random = ChangeMaterialColor(random,triggerEvent.EntityB);
            }

            private Random ChangeMaterialColor(Random random, Entity entity)
            {
                if (ColorGroup.HasComponent(entity))
                {
                    var colorComponent = ColorGroup[entity];
                    colorComponent.Value.x = random.NextFloat(0f, 1f);
                    colorComponent.Value.y = random.NextFloat(0f, 1f);
                    colorComponent.Value.z = random.NextFloat(0f, 1f);
                    ColorGroup[entity] = colorComponent;
                }

                return random;
            }
        }

        protected override void OnUpdate()
        {
            Dependency = new PersonCollisionJob()
            {
                PersonGroup = GetComponentDataFromEntity<PersonTag>(true),
                ColorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
                Seed = System.DateTimeOffset.Now.Millisecond
            }.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, Dependency);
        }
    }
}