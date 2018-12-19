using NUnit.Framework;
using NUnit.Framework.Internal;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class AddReactiveSystemTest : ECSTestBase
{
    
    [TestCase(100)]
    public void AddTimesCorrect(int times)
    {
        AddReactiveSystem system = _world.GetOrCreateManager<AddReactiveSystem>();
        system.Update();
        _entityManager.CompleteAllJobs();
        
        for (int i = 0; i < times; i++)
        {
            var entity = _entityManager.CreateEntity(typeof(Adder));
            //从测试结果来看，对于ReactiveSystem,CreateEntity时是无法激活System的
            //必须首次的SetComponentData才能激活
            _entityManager.SetComponentData(entity, new Adder
            {
                Value = 0
            });
        }
        
        system.Update();
        _entityManager.CompleteAllJobs();

        var adderGroup = _entityManager.CreateComponentGroup(typeof(Adder));
        var adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(1, adders[i].Value);
        }
        
        //再次Update不会再次激活ReactiveSystem运行
        system.Update();
        _entityManager.CompleteAllJobs();
        
        adders = adderGroup.GetComponentDataArray<Adder>();
        Assert.AreEqual(100, adders.Length);
        for (int i = 0; i < times; i++)
        {
            Assert.AreEqual(1, adders[i].Value);
        }
    }
}
