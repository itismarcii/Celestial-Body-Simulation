using Components;
using Tags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authority
{
    public class PlanetAuthority : MonoBehaviour
    {
        [field: SerializeField] public float Mass { get; private set; }
        [field: SerializeField] public Vector3 RotationSpeed { get; private set; }
    }
    
    public partial class PlanetBaker : Baker<PlanetAuthority>
    {
        public override void Bake(PlanetAuthority authoring)
        {
            var planetEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(planetEntity, new CMass { Value = authoring.Mass });
            AddComponent(planetEntity, new CRotationSpeed() { Value = new double3(authoring.RotationSpeed) });
            AddComponent<PlanetTag>(planetEntity);
        }
    }

}
