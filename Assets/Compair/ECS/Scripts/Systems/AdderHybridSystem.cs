using Unity.Collections;
using Unity.Entities;

public class AdderWraper : ComponentDataWrapper<Adder>
{
}

public class AdderHybridSystem : ComponentSystem
{
    public ComponentGroup AdderGroup;

    protected override void OnCreateManager()
    {
        AdderGroup = GetComponentGroup(typeof(Adder));
    }

    protected override void OnUpdate()
    {
        var adders = AdderGroup.GetComponentDataArray<Adder>();
        for (int i = 0; i < adders.Length; i++)
        {
            adders[i] = new Adder()
            {
                Value = adders[i].Value+1
            };
        }
    }
}