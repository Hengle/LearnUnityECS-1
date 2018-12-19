using NUnit.Framework;
using Unity.Entities;
using Unity.Jobs;

[DisableAutoCreation]
public class EmptySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle dep) { return dep; }


    new public ComponentGroup GetComponentGroup(params EntityArchetypeQuery[] queries)
    {
        return base.GetComponentGroup(queries);
    }

    new public ComponentGroup GetComponentGroup(params ComponentType[] componentTypes)
    {
        return base.GetComponentGroup(componentTypes);
    }

    new public ComponentGroupArray<T> GetEntities<T>() where T : struct
    {
        return base.GetEntities<T>();
    }
}

public class ECSTestBase
{
    protected static World _previousWorld;
    protected static World _world;
    protected EntityManager _entityManager;
    protected EntityManager.EntityManagerDebug _managerDebug;

    protected int StressTestEntityCount = 1000;
    
    [SetUp]
    public virtual void Setup()
    {
        _previousWorld = World.Active;
        _world = World.Active = new World("Test World");

        _entityManager = _world.GetOrCreateManager<EntityManager>();
        _managerDebug = new EntityManager.EntityManagerDebug(_entityManager);
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (_entityManager != null)
        {
            _world.Dispose();
            _world = null;

            World.Active = _previousWorld;
            _previousWorld = null;
            _entityManager = null;
        }
    }

    public void AssertDoesNotExist(Entity entity)
    {
        Assert.IsFalse(_entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsFalse(_entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(_entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsFalse(_entityManager.Exists(entity));
    }

    public void AssertComponentData(Entity entity, int index)
    {
        Assert.IsTrue(_entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(_entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(_entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(_entityManager.Exists(entity));

        Assert.AreEqual(-index, _entityManager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(-index, _entityManager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(index, _entityManager.GetComponentData<EcsTestData>(entity).value);
    }

    public Entity CreateEntityWithDefaultData(int index)
    {
        var entity = _entityManager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));

        // HasComponent & Exists setup correctly
        Assert.IsTrue(_entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(_entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(_entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(_entityManager.Exists(entity));

        // Create must initialize values to zero
        Assert.AreEqual(0, _entityManager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(0, _entityManager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(0, _entityManager.GetComponentData<EcsTestData>(entity).value);

        // Setup some non zero default values
        _entityManager.SetComponentData(entity, new EcsTestData2(-index));
        _entityManager.SetComponentData(entity, new EcsTestData(index));

        AssertComponentData(entity, index);

        return entity;
    }

    public EmptySystem EmptySystem
    {
        get
        {
            return World.Active.GetOrCreateManager<EmptySystem>();
        }
    }
}
