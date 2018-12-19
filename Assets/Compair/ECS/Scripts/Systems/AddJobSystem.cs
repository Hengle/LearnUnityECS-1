using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class AddJobSystem : JobComponentSystem
{
    private ComponentGroup _adderGroup;

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
        _adderGroup = GetComponentGroup(typeof(Adder));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new AddJob()
        {
            Adders = _adderGroup.GetComponentDataArray<Adder>()
        };
        return job.Schedule(_adderGroup.CalculateLength(), 64, inputDeps);
    }
}