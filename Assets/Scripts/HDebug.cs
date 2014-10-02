using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HDebug : MonoBehaviour {

	private GUIText timeText;
	private GUIText nextSpawnTimeText;
	private GUIText lifesText;
	private GameController gameController;
	private List<GameController.CorridorSpawnData> corrSpawnDatas;

	void Start()
	{
		timeText = transform.FindChild ("TimeText").GetComponent<GUIText>();
		nextSpawnTimeText = transform.FindChild ("NextSpawnTimeText").GetComponent<GUIText> ();
		lifesText = transform.FindChild ("LifesText").GetComponent<GUIText> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		corrSpawnDatas = gameController.GetCorrSpawnDatas ();
	}

	void Update()
	{
		timeText.text = "Time : " + gameController.GetTime();
		nextSpawnTimeText.text = "Next Spawn : " + gameController.GetNextCorrSpawnTime();
		lifesText.text = "Lifes : " + gameController.lifes;
	}
}
