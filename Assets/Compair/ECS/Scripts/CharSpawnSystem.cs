﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CharSpawnSystem : ComponentSystem
{
    #pragma warning disable 649
    struct Group
    {
        public readonly int Length;
        [ReadOnly]
        public SharedComponentDataArray<CharSpawner> Spawners;
        public ComponentDataArray<Position> Positions;
        public EntityArray Entities;
    }
    #pragma warning restore 649

    [Inject] Group _group;
    
    protected override void OnUpdate()
    {
        while (_group.Length != 0)
        {
            var spawnerEntity = _group.Entities[0];
            //获取生成器
            var spawner = _group.Spawners[0];
            //获取坐标位置
            var pivot = _group.Positions[0].Value;
            
            //为产生角色分配空间
            var entities = new NativeArray<Entity>(spawner.Count, Allocator.Temp);
            //生成新角色
            EntityManager.Instantiate(spawner.Prefab, entities);
            
            //分散位置
            var positions = new NativeArray<float3>(spawner.Count, Allocator.Temp);
            for (int i = 0; i < spawner.Count/10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    positions[i*10+j] = pivot
                        + new float3(i*spawner.Distance, 0.0f, j*spawner.Distance);
                }
            }
            
            for (var i = 0; i < spawner.Count; i++)
            {
                EntityManager.SetComponentData(entities[i], 
                    new Position{Value = positions[i]});
            }
            
            entities.Dispose();
            positions.Dispose();
            
            EntityManager.RemoveComponent<CharSpawner>(spawnerEntity);
            UpdateInjectedComponentGroups();
        }
    }
}