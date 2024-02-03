using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct SpaceAspect : IAspect
    {
        public enum SpawnObject
        {
            Planet,
            Meteor
        }
        
        public readonly Entity Entity;

        private readonly RefRO<LocalTransform> _Transform;
        private readonly RefRW<CRandom> _Random;
        private readonly RefRO<CSpaceProperties> _SpaceProperties;
        
        private LocalTransform Transform => _Transform.ValueRO;

        #region Planet

        private float2 PlanetDimensionRange => _SpaceProperties.ValueRO.PlanetSpawnDimension;
        private float2 PlanetMassRange => _SpaceProperties.ValueRO.PlanetMassRange;
        private float2 PlanetSizeRange => _SpaceProperties.ValueRO.PlanetSizeRange;
        private float3x2 RotationSpeedRange => _SpaceProperties.ValueRO.PlanetRotationSpeed;

        #endregion

        #region Meteor

        private float2 MeteorDimensionRange => _SpaceProperties.ValueRO.MeteorSpawnDimension;
        private float2 MeteorMassRange => _SpaceProperties.ValueRO.MeteorMassRange;
        private float2 MeteorSizeRange => _SpaceProperties.ValueRO.MeteorSizeRange;
        private float3x2 MeteorVelocityRange => _SpaceProperties.ValueRO.MeteorVelocity;

        #endregion
        


        
        
        public Entity SpawnEntity(ref EntityCommandBuffer commandBuffer, SpawnObject spawnObject)
        {
            return spawnObject switch
            {
                SpawnObject.Planet => commandBuffer.Instantiate(_SpaceProperties.ValueRO.PlanetPrefab),
                SpawnObject.Meteor => commandBuffer.Instantiate(_SpaceProperties.ValueRO.MeteorPrefab),
                _ => throw new ArgumentOutOfRangeException(nameof(spawnObject), spawnObject, null)
            };
        }

        public float3 GetRandomPositionInSphere(float3 origin, SpawnObject spawnObject)
        {
            var theta = _Random.ValueRW.Random.NextFloat(0, 2f * math.PI);
            var phi = _Random.ValueRW.Random.NextFloat(0, math.PI); // Use half the range for phi to stay in the upper hemisphere

            var radius = spawnObject switch
            {
                SpawnObject.Planet => _Random.ValueRW.Random.NextFloat(PlanetDimensionRange.x, PlanetDimensionRange.y),
                SpawnObject.Meteor => _Random.ValueRW.Random.NextFloat(MeteorDimensionRange.x, MeteorDimensionRange.y),
                _ => throw new ArgumentOutOfRangeException(nameof(spawnObject), spawnObject, null)
            };

            var x = radius * math.sin(phi) * math.cos(theta);
            var y = radius * math.cos(phi);
            var z = radius * math.sin(phi) * math.sin(theta);

            return new float3(x, y, z);

        }

        public float GetRandomSize(SpawnObject spawnObject)
        {
            return spawnObject switch
            {
                SpawnObject.Planet => _Random.ValueRW.Random.NextFloat(PlanetSizeRange.x, PlanetSizeRange.y),
                SpawnObject.Meteor => _Random.ValueRW.Random.NextFloat(MeteorSizeRange.x, MeteorSizeRange.y),
                _ => throw new ArgumentOutOfRangeException(nameof(spawnObject), spawnObject, null)
            };
        }

        public float GetRandomMass(SpawnObject spawnObject)
        {
            return spawnObject switch
            {
                SpawnObject.Planet => _Random.ValueRW.Random.NextFloat(PlanetMassRange.x, PlanetMassRange.y),
                SpawnObject.Meteor => _Random.ValueRW.Random.NextFloat(MeteorMassRange.x, MeteorMassRange.y),
                _ => throw new ArgumentOutOfRangeException(nameof(spawnObject), spawnObject, null)
            };
        }
            

        public float3 GetRandomVelocity() =>
            _Random.ValueRW.Random.NextFloat3(MeteorVelocityRange.c0, MeteorVelocityRange.c1);

        public double3 GetRandomRotationSpeed() =>
            _Random.ValueRW.Random.NextFloat3(RotationSpeedRange.c0, RotationSpeedRange.c1);
    }
}
