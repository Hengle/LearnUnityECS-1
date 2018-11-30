using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct MovementJob : IJobParallelForTransform
{

	public float moveSpeed;
	public float leftBound;
	public float rightBound;
	public float deltaTime;

	public void Execute(int index, TransformAccess transform)
	{
		Vector3 pos = transform.position;
		pos += moveSpeed * deltaTime *
		       (transform.rotation * new Vector3(0f, 0f, 1f));

		if (pos.x < leftBound)
		{
			pos.x = rightBound;
		}

		transform.position = pos;
	}
}
