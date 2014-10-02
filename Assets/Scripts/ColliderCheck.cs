using UnityEngine;
using System.Collections;

public class ColliderCheck : MonoBehaviour {

	private GameController gameController;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			gameController.HitPlayer();
		}
	}

}
