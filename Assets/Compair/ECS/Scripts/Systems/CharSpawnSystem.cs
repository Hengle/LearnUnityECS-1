using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharSpawnBarrier : BarrierSystem
{
}

public class CharSpawnSystem : ComponentSystem
{
    #pragma warning disable 649
    struct Group
    {
        public readonly int Length;
        [ReadOnly]
        public SharedComponentDataArray<CharSpawner> Spawners;
        public EntityArray Entities;
    }
    #pragma warning restore 649

    [Inject] Group _group;
    [Inject] CharSpawnBarrier _barrier;
    
    protected override void OnUpdate()
    {
        while (_group.Length != 0)
        {
            var spawnerEntity = _group.Entities[0];
            //获取生成器
            var spawner = _group.Spawners[0];
            //获取散布范围
            var boundary = Settings.Instance.Boundary;
            
            //为产生角色分配空间
            var entities = new NativeArray<Entity>(spawner.Count, Allocator.Temp);
            //生成新角色
            EntityManager.Instantiate(spawner.Prefab, entities);
            
            //分散位置
            var positions = new NativeArray<float3>(spawner.Count, Allocator.Temp);
            for (var i = 0; i < spawner.Count; i++)
            {
                positions[i] = new float3(
                    Random.Range(boundary.xMin, boundary.xMax), 0.0f,
                    Random.Range(boundary.yMin, boundary.yMax));
            }
            
            for (var i = 0; i < spawner.Count; i++)
            {
                EntityManager.SetComponentData(entities[i], 
                    new Position{Value = positions[i]});
            }
            
            entities.Dispose();
            positions.Dispose();
            
//            _barrier.CreateCommandBuffer().DestroyEntity(spawnerEntity);
            EntityManager.RemoveComponent<CharSpawner>(spawnerEntity);
            UpdateInjectedComponentGroups();
        }
    }
}