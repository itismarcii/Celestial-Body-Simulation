using Aspects;
using Components;
using Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace Systems
{
    [BurstCompile, UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(SpawnMeteorSystem))]
    public partial struct SpawnPlanetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SpaceTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var spaceEntity = SystemAPI.GetSingletonEntity<SpaceTag>();
            var spaceProperties = SystemAPI.GetComponentRO<CSpaceProperties>(spaceEntity).ValueRO;
            
            new SpawnPlanetJob
            {
                ECB = ecb,
                Amount = spaceProperties.PlanetAmount,
                Prefab = spaceProperties.PlanetPrefab,
                
            }.Run();
        }
    }

    [BurstCompile, WithAny(typeof(SpaceTag))]
    [UpdateBefore(typeof(SpawnMeteorJob))]
    public partial struct SpawnPlanetJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public int Amount;
        public Entity Prefab;

        [BurstCompile]
        private void Execute(SpaceAspect space)
        {
            for (var i = 0; i < Amount; i++)
            {
                const SpaceAspect.SpawnObject celestialType = SpaceAspect.SpawnObject.Planet;
                
                var planetEntity = space.SpawnEntity(ref ECB, celestialType);
                var mass = space.GetRandomMass(celestialType);
                var transform = new LocalTransform()
                {
                    Position = space.GetRandomPositionInSphere(float3.zero, celestialType),
                    Scale =  space.GetRandomSize(celestialType) * math.pow(mass, 1.25f)
                };
                
                ECB.SetComponent(planetEntity, transform);               
                ECB.SetComponent(planetEntity, new CMass() {Value = (12e+6f * mass) });
                ECB.SetComponent(planetEntity, new CRotationSpeed() { Value = (space.GetRandomRotationSpeed()) });

            }
        }
    }
}