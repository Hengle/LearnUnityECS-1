using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{

	public static GameManager GM;

	public CharCountText CharCountText;

	[Header("Simulation Settings")]
	public float topBound;
	public float bottomBound;
	public float leftBound;
	public float rightBound;

	[Header("Char Settings")]
	public GameObject charPrefab;
	public float charSpeed;

	[Header("Spawn Settings")]
	public int charCount;
	public int charIncremement;

	public int CurrentCount;

	private void Awake()
	{
		GM = this;
	}

	private void Start()
	{
		AddChar(charCount);
	}

	private void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			AddChar(charIncremement);
		}
	}

	private void AddChar(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			float x = Random.Range(leftBound, rightBound);
			float z = Random.Range(bottomBound, topBound);
			
			Vector3 pos = new Vector3(x, 0, z);

			var obj = Instantiate(charPrefab) as GameObject;
			obj.transform.position = pos;
		}

		CurrentCount += amount;
		CharCountText.UpdateCount(CurrentCount);
	}
}
