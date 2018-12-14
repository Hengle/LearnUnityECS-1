using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnPureSystemTest : ECSTestBase
{
    
    [Test]
    public void Spawn1Entity()
    {
        SpawnPureSystem ss = World.GetOrCreateManager<SpawnPureSystem>();
        
        ss.Update();

        var entities = entityManager.GetAllEntities();
        Assert.AreEqual(1, entities.Length);
        
        entities.Dispose();
        
        ss.Update();
        
        entities = entityManager.GetAllEntities();
        Assert.AreEqual(2, entities.Length);
        
        entities.Dispose();
    }
}
