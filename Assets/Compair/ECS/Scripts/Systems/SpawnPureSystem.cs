/*
 * 纯粹为了学习测试系统写的Pure ECS
 */

using Unity.Entities;
using Unity.Jobs;

public struct Empty : IComponentData
{
    
}

public class SpawnPureSystem : ComponentSystem
{
    
    protected override void OnUpdate()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var entity = entityManager.CreateEntity();
        entityManager.AddComponentData(entity, new Empty());
    }
}