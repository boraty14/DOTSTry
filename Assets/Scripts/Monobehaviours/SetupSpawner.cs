using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monobehaviours
{
    public class SetupSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int gridSize;
        [SerializeField] private int spread;
        [SerializeField] private Vector2 speedRange;

        private BlobAssetStore _blob;

        private void Start()
        {
            _blob = new BlobAssetStore();
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,_blob);
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(personPrefab, settings);
            var entityManager =  World.DefaultGameObjectInjectionWorld.EntityManager;
            

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    var instance = entityManager.Instantiate(entity);

                    float3 position = new float3(x * spread, 0f, z* spread);
                    entityManager.SetComponentData(instance, new Translation{Value = position});
                    entityManager.SetComponentData(instance, new Destination{Value = position});
                    float speed = Random.Range(speedRange.x, speedRange.y);
                    entityManager.SetComponentData(instance, 
                        new MovementSpeed{Value = speed});
                }
            }
        }

        private void OnDestroy()
        {
            _blob.Dispose();
        }
    }
}
