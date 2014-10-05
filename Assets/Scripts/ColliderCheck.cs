using UnityEngine;
using System.Collections;

public class ColliderCheck : MonoBehaviour {

	public AudioClip hitCorrClip;
	public AudioClip passCorrClip; // not used anymore

	private GameController gameController;
	private float passZ; // z pos when player is considered to be successfu;; going through the corridor
	private GameObject mainCam;

	private bool isHit;
	private bool isPassClipPlayed;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		mainCam = gameController.GetMainCam ();
	}

	void Start()
	{
		passZ = gameController.GetPlayer1 ().transform.position.z - 0.1f;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			gameController.HitPlayer();
			AudioSource.PlayClipAtPoint (hitCorrClip, other.gameObject.transform.position);
			isHit = true;
			gameController.CancelCombo();
		}
	}

	void Update()
	{
		//play happy sound when player has passed the corridor without hitting
		if(transform.position.z < passZ && !isPassClipPlayed && !isHit)
		{
			//AudioSource.PlayClipAtPoint (passCorrClip, mainCam.transform.position);
			isPassClipPlayed = true;
		}
	}

}
