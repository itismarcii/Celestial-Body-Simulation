using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public struct PlanetData
{
    public NativeList<LocalTransform> Transforms;
    public NativeList<CMass> Masses;
    public NativeList<CRotationSpeed> RotationSpeeds;
        
    public static JobHandle InitPlanetData(EntityQuery query, ref PlanetData planetData)
    {
        planetData.Transforms =
            query.ToComponentDataListAsync<LocalTransform>(Allocator.TempJob, out var jobHandleTransforms);
        planetData.Masses = 
            query.ToComponentDataListAsync<CMass>(Allocator.TempJob, out var jobHandleMasses);
        planetData.RotationSpeeds =
            query.ToComponentDataListAsync<CRotationSpeed>(Allocator.TempJob, out var jobHandleRotationSpeeds);
        
        return JobHandle.CombineDependencies(jobHandleTransforms, jobHandleMasses, jobHandleRotationSpeeds);
    }
}