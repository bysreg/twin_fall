﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour {
	
	public bool simulateWithKeyboard;
	public float playerZDistanceFromCamera;
	public Vector3 corridorV;
	public int lifes;
	public TextAsset corrSpawnDataText;
	public TextAsset collectiblesSpawnDataText;

	private GameObject player;
	private GameObject player2;
	private GameObject mainCam;

	private GameObject[] corridors;
	private List<GameObject> activeCorridors;
	private List<CorridorSpawnData> corrSpawnDatas;
	//fixme
	//private List<
	private int curCorrSpawnIndex;
	private float time;
	private float nextCorrSpawnTime;
	private GameObject[] trunks;
	private GameObject curTrunk;
	private GameObject nextTrunk;

	public class CorridorSpawnData
	{
		public float time;
	}

	public class CollSpawnData
	{
		public float time;
		//public Vector2
	}

	void Awake()
	{
		player = GameObject.Find("Bob");
		player2 = GameObject.Find("Pew");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		corridors = GameObject.FindGameObjectsWithTag ("Corridor");
		corrSpawnDatas = new List<CorridorSpawnData> ();
		activeCorridors = new List<GameObject> ();

		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		player2.transform.position = new Vector3 (mainCam.transform.position.x + 3, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);

		string spawnDataContent = corrSpawnDataText.text;
		string[] lines = spawnDataContent.Split (new char[] {'\n'});
		float s = player.transform.position.z - corridors [0].transform.position.z;
		float deltaTime = Mathf.Abs(s / corridorV.z); // time from spawn to reach player 

		foreach(string line in lines)
		{
			if(line.Length == 0)
			{
				continue;
			}

			CorridorSpawnData corrSpawnData = new CorridorSpawnData();
			corrSpawnData.time = float.Parse(line) - deltaTime;
			corrSpawnDatas.Add(corrSpawnData);
		}

		nextCorrSpawnTime = GetNextCorrSpawnData ().time;

		//fixme
		//string collectiblesSpawnDataString = collectiblesSpawnDataText.text;
		//string[] collLines = collectiblesSpawnDataString.Split(new char[] {'\n'});

		//fixme
		//foreach

		trunks = new GameObject[2];
		trunks[0] = GameObject.Find("/trunks_new/Trunk1");
		trunks[1] = GameObject.Find("/trunks_new/Trunk2");
		curTrunk = trunks[0];
		nextTrunk = trunks[1];
	}

	//fixme
	//void Initialize

	void Update()
	{
		time += Time.deltaTime;

		UpdateSpawnCorr();
		UpdateSpawnCollectibles();
	}

	void FixedUpdate()
	{
		UpdateCorridor ();
		UpdateTrunk();
	}

	void UpdateSpawnCorr()
	{	
		if(time >= nextCorrSpawnTime && curCorrSpawnIndex < corrSpawnDatas.Count)
		{
			int random = Random.Range(0, corridors.Length);
			//print (random + " " + (corridors.Length));
			activeCorridors.Add(SpawnCorridor(random));
			curCorrSpawnIndex++;
			CorridorSpawnData nextCorrSpawnData = GetNextCorrSpawnData();
			if(nextCorrSpawnData != null)
			{
				nextCorrSpawnTime = GetNextCorrSpawnData().time;
			}
		}
	}

	void UpdateSpawnCollectibles()
	{

	}

	void UpdateCorridor()
	{
		for(int i=activeCorridors.Count -1; i >= 0; i--)
		{
			var corridor = activeCorridors[i];
			corridor.transform.position += corridorV * Time.fixedDeltaTime; 

			//delete the corridor if it's not used anymore
			if (corridor.transform.position.z < -10)
			{
				activeCorridors.Remove(corridor);
				Destroy(corridor);
			}
		}
	}

	void UpdateTrunk()
	{
		for(int i=0;i < trunks.Length;i++)
		{
			trunks[i].transform.position += corridorV * Time.fixedDeltaTime;
		}

		for(int i=0;i < trunks.Length; i++)
		{
			if(trunks[i].transform.position.z < -64)
			{
				trunks[i].transform.position = trunks[(i + 1) % 2].transform.position + new Vector3(0, 0, 64.71f);

				break;
			}
		}
	}

	GameObject SpawnCorridor(int type)
	{
		GameObject corr = Instantiate (corridors [type]) as GameObject;
		corr.transform.position = new Vector3(0, 1, 50);
		return corr;
	}

	public List<CorridorSpawnData> GetCorrSpawnDatas()
	{
		return corrSpawnDatas;
	}

	public int GetCurCorrSpawnindex()
	{
		return curCorrSpawnIndex;
	}

	public CorridorSpawnData GetNextCorrSpawnData()
	{
		if(corrSpawnDatas.Count > curCorrSpawnIndex)
			return corrSpawnDatas[curCorrSpawnIndex];

		return null;
	}

	public float GetTime()
	{
		return time;
	}

	public float GetNextCorrSpawnTime()
	{
		return nextCorrSpawnTime;
	}

	public void HitPlayer()
	{
		lifes--;
		lifes = Mathf.Max(0, lifes);
	}

	public GameObject GetPlayer1()
	{
		return player;
	}

	public GameObject GetMainCam()
	{
		return mainCam;
	}
}
