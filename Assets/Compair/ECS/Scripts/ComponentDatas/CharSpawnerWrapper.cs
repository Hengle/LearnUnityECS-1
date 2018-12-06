using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct CharSpawner : ISharedComponentData
{
    public GameObject Prefab;
    public float Distance;
    public int Count;
}

public class CharSpawnerWrapper : SharedComponentDataWrapper<CharSpawner>
{
}