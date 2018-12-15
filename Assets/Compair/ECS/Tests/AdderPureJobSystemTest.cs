using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AdderPureJobSystemTest : ECSTestBase
{
    
    [TestCase(100)]
    public void AddTimesCorrect(int times)
    {
        for (int i = 0; i < times; i++)
        {
            entityManager.CreateEntity(typeof(Adder));
        }
        
        AddPureSystem system = World.GetOrCreateManager<AddPureSystem>();
        
        system.Update();

        var adderGroup = entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(1, adders[i].Value);
        }
        
        system.Update();
        
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(2, adders[i].Value);
        }
    }
}
