using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

public class UpdateTransformPositionSystem : ComponentSystem
{
    struct Data
    {
        public readonly int Length;
        [ReadOnly] public ComponentArray<PositionHybrid> Positions;
        public TransformAccessArray Transforms;
    }

    [Inject] Data _data;
    
    protected override void OnUpdate()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            var position = _data.Positions[i];
            _data.Transforms[i].position = new float3(
                position.Value.x, position.Value.y, position.Value.z);
        }
    }
}