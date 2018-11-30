using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharCountText : MonoBehaviour
{

	public Text Text;
	
	public void UpdateCount(int charCount)
	{
		Text.text = ""+charCount;
	}
	
}
