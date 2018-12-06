using System;
using Unity.Entities;

[Serializable]
public struct Char : IComponentData
{
}

public class CharWrapper : ComponentDataWrapper<Char>
{
}