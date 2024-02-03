using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CSpaceProperties : IComponentData
    {
        #region Planet

        public Entity PlanetPrefab;
        public int PlanetAmount;
        public float2 PlanetSpawnDimension;
        public float2 PlanetMassRange;
        public float2 PlanetSizeRange;
        public float3x2 PlanetRotationSpeed;

        #endregion
        
        #region Meteor

        public Entity MeteorPrefab;
        public int MeteorAmount;
        public float2 MeteorSpawnDimension;
        public float2 MeteorMassRange;
        public float2 MeteorSizeRange;
        public float3x2 MeteorVelocity;

        #endregion

    }
}