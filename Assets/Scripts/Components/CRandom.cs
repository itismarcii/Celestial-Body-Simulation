using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CRandom : IComponentData
    {
        public Random Random;
    }
}