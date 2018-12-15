using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AdderPureSystemTest : ECSTestBase
{
    
    [Test]
    public void AddOnceCorrect()
    {
        entityManager.CreateEntity(typeof(Adder));
        
        AdderPureSystem system = World.GetOrCreateManager<AdderPureSystem>();
        
        system.Update();

        var adderGroup = entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(1, adders[0].Value);
        
        system.Update();
        
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(2, adders[0].Value);
    }
}
