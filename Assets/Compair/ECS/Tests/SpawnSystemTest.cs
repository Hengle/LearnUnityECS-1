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

        var entityManager = World.GetOrCreateManager<EntityManager>();
        Assert.AreEqual(1, entityManager.GetAllEntities().Length);
    }
    
    [TearDown]
    public void TearDown() 
    {
       
    }
}
