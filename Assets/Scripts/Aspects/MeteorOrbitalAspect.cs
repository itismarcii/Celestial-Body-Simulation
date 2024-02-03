using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct MeteorOrbitalAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> MeteorTransform;
        private readonly RefRO<CMass> MeteorMass;
        public readonly RefRW<CVelocity> MeteorVelocity;
        public readonly RefRW<CRemove> RemoveTag;

        public LocalTransform Transform => MeteorTransform.ValueRO;

        public float3 Position
        {
            get => Transform.Position;
            set => MeteorTransform.ValueRW = new LocalTransform()
            {
                Position = value,
                Rotation = Transform.Rotation,
                Scale = Transform.Scale
            };
        }
        
        public quaternion Rotation
        {
            get => Transform.Rotation;
            set => MeteorTransform.ValueRW = new LocalTransform()
            {
                Position = Transform.Position,
                Rotation = value,
                Scale = Transform.Scale
            };
        }
        
        
        public double Mass => MeteorMass.ValueRO.Value;

        public double3 Velocity
        {
            get => MeteorVelocity.ValueRO.Value;
            set => MeteorVelocity.ValueRW.Value = value;
        }
        
        
    }
}