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
    [UpdateAfter(typeof(SpawnPlanetSystem))]
    public partial struct SpawnMeteorSystem : ISystem
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
            
            var spaceEntity = SystemAPI.GetSingletonEntity<SpaceTag>();
            var spaceProperties = SystemAPI.GetComponent<CSpaceProperties>(spaceEntity);
            
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            
            new SpawnMeteorJob
            {
                ECB = ecb,
                Amount = spaceProperties.MeteorAmount,
            }.Run();
        }
    }
    
    [BurstCompile]
    [UpdateAfter(typeof(SpawnPlanetJob))]
    public partial struct SpawnMeteorJob : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public int Amount;

        [BurstCompile]
        private void Execute(SpaceAspect space)
        {
            for (var i = 0; i < Amount; i++)
            {
                const SpaceAspect.SpawnObject celestialType = SpaceAspect.SpawnObject.Meteor;
                var entity = space.SpawnEntity(ref ECB, celestialType);
                
                var newTransform = new LocalTransform()
                {
                    Position = space.GetRandomPositionInSphere(float3.zero, celestialType),
                    Rotation = quaternion.identity,
                    Scale = space.GetRandomSize(celestialType)
                };

                var velocity = space.GetRandomVelocity();
                
                ECB.SetComponent(entity, newTransform);
                ECB.SetComponent(entity, new CVelocity() {Value = velocity});
                ECB.SetComponent(entity, new CMass() {Value = (1e+3f * space.GetRandomMass(celestialType)) });
            }
        }
    }
}