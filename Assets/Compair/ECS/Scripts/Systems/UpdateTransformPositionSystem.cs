using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

public class UpdateTransformPositionSystem : JobComponentSystem
{
    struct Data
    {
        public readonly int Length;
        [ReadOnly] public ComponentArray<PositionHybrid> Positions;
        public TransformAccessArray Transforms;
    }

    [Inject] Data _data;

    struct UpdatePosition : IJobParallelForTransform
    {
        public ComponentArray<PositionHybrid> Positions;
        public void Execute(int i, TransformAccess transform)
        {
            transform.position = new float3(
                Positions[i].Value.x, Positions[i].Value.y, Positions[i].Value.z);
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UpdatePosition()
        {
            Positions = _data.Positions
        };

        return job.Schedule(_data.Transforms, inputDeps);
    }
}