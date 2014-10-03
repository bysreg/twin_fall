using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour {
	
	public bool simulateWithKeyboard;
	public float playerZDistanceFromCamera;
	public Vector3 corridorV;
	public int lifes;
	public Rect bounds;
	public TextAsset spawnData;

	private GameObject player;
	private GameObject player2;
	private GameObject mainCam;

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
		player = GameObject.Find("Player1");
		player2 = GameObject.Find("Player2");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		corridors = GameObject.FindGameObjectsWithTag ("Corridor");
		corrSpawnDatas = new List<CorridorSpawnData> ();
		activeCorridors = new List<GameObject> ();

		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		player2.transform.position = new Vector3 (mainCam.transform.position.x + 3, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);

		string spawnDataContent = spawnData.text;
		string[] lines = spawnDataContent.Split (new char[] {'\n'});
		float s = player.transform.position.z - corridors [0].transform.position.z;
		float deltaTime = Mathf.Abs(s / corridorV.z); // time from spawn to reach player 
		print (deltaTime);

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
		for(int i=activeCorridors.Count -1; i >= 0; i--)
		{
			var corridor = activeCorridors[i];
			corridor.transform.position += corridorV * Time.fixedDeltaTime; 
			
			//delete the corridor if it's not used anymore
			if (corridor.transform.position.z < - 15)
			{
				activeCorridors.Remove(corridor);
				Destroy(corridor);
			}
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

	public void HitPlayer()
	{
		lifes--;
		lifes = Mathf.Max(0, lifes);
	}
}
