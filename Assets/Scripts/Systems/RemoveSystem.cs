using Components;
using Unity.Entities;

namespace Systems
{
    public partial class RemoveSystem : SystemBase
    {
        protected override void OnUpdate()
        {

            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);
            
            Entities.ForEach((Entity entity, CRemove remove) =>
            {
                if(remove.Value) ecb.DestroyEntity(entity);
            }).Run();
        }
    }
}