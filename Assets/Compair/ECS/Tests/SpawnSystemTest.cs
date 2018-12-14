using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnSystemTest : ECSTestBase
{
    
    [Test]
    public void Spawn1Entity()
    {
        SpawnSystem ss = World.GetOrCreateManager<SpawnSystem>();
        
        ss.Update();

        var entities = entityManager.GetAllEntities();
        Assert.AreEqual(1, entities.Length);
        
        entities.Dispose();
    }
}
