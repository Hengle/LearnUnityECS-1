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
    protected static World m_PreviousWorld;
    protected static World World;
    protected EntityManager entityManager;
    protected EntityManager.EntityManagerDebug m_ManagerDebug;

    protected int StressTestEntityCount = 1000;
    
    [SetUp]
    public virtual void Setup()
    {
        m_PreviousWorld = World.Active;
        World = World.Active = new World("Test World");

        entityManager = World.GetOrCreateManager<EntityManager>();
        m_ManagerDebug = new EntityManager.EntityManagerDebug(entityManager);
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (entityManager != null)
        {
            World.Dispose();
            World = null;

            World.Active = m_PreviousWorld;
            m_PreviousWorld = null;
            entityManager = null;
        }
    }

    public void AssertDoesNotExist(Entity entity)
    {
        Assert.IsFalse(entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsFalse(entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsFalse(entityManager.Exists(entity));
    }

    public void AssertComponentData(Entity entity, int index)
    {
        Assert.IsTrue(entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(entityManager.Exists(entity));

        Assert.AreEqual(-index, entityManager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(-index, entityManager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(index, entityManager.GetComponentData<EcsTestData>(entity).value);
    }

    public Entity CreateEntityWithDefaultData(int index)
    {
        var entity = entityManager.CreateEntity(typeof(EcsTestData), typeof(EcsTestData2));

        // HasComponent & Exists setup correctly
        Assert.IsTrue(entityManager.HasComponent<EcsTestData>(entity));
        Assert.IsTrue(entityManager.HasComponent<EcsTestData2>(entity));
        Assert.IsFalse(entityManager.HasComponent<EcsTestData3>(entity));
        Assert.IsTrue(entityManager.Exists(entity));

        // Create must initialize values to zero
        Assert.AreEqual(0, entityManager.GetComponentData<EcsTestData2>(entity).value0);
        Assert.AreEqual(0, entityManager.GetComponentData<EcsTestData2>(entity).value1);
        Assert.AreEqual(0, entityManager.GetComponentData<EcsTestData>(entity).value);

        // Setup some non zero default values
        entityManager.SetComponentData(entity, new EcsTestData2(-index));
        entityManager.SetComponentData(entity, new EcsTestData(index));

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
