using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		pos += transform.forward * GameManager.GM.charSpeed * Time.deltaTime;

		if (pos.x < GameManager.GM.leftBound)
		{
			pos.x = GameManager.GM.rightBound;
		}

		transform.position = pos;
	}
}
