using UnityEngine;
using System.Collections;

public class RetryButtonController : MonoBehaviour {

	void OnGUI() {
		GUI.backgroundColor = Color.clear;
		if (GUI.Button(new Rect(0, 0, 5000, 5000), ""))
			Application.LoadLevel(1);
		
	}
}
