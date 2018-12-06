using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CharMoveSystem : JobComponentSystem
{
    [BurstCompile]
    struct Move : IJobParallelFor
    {
        public ComponentDataArray<Position> Positions;
        public float DeltaTime;
        public float3 MoveSpeed;
        
        public void Execute(int i)
        {
            Positions[i] = new Position()
            {
                Value = Positions[i].Value + MoveSpeed * DeltaTime
            };
        }
    }

    ComponentGroup _charGroup;

    protected override void OnCreateManager()
    {
        _charGroup = GetComponentGroup(ComponentType.ReadOnly<Char>(),
            typeof(Position));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var moveJob = new Move
        {
            Positions = _charGroup.GetComponentDataArray<Position>(),
            DeltaTime = Time.deltaTime,
            MoveSpeed = Settings.Instance.CharMoveSpeed
        };
        var moveJobHandle = moveJob.Schedule(_charGroup.CalculateLength(),
            64, inputDeps);
        return moveJobHandle;
    }
}