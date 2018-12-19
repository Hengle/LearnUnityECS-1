using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CharMoveSystem : JobComponentSystem
{
    struct Move : IJobParallelFor
    {
        public ComponentArray<PositionHybrid> Positions;
        public float DeltaTime;
        public float3 MoveSpeed;
        public float xMin;
        public float xMax;
        
        public void Execute(int i)
        {
            var newPosition = new float3(
                Positions[i].Value + MoveSpeed * DeltaTime);
            if (newPosition.x <= xMin)
            {
                newPosition.x = xMax;
            }
            
            Positions[i].Value = newPosition;
        }
    }

    public ComponentGroup CharGroup;

    protected override void OnCreateManager()
    {
        CharGroup = GetComponentGroup(ComponentType.ReadOnly<Char>(),
            typeof(PositionHybrid));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settings = Settings.Instance;
        var moveJob = new Move
        {
            Positions = CharGroup.GetComponentArray<PositionHybrid>(),
            DeltaTime = 1,
            MoveSpeed = 1,
            xMin = 1,
            xMax = 1
        };
        var moveJobHandle = moveJob.Schedule(CharGroup.CalculateLength(),
            64, inputDeps);
        return moveJobHandle;
    }
}