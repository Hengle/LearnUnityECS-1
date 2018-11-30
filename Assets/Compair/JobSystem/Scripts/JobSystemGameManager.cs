using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class JobSystemGameManager : MonoBehaviour
{

	public static JobSystemGameManager GM;

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

	private TransformAccessArray _transforms;
	private MovementJob _movementJob;
	private JobHandle _moveHandle;

	private void Awake()
	{
		GM = this;
	}

	private void OnDisable()
	{
		_moveHandle.Complete();
		_transforms.Dispose();
	}

	private void Start()
	{
		_transforms = new TransformAccessArray(0, -1);
		
		AddChar(charCount);
	}

	private void Update()
	{
		_moveHandle.Complete();
		
		if (Input.GetKeyDown("space"))
		{
			AddChar(charIncremement);
		}

		_movementJob = new MovementJob()
		{
			moveSpeed = charSpeed,
			leftBound = leftBound,
			rightBound = rightBound,
			deltaTime = Time.deltaTime
		};

		_moveHandle = _movementJob.Schedule(_transforms);
		
		JobHandle.ScheduleBatchedJobs();
	}

	private void AddChar(int amount)
	{
		_moveHandle.Complete();

		_transforms.capacity = _transforms.length + amount;
		
		for (int i = 0; i < amount; i++)
		{
			float x = Random.Range(leftBound, rightBound);
			float z = Random.Range(bottomBound, topBound);
			
			Vector3 pos = new Vector3(x, 0, z);

			var obj = Instantiate(charPrefab) as GameObject;
			obj.transform.position = pos;
			
			_transforms.Add(obj.transform);
		}

		CurrentCount += amount;
		CharCountText.UpdateCount(CurrentCount);
	}
}
