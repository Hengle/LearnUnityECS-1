using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AddPureSystemTest : ECSTestBase
{
    
    [Test]
    public void AddOnceCorrect()
    {
        _entityManager.CreateEntity(typeof(Adder));
        
        AddPureSystem system = _world.GetOrCreateManager<AddPureSystem>();
        
        system.Update();

        var adderGroup = _entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(1, adders[0].Value);
        
        system.Update();
        
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(2, adders[0].Value);
    }
}
