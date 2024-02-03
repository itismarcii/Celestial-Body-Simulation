using System;
using Components;
using Tags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Authority
{
    public class SpaceAuthority : MonoBehaviour
    {
        [Serializable]
        public struct CelestialBody
        {
            [field: SerializeField] public GameObject Prefab { get; private set; }
            [field: SerializeField] public int SpawnAmount { get; private set; }
            [field: SerializeField] public Vector2 SpawnRadius { get; private set; }
            [field: SerializeField, Tooltip("Planets have a times 12e+6f modifier")] public Vector2 MassRange { get; private set; }
            [field: SerializeField, Tooltip("Planets sizes are influenced by the mass")] public Vector2 SizeRange { get; private set; }
        }
        
        [field: SerializeField] public uint Seed { get; private set; }

        [field: Header ("Planet")] 
        [field: SerializeField] public CelestialBody Planet { get; private set; }
        [field: SerializeField, Tooltip("Modified by times 1e-4f")] public Vector3 MinRotation { get; private set; }
        [field: SerializeField, Tooltip("Modified by times 1e-4f")] public Vector3 MaxRotation { get; private set; } 
        
        [field: Header ("Meteor")] 
        [field: SerializeField] public CelestialBody Meteor { get; private set; }
        [field: SerializeField] public Vector3 SpawnMinVelocity { get; private set; }
        [field: SerializeField] public Vector3 SpawnMaxVelocity { get; private set; }
    }

    public class SpaceBaker : Baker<SpaceAuthority>
    {
        public override void Bake(SpaceAuthority authoring)
        {
            var spaceEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(spaceEntity, new CSpaceProperties()
            {
                PlanetPrefab = GetEntity(authoring.Planet.Prefab,TransformUsageFlags.Dynamic),
                PlanetAmount = authoring.Planet.SpawnAmount,
                PlanetSpawnDimension = authoring.Planet.SpawnRadius,
                PlanetMassRange = authoring.Planet.MassRange,
                PlanetSizeRange = authoring.Planet.SizeRange,
                PlanetRotationSpeed = new float3x2(authoring.MinRotation, authoring.MaxRotation) * 1e-4f,
                
                MeteorPrefab = GetEntity(authoring.Meteor.Prefab, TransformUsageFlags.Dynamic),
                MeteorAmount = authoring.Meteor.SpawnAmount,
                MeteorSpawnDimension = authoring.Meteor.SpawnRadius,
                MeteorMassRange = authoring.Meteor.MassRange,
                MeteorSizeRange = authoring.Meteor.SizeRange,
                MeteorVelocity = new float3x2(authoring.SpawnMinVelocity, authoring.SpawnMaxVelocity),
            });

            AddComponent(spaceEntity, new CRandom()
            {
                Random = Random.CreateFromIndex(authoring.Seed)
            });
            
            AddComponent<SpaceTag>(spaceEntity);
        }
    }
}