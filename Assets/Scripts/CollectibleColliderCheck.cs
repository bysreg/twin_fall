using UnityEngine;
using System.Collections;

public class CollectibleColliderCheck : MonoBehaviour {

	public AudioClip hitCollClip;
	public GameController.CollSpawnData.Type type;
	
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
			AudioSource.PlayClipAtPoint (hitCollClip, other.gameObject.transform.position);
			isHit = true;
			if(type == GameController.CollSpawnData.Type.Beat)
			{
				gameController.IncComboCount(transform.position);
			}
		}
	}

	public bool IsHit()
	{
		return isHit;
	}
}
