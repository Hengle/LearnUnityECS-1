using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class AddJobProcessComponentDataTest : ECSTestBase
{
    
    [TestCase(100)]
    public void AddTimesCorrect(int times)
    {
        for (int i = 0; i < times; i++)
        {
            _entityManager.CreateEntity(typeof(Adder));
        }
        
        AddJobProcessComponentDataSystem system = 
            _world.GetOrCreateManager<AddJobProcessComponentDataSystem>();
        
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
