using UnityEngine;
using System.Collections;

public class ColliderCheck : MonoBehaviour {

	public AudioClip[] hitCorrClip;
	public AudioClip passCorrClip; // not used anymore
	public bool hasMultipleTrigger;

	private GameController gameController;
	private float passZ; // z pos when player is considered to be successfu;; going through the corridor
	private GameObject mainCam;

	private bool isHit;
	private bool isPassClipPlayed;
	private GameObject hitPlayer;
	private GameObject hitPlayer2;

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
			if(hasMultipleTrigger)
			{
				if(hitPlayer != null && other.name == "Bob")
					return;
				if(hitPlayer2 != null && other.name == "Pew")
					return;
			}

			int random = Random.Range(0, hitCorrClip.Length);
			gameController.HitPlayer(random, other.gameObject.transform.position);
			//print (random);
			AudioSource.PlayClipAtPoint (hitCorrClip[random], other.gameObject.transform.position);
			isHit = true;
			gameController.CancelCombo();

			if(hasMultipleTrigger)
			{
				if(hitPlayer == null && other.name == "Bob")
					hitPlayer = other.gameObject;
				if(hitPlayer2 == null && other.name == "Pew")
					hitPlayer2 = other.gameObject;
			}

			if(hitPlayer == null)
				hitPlayer = other.gameObject;
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
