using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CRotationSpeed : IComponentData
    {
        public double3 Value;
    }
}