using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AddHybridSystemTest : ECSTestBase
{
    
    [Test]
    public void AddOnceCorrect()
    {
        var go = new GameObject("test", typeof(AdderWraper));
        
        AddHybridSystem system = _world.GetOrCreateManager<AddHybridSystem>();
        
        system.Update();

        var adderGroup = _entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(1, adders[0].Value);
        
        system.Update();
        
        Assert.AreEqual(1, adders.Length);
        Assert.AreEqual(2, adders[0].Value);
        
        //用于创建entity的原型gameobject是被抛弃，不算在ECSWorld内的
        Assert.AreEqual(0,go.GetComponent<AdderWraper>().Value.Value);
    }
}
