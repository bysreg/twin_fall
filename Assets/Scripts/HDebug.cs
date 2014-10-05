using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HDebug : MonoBehaviour {

	private GUIText timeText;
	private GUIText nextCorrHitText;
	private GUIText nextSpawnTimeText;
	private GUIText lifesText;
	private GUIText nextFeatherHit;
	private GUIText comboText;
	private GameController gameController;
	private List<GameController.CorridorSpawnData> corrSpawnDatas;

	void Start()
	{
		timeText = transform.FindChild ("TimeText").GetComponent<GUIText>();
		nextCorrHitText = transform.FindChild("NextCorrHit").GetComponent<GUIText>();
		nextSpawnTimeText = transform.FindChild ("NextSpawnTimeText").GetComponent<GUIText> ();
		lifesText = transform.FindChild ("LifesText").GetComponent<GUIText> ();
		nextFeatherHit = transform.FindChild ("NextFeatherHit").GetComponent<GUIText>();
		comboText = transform.FindChild("ComboText").GetComponent<GUIText>();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		corrSpawnDatas = gameController.GetCorrSpawnDatas ();
	}

	void Update()
	{
		timeText.text = "Time : " + gameController.GetTime();
		nextCorrHitText.text = "Next Corr Hit : " + gameController.GetCurOldestCorrHitTime();
		nextSpawnTimeText.text = "Next Spawn : " + gameController.GetNextCorrSpawnTime();
		lifesText.text = "Lifes : " + gameController.lifes;
		nextFeatherHit.text = "Next Feather Hit : " + gameController.GetCurOldestCollHitTime();
	}
}
