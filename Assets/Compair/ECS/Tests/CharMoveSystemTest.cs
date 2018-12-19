using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharMoveSystemTest : ECSTestBase
{
    
    [Test]
    public void Speed1_Move()
    {
        var entity = new GameObject();
        entity.AddComponent<GameObjectEntity>();
        entity.AddComponent<CharWrapper>();
        entity.AddComponent<PositionHybrid>();
        GameObjectEntity.AddToEntityManager(_entityManager, entity);
            
        // act
        var system = _world.GetOrCreateManager<CharMoveSystem>();
        
        system.Update();
        _entityManager.CompleteAllJobs();
        
        // assert
        //确实进入到system的监视列表中
        Assert.AreEqual(1, system.CharGroup.CalculateLength());
        
        //手动创建的gameobject与由他产生的entity确实是关联的，
        Assert.AreSame(entity.GetComponent<PositionHybrid>(),
            system.CharGroup.GetComponentArray<PositionHybrid>()[0]);
        Assert.AreEqual(new float3(1,1,1), entity.GetComponent<PositionHybrid>().Value);
        Assert.AreEqual(new float3(1,1,1),
            system.CharGroup.GetComponentArray<PositionHybrid>()[0].Value);
    }
}
