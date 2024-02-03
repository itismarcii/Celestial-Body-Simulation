╔═══════════════════════════╗
║    README INSTRUCTIONS    ║
╚═══════════════════════════╝

1. Select the "Space" entity in the EntitySubspace inside the MainScene.
2. On the GameObject, find the "SpaceAuthority" script.

╔════════════════════════════╗
║   SpaceAuthority Script    ║
╚════════════════════════════╝

// Seed //
- The random input seed.

// Planet Header //
- Prefab: Holds the EntityComponent "PlanetAuthority," including Mass and RotationSpeed components (overridden later). Contains MeshFilter and MeshRenderer.
- Spawn Amount: Number of planets to be spawned (consider reducing for more meteors).
- Spawn Radius: Random radius from the origin (0,0,0) for planet spawn.
- Mass Range: Min and Max values for a random planet, later multiplied by 12e+6f for gravitational force.
- Size Range: Min and Max values for the default planet size, influenced by planet mass.
- Min Rotation: Minimum rotation for all three axes.
- Max Rotation: Maximum rotation for all three axes.

// Meteor Header //
- Prefab: Holds the EntityComponent "MeteorAuthority," including Mass and Initial Velocity at spawn (overridden later). Contains MeshFilter and MeshRenderer.
- Spawn Amount: Number of meteors to be spawned.
- Spawn Radius: Random radius from the origin (0,0,0) for meteor spawn.
- Mass Range: Min and Max values for a random meteor.
- Size Range: Min and Max values for the default meteor size.
- Spawn Min Velocity: Minimum initial velocity for a meteor in all three axes.
- Spawn Max Velocity: Maximum initial velocity for a meteor in all three axes.

╔══════════════════════════╗
║    Additional Note       ║
╚══════════════════════════╝

- The camera may need readjustment based on the planet spawn radius. Use the "Align with view" shortcut on the MainCamera inside the Scene for best results.
