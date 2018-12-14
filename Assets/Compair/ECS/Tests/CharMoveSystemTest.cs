//using NUnit.Framework;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//
//public class CharMoveSystemTest : ECSTestBase
//{
//    
//    [Test]
//    public void Speed0_DontMove()
//    {
//        var entity = new GameObject();
//        entity.AddComponent<CharWrapper>();
//        entity.AddComponent<PositionHybrid>();
//        GameObjectEntity.AddToEntityManager(entityManager, entity);
//            
//        // act
//        var manager = World.GetOrCreateManager<CharMoveSystem>();
//        
//        Assert.AreEqual(new float3(0,0,0), entity.GetComponent<PositionHybrid>().Value);
//        
//        manager.Update();
//        
//        // assert
//        //确实进入到system的监视列表中
//        Assert.AreEqual(1, manager.CharGroup.CalculateLength());
//        
//        Assert.AreEqual(new float3(1, 1, 1), entity.GetComponent<PositionHybrid>().Value);
////        Assert.AreEqual(new float3(1,1,1), manager.CharGroup.GetComponentArray<PositionHybrid>()[0].Value);
//    }
//}
