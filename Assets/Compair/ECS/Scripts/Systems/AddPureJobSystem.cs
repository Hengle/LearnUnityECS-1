using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class AddPureJobSystem : JobComponentSystem
{
    public ComponentGroup AdderGroup;

    public struct AddJob : IJobParallelFor
    {
        public ComponentDataArray<Adder> Adders;
        public void Execute(int i)
        {
            Adders[i] = new Adder()
            {
                Value = Adders[i].Value+1
            };
        }
    }
    
    protected override void OnCreateManager()
    {
        AdderGroup = GetComponentGroup(typeof(Adder));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AddJob()
        {
            Adders = AdderGroup.GetComponentDataArray<Adder>()
        };
        return job.Schedule(AdderGroup.CalculateLength(), 64, inputDeps);
    }
}