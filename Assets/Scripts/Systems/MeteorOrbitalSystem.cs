using Aspects;
using Components;
using Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile, UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MeteorOrbitalSystem : ISystem
    {
        private bool _IsSetup;
        private Entity _SpaceEntity;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SpaceTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!_IsSetup)
            {
                _SpaceEntity = SystemAPI.GetSingletonEntity<SpaceTag>();
            }

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var deltaTime = SystemAPI.Time.DeltaTime;
            var planetQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, CMass, CRotationSpeed, PlanetTag>()
                .Build();
            
            for (var i = 0; i < 1; i++)
            {
                var planetData = new PlanetData();
                var planetJobHandle = PlanetData.InitPlanetData(planetQuery, ref planetData);
                    
                planetJobHandle.Complete();
                if(!planetJobHandle.IsCompleted) return;
                
                new MeteorOrbitalJob()
                {
                    PlanetData = planetData,
                    DeltaTime = deltaTime
                }.ScheduleParallel(planetJobHandle).Complete();
            }
        }

        [BurstCompile, WithAll(typeof(MeteorTag))]
        public partial struct MeteorOrbitalJob : IJobEntity
        {
            [ReadOnly] public PlanetData PlanetData;
            public float DeltaTime;
            
            [BurstCompile]
            public void Execute(MeteorOrbitalAspect meteor)
            {
                var allMaxDistance = true;
                
                for (var i = 0; i < PlanetData.Transforms.Length; i++)
                {
                    var planet = new PlanetOrbitalAspect(
                        new RefRO<CMass>(PlanetData.Masses.ToArray(Allocator.Temp), i),
                        new RefRO<CRotationSpeed>(PlanetData.RotationSpeeds.ToArray(Allocator.Temp), i),
                        new RefRO<LocalTransform>(PlanetData.Transforms.ToArray(Allocator.Temp), i));

                    if (planet.CheckSurfaceCollision(meteor))
                    {
                        meteor.RemoveTag.ValueRW.Value = true;
                        return;
                    }
                    
                    if (planet.CheckMaxDistance(meteor)) continue;
                    
                    allMaxDistance = false;
                    planet.Update(meteor, DeltaTime);
                }
                
                if(allMaxDistance) meteor.RemoveTag.ValueRW.Value = true;
            }
        }
    }
}
