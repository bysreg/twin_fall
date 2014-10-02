using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour {
	
	public bool simulateWithKeyboard;
	public float playerZDistanceFromCamera;
	public Vector3 corridorV;

	private GameObject player;
	private GameObject mainCam;
	private StreamReader sr;

	private GameObject[] corridors;
	private List<GameObject> activeCorridors;
	private List<CorridorSpawnData> corrSpawnDatas;
	private int curCorrSpawnIndex;
	private float time;
	private float nextCorrSpawnTime;

	public class CorridorSpawnData
	{
		public float time;
	}

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		corridors = GameObject.FindGameObjectsWithTag ("Corridor");
		corrSpawnDatas = new List<CorridorSpawnData> ();
		activeCorridors = new List<GameObject> ();

		sr = new StreamReader (@"Assets\sound_beat.txt");
		while(sr.Peek() >= 0)
		{
			string data;
			CorridorSpawnData corrSpawnData = new CorridorSpawnData();
			data = sr.ReadLine();
			corrSpawnData.time = float.Parse(data);
			corrSpawnDatas.Add(corrSpawnData);
		}

		nextCorrSpawnTime = GetNextCorrSpawnData ().time;
	}

	void Update()
	{
		time += Time.deltaTime;

		if(time >= nextCorrSpawnTime && curCorrSpawnIndex < corrSpawnDatas.Count)
		{
			activeCorridors.Add(SpawnCorridor(0));
			curCorrSpawnIndex++;
			CorridorSpawnData nextCorrSpawnData = GetNextCorrSpawnData();
			if(nextCorrSpawnData != null)
			{
				nextCorrSpawnTime = GetNextCorrSpawnData().time;
			}
		}
	}

	void FixedUpdate()
	{
		UpdateCorridor ();
	}

	void UpdateCorridor()
	{
		foreach(GameObject corridor in activeCorridors)
		{
			corridor.transform.position += corridorV * Time.fixedDeltaTime; 
		}
	}

	GameObject SpawnCorridor(int type)
	{
		GameObject corr = Instantiate (corridors [type]) as GameObject;
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
}
