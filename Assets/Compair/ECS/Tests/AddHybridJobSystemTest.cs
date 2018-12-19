using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AddHybridJobSystemTest : ECSTestBase
{
    
    [TestCase(100)]
    public void AddTimesCorrect(int times)
    {
        for (int i = 0; i < times; i++)
        {
            var go = new GameObject("test", typeof(AdderWraper));
        }
        
        AddJobSystem system = _world.GetOrCreateManager<AddJobSystem>();
        
        system.Update();
        _entityManager.CompleteAllJobs();

        var adderGroup = _entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(1, adders[i].Value);
        }
        
        system.Update();
        _entityManager.CompleteAllJobs();
        
        adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(2, adders[i].Value);
        }
    }
}
