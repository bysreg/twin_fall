using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public bool simulateWithMouse;

	private GameObject player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update()
	{
		if (simulateWithMouse)
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 point = ray.GetPoint(10);
			player.transform.position = point;
		}
	}

}
