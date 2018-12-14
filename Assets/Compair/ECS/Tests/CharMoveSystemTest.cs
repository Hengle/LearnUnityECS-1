using NUnit.Framework;
using Unity.Entities;
using Unity.Mathematics;

public class CharMoveSystemTest : ECSTestBase
{

    private EntityManager _entityManager;
    
    [SetUp]
    public void Init()
    {
        _entityManager = new EntityManager();
    }

    [Test]
    public void Speed0_DontMove()
    {
        var entity = _entityManager.CreateEntity(typeof(Char), typeof (PositionHybrid));
        _entityManager.SetComponentData(entity, new Char());
//        _entityManager.SetComponentData(entity, new PositionHybrid{Value = float3.zero});
//            
//        // act
//        World.CreateManager<CharMoveSystem>().Update();
//            
//        // assert
//        Assert.AreEqual(new float3(0, 0, 0), _entityManager.GetComponentData<Position>(entity).Value);
    }
}
