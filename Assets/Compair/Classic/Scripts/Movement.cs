using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		pos += transform.forward * ClassicGameManager.GM.charSpeed * Time.deltaTime;

		if (pos.x < ClassicGameManager.GM.leftBound)
		{
			pos.x = ClassicGameManager.GM.rightBound;
		}

		transform.position = pos;
	}
}
