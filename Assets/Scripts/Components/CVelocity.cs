using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CVelocity : IComponentData
    {
        public double3 Value;
    }
}