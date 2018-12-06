using Unity.Mathematics;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance;
    
    public float3 CharMoveSpeed;
    
    private void Awake()
    {
        Instance = this;
    }
}