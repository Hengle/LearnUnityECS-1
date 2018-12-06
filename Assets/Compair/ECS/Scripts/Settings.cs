using Unity.Mathematics;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance;
    
    public float3 CharMoveSpeed;

    public Rect Boundary;
    
    private void Awake()
    {
        Instance = this;
    }
}