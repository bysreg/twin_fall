using UnityEngine;
using System.Collections;

public class CollectibleColliderCheck : MonoBehaviour {

	public AudioClip hitCollClip; // unused for type melody, instead melody will use melodies stored in gamecontroller
	public GameController.CollSpawnData.Type type;

	//melody only
	public AudioClip melody;

	private GameController gameController;
	
	private bool isHit;

	void Awake()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !isHit) 
		{
			isHit = true;
			if(type == GameController.CollSpawnData.Type.Beat)
			{
				gameController.IncComboCount(transform.position);
				AudioSource.PlayClipAtPoint (hitCollClip, other.gameObject.transform.position);
			}
			else if(type == GameController.CollSpawnData.Type.Melody)
			{
				AudioSource.PlayClipAtPoint (melody, other.gameObject.transform.position);
			}
		}
	}

	public bool IsHit()
	{
		return isHit;
	}
}
