using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Aspects
{
    public readonly partial struct PlanetOrbitalAspect : IAspect
    { 
        private const double GRAVITY_CONSTANT = 6.67430e-11f; // Default 6.67430e-11f but for scaling reasons it is sized down
        private const double MAX_DISTANCE = 1e+11f;
        private const double MAX_ROTATION_SPEED = 1e-5f;
        private const double BETA = 1e-13f;
        private const double EPSILON = 1e-10f;

        private readonly RefRO<LocalTransform> _PlanetTransform;
        private readonly RefRO<CMass> _PlanetMass;
        private readonly RefRO<CRotationSpeed> _PlanetRotationSpeed;

        private LocalTransform Transform => _PlanetTransform.ValueRO;
        private float3 Position => Transform.Position;
        private double Mass => _PlanetMass.ValueRO.Value * 1e+12;
        private double3 RotationSpeed => _PlanetRotationSpeed.ValueRO.Value * EPSILON;
        
        private double3 GravitationalForce(in float3 position, in double mass, in double distanceSquared)
        {
            double3 direction = math.normalize(Position - position);
            var forceMagnitude = GRAVITY_CONSTANT * (Mass * mass) / distanceSquared;
            return direction * forceMagnitude;
        }
        
        private double3 CentrifugalForce(in float3 position, in double3 velocity, in double mass)
        {
            var radialVector = position - Position;
            var tangentialVector = math.cross(RotationSpeed, radialVector); // Use RotationSpeed in the tangential vector
            return mass * math.lengthsq(velocity) / math.length(radialVector) * tangentialVector;
        }

        private double3 CoriolisForce(in double3 velocity) => 2 * math.cross(RotationSpeed, velocity);

        private double3 Torque(in float3 position, in double mass, in quaternion meteorRotation)
        {
            var radialVector = position - Position;
            var tangentialVelocity = math.cross(RotationSpeed, radialVector); 
    
            var rotationAxis = math.rotate(meteorRotation, new float3(1, 0, 0)); 
    
            var lengthRotationAxis = math.length(rotationAxis);

            return (lengthRotationAxis > float.Epsilon) ? 
                math.cross(radialVector, tangentialVelocity) * mass * lengthRotationAxis :
                double3.zero;
        }
        
        private float CalculateDistance(float3 point0, float3 point1)
        {
            var dx = point1.x - point0.x;
            var dy = point1.y - point0.y;
            var dz = point1.z - point0.z;

            return math.abs(math.sqrt(dx * dx + dy * dy + dz * dz));
        }
        
        public bool CheckMaxDistance(MeteorOrbitalAspect meteor) => MAX_DISTANCE < CalculateDistance(Position, meteor.Position);
        public bool CheckSurfaceCollision(MeteorOrbitalAspect meteor) =>  (_PlanetTransform.ValueRO.Scale / 2) > CalculateDistance(Position, meteor.Position);
        
        public void Update(MeteorOrbitalAspect meteor, float deltaTime)
        {
            deltaTime *= .0685f;
            
            var distanceSquared = math.lengthsq(Position - meteor.Position);
            var gravityForce = GravitationalForce(meteor.Position, meteor.Mass, distanceSquared);

            if (math.length(gravityForce) >= BETA)
            {
                var centrifugalForce = CentrifugalForce(meteor.Position, meteor.Velocity, meteor.Mass);
                var coriolisForce = CoriolisForce(meteor.Mass);
                meteor.Velocity += (gravityForce / meteor.Mass - centrifugalForce - coriolisForce) * deltaTime;

                var torque = math.clamp((Torque(meteor.Position, meteor.Mass, meteor.Rotation)), - MAX_ROTATION_SPEED, MAX_ROTATION_SPEED);
                
                var deltaRotation = new quaternion(
                    (float) torque.x * deltaTime, 
                    (float) torque.y * deltaTime, 
                    (float) torque.z * deltaTime, 
                    0f);

                meteor.Rotation = math.normalize(math.mul(meteor.Rotation, deltaRotation));
            }

            meteor.Position += (float3)meteor.Velocity * deltaTime;
        }
    }
}
