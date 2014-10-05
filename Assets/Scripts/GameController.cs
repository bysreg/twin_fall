using UnityEngine;
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

	private float time;
	private GameObject[] corridors;
	private GameObject[] collectibles;
	private List<GameObject> activeCorridors;
	private List<GameObject> activeColl;
	private List<CorridorSpawnData> corrSpawnDatas;
	private List<CollSpawnData> collSpawnDatas;
	private int curCorrSpawnIndex;
	private int curOldestCorrIndex;
	private int curOldestCollIndex;
	private int curCollSpawnIndex;
	private float nextCorrSpawnTime;
	private float nextCollSpawnTime;
	private Vector2 nextCollSpawnPos;
	private GameObject[] trunks;
	private GameObject curTrunk;
	private GameObject nextTrunk;

	//combo system
	private int comboCount;
	private bool isPrevHit;

	public class CorridorSpawnData
	{
		public float spawnTime;
		public float hitTime;
	}

	public class CollSpawnData
	{
		public float spawnTime;
		public float hitTime;
		public Vector2 position;
	}

	void Awake()
	{
		player = GameObject.Find("Bob");
		player2 = GameObject.Find("Pew");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		corridors = GameObject.FindGameObjectsWithTag ("Corridor");
		collectibles = GameObject.FindGameObjectsWithTag("Collectible");
		corrSpawnDatas = new List<CorridorSpawnData> ();
		collSpawnDatas = new List<CollSpawnData> ();
		activeCorridors = new List<GameObject> ();
		activeColl = new List<GameObject>();

		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		player2.transform.position = new Vector3 (mainCam.transform.position.x + 3, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);

		InitializeCorridorSpawnDatas();
		InitializeCollectibleSpawnDatas();

		trunks = new GameObject[2];
		trunks[0] = GameObject.Find("/trunks_new/Trunk1");
		trunks[1] = GameObject.Find("/trunks_new/Trunk2");
		curTrunk = trunks[0];
		nextTrunk = trunks[1];
	}
	
	void InitializeCollectibleSpawnDatas()
	{
		string collectiblesSpawnDataString = collectiblesSpawnDataText.text;
		string[] collLines = collectiblesSpawnDataString.Split(new char[] {'\n'});
		float s = player.transform.position.z - corridors [0].transform.position.z;
		float deltaTime = Mathf.Abs(s / corridorV.z); // time from spawn to reach player 

		foreach(string line in collLines)
		{
			if(line.Length == 0)
			{
				continue;
			}

			CollSpawnData collSpawnData = new CollSpawnData();
			string[] splits = line.Split(new char[] {' '});
			collSpawnData.spawnTime = float.Parse(splits[0]) - deltaTime;
			collSpawnData.hitTime = float.Parse(splits[0]);
			float x = float.Parse(splits[1]);
			float y = float.Parse(splits[2]);
			collSpawnData.position = new Vector2(x, y);

			collSpawnDatas.Add(collSpawnData);
		}

		nextCollSpawnTime = GetNextCollSpawnData().spawnTime;
		nextCollSpawnPos = GetNextCollSpawnData().position;
	}

	void InitializeCorridorSpawnDatas()
	{
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
			corrSpawnData.spawnTime = float.Parse(line) - deltaTime;
			corrSpawnData.hitTime = float.Parse(line);
			corrSpawnDatas.Add(corrSpawnData);
		}
		
		nextCorrSpawnTime = GetNextCorrSpawnData ().spawnTime;
	}

	void Update()
	{
		time += Time.deltaTime;

		UpdateSpawnCorr();
		UpdateSpawnCollectibles();
	}

	void FixedUpdate()
	{
		UpdateCorridor ();
		UpdateCollectibles();
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
				nextCorrSpawnTime = nextCorrSpawnData.spawnTime;
			}
		}
	}

	void UpdateSpawnCollectibles()
	{
		if(time >= nextCollSpawnTime && curCollSpawnIndex < collSpawnDatas.Count)
		{
			activeColl.Add(SpawnColl(nextCollSpawnPos.x, nextCollSpawnPos.y));
			curCollSpawnIndex++;
			CollSpawnData nextCollSpawnData = GetNextCollSpawnData();
			if(nextCollSpawnData != null)
			{
				nextCollSpawnTime = nextCollSpawnData.spawnTime;
				nextCollSpawnPos = nextCollSpawnData.position;
			}
		}
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
				curOldestCorrIndex++;
				Destroy(corridor);
				continue;
			}
		}
	}

	void UpdateCollectibles()
	{
		for(int i=activeColl.Count - 1; i >= 0; i--)
		{
			var coll = activeColl[i];

			if(coll.GetComponent<CollectibleColliderCheck>().IsHit())
			{
				activeColl.Remove(coll);
				Destroy(coll);
				curOldestCollIndex++;
				continue;
			}

			coll.transform.position += corridorV * Time.fixedDeltaTime;

			//delete the coll if it's not used anymore, this means that this collectible is not hit by the player
			if(coll.transform.position.z < -10)
			{
				activeColl.Remove(coll);
				Destroy(coll);
				curOldestCollIndex++;
				CancelCombo();
				continue;
			}
		}
	}

	void UpdateTrunk()
	{
		for(int i=0;i < trunks.Length;i++)
		{
			trunks[i].transform.position += (corridorV) * Time.fixedDeltaTime;
		}

		for(int i=0;i < trunks.Length; i++)
		{
			if(trunks[i].transform.position.z < -186)
			{
				trunks[i].transform.position = trunks[(i + 1) % 2].transform.position + new Vector3(0, 0, 186.384f);

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

	GameObject SpawnColl(float x, float y)
	{
		GameObject coll = Instantiate (collectibles[0]) as GameObject;
		coll.transform.position = new Vector3(x, y, 50);
		return coll;
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

	public CollSpawnData GetNextCollSpawnData()
	{
		if(collSpawnDatas.Count > curCollSpawnIndex)
			return collSpawnDatas[curCollSpawnIndex];

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

	public float GetCurOldestCorrHitTime()
	{
		if(curOldestCorrIndex < corrSpawnDatas.Count)
			return corrSpawnDatas[curOldestCorrIndex].hitTime;
		return 0;
	}

	public float GetCurOldestCollHitTime()
	{
		if(curOldestCollIndex < collSpawnDatas.Count)
			return collSpawnDatas[curOldestCollIndex].hitTime;
		return 0;
	}

	public int GetComboCount()
	{
		return comboCount;
	}

	public void IncComboCount()
	{
		comboCount++;
	}

	public void CancelCombo()
	{
		comboCount = 0;
	}
}
