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

    ComponentGroup _charGroup;

    protected override void OnCreateManager()
    {
        _charGroup = GetComponentGroup(ComponentType.ReadOnly<Char>(),
            typeof(PositionHybrid));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var settings = Settings.Instance;
        var moveJob = new Move
        {
            Positions = _charGroup.GetComponentArray<PositionHybrid>(),
            DeltaTime = Time.deltaTime,
            MoveSpeed = settings.CharMoveSpeed,
            xMin = settings.Boundary.xMin,
            xMax = settings.Boundary.xMax
        };
        var moveJobHandle = moveJob.Schedule(_charGroup.CalculateLength(),
            64, inputDeps);
        return moveJobHandle;
    }
}