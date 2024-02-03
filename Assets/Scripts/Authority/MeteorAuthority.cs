using Components;
using Tags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authority
{
    public class MeteorAuthority : MonoBehaviour
    {
        [field: SerializeField] public float Mass;
        [field: SerializeField] public Vector3 InitVelocity;
    }
    
    public class MeteorBaker : Baker<MeteorAuthority>
    {
        public override void Bake(MeteorAuthority authoring)
        {
            var meteorEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(meteorEntity, new CMass() { Value = authoring.Mass });
            AddComponent(meteorEntity, new CVelocity() { Value = new double3(authoring.InitVelocity) });
            AddComponent(meteorEntity, new CRemove() { Value = false });
            AddComponent<MeteorTag>(meteorEntity);
        }
    }
}