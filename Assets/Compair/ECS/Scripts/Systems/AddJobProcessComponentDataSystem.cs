using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class AddJobProcessComponentDataSystem : JobComponentSystem
{
    public struct AddJob : IJobProcessComponentData<Adder>
    {
        public void Execute(ref Adder data)
        {
            data.Value = data.Value + 1;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AddJob();
        return job.Schedule(this, inputDeps);
    }
}