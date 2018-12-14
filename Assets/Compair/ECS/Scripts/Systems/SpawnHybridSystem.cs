using Unity.Entities;
using UnityEngine;

public struct Record : IComponentData
{
    public int Value;
}
public class RecordWrapper : ComponentDataWrapper<Record>
{
}

public class SpawnHybridSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var gameObject = new GameObject("empty", typeof(RecordWrapper));
        var record = gameObject.GetComponent<RecordWrapper>();
        record.Value = new Record {Value = 3};
    }
}