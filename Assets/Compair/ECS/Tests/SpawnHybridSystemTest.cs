using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnHybridSystemTest : ECSTestBase
{
    private NativeArray<Entity> _entities;

    [Test]
    public void Spawn1Object()
    {
        SpawnHybridSystem system = _world.GetOrCreateManager<SpawnHybridSystem>();
        
        system.Update();

        //由混合系统创建的GameObject由于具有GameObjectEntity组件
        //确实会自动被加入到EntityManager的监视中
        _entities = _entityManager.GetAllEntities();
        Assert.AreEqual(1, _entities.Length);

        var record = _entityManager.GetComponentData<Record>(_entities[0]);
        Assert.AreEqual(3, record.Value);
    }

    [TearDown]
    public override void TearDown()
    {
        _entities.Dispose();
        base.TearDown();
    }
}
