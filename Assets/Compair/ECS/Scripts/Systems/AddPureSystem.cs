using Unity.Collections;
using Unity.Entities;

public struct Adder : IComponentData
{
    public int Value;
}

public class AddPureSystem : ComponentSystem
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