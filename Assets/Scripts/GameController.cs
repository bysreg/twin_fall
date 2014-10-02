using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	public bool simulateWithKeyboard;
	public float playerZDistanceFromCamera;

	private GameObject player;
	private GameObject mainCam;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
	}

	void Update()
	{

	}

}
