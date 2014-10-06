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
	public TextAsset beatCollectiblesSpawnDataText;
	public AudioClip[] comboSound;

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
	private GameObject[] trunks;
	private GameObject curTrunk;
	private GameObject nextTrunk;
	private GameObject parent;

	//next collectibles to be spawned
	private float nextCollSpawnTime;
	private Vector2 nextCollSpawnPos;
	private CollSpawnData.Type nextCollSpawnType;

	//combo system
	private int comboCount; // dont modify combocount directly
	private bool isPrevHit;
	private Fade[] comboPops;

	//snakes
	public float snakeMinTime; // minimum time to spawn the snake for the first time
	public Vector2 minMaxSnakeSpawnTime; // minimum and maximum snake spawn time
	public float snakeShowUpZ;
	private float nextSnakeSpawnTime; //time to spawn next snake
	private GameObject snakeInstance; // original snake gameobject to be cloned
	private List<GameObject> activeSnakes;

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
		public enum Type{
			Melody = 0,
			Beat,
		}

		public Type type;
	}

	void Awake()
	{
		player = GameObject.Find("Bob");
		player2 = GameObject.Find("Pew");
		parent = GameObject.Find("parentObject");
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		corridors = GameObject.FindGameObjectsWithTag ("Corridor");
		collectibles = new GameObject[2];
		collectibles[0] = GameObject.Find("Collectibles/Feather");
		collectibles[1] = GameObject.Find("Collectibles/BeatFeather");
		corrSpawnDatas = new List<CorridorSpawnData> ();
		collSpawnDatas = new List<CollSpawnData> ();
		activeCorridors = new List<GameObject> ();
		activeColl = new List<GameObject>();
		activeSnakes = new List<GameObject>();

		player.transform.position = new Vector3 (mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		player2.transform.position = new Vector3 (mainCam.transform.position.x + 3, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);
		parent.transform.position = new Vector3 (mainCam.transform.position.x + 3, mainCam.transform.position.y, mainCam.transform.position.z + playerZDistanceFromCamera);

		InitializeCorridorSpawnDatas();
		InitializeCollectibleSpawnDatas();

		trunks = new GameObject[2];
		trunks[0] = GameObject.Find("/trunks_new/Trunk1");
		trunks[1] = GameObject.Find("/trunks_new/Trunk2");
		curTrunk = trunks[0];
		nextTrunk = trunks[1];

		//init snake
		nextSnakeSpawnTime = Random.Range(minMaxSnakeSpawnTime.x, minMaxSnakeSpawnTime.y) + snakeMinTime;
		snakeInstance = GameObject.Find("Snake");

		//combos
		comboPops = new Fade[4];
		comboPops[0] = GameObject.Find("/ComboTextures/Good").GetComponent<Fade>();
		comboPops[0].gameObject.SetActive(false);
		comboPops[1] = GameObject.Find("/ComboTextures/Cool").GetComponent<Fade>();
		comboPops[1].gameObject.SetActive(false);
		comboPops[2] = GameObject.Find("/ComboTextures/Yoho").GetComponent<Fade>();
		comboPops[2].gameObject.SetActive(false);
		comboPops[3] = GameObject.Find("/ComboTextures/Woo").GetComponent<Fade>();
		comboPops[3].gameObject.SetActive(false);
	}
	
	void InitializeCollectibleSpawnDatas()
	{
		float s = player.transform.position.z - corridors [0].transform.position.z;
		float deltaTime = Mathf.Abs(s / corridorV.z); // time from spawn to reach player 

		{
			string collectiblesSpawnDataString = collectiblesSpawnDataText.text;
			string[] collLines = collectiblesSpawnDataString.Split(new char[] {'\n'});


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
				collSpawnData.type = CollSpawnData.Type.Melody;

			collSpawnDatas.Add(collSpawnData);
		}
		}

		{
			string collectiblesSpawnDataString = beatCollectiblesSpawnDataText.text;
			string[] collLines = collectiblesSpawnDataString.Split(new char[] {'\n'});
			
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
				collSpawnData.type = CollSpawnData.Type.Beat;
				
				collSpawnDatas.Add(collSpawnData);
			}
		}

		collSpawnDatas.Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));

		//debugging
//		foreach( var data in collSpawnDatas)
//		{
//			print (data.spawnTime + " " + data.type);
//		}

		nextCollSpawnTime = GetNextCollSpawnData().spawnTime;
		nextCollSpawnPos = GetNextCollSpawnData().position;
		nextCollSpawnType = GetNextCollSpawnData().type;
	}

	void InitializeCorridorSpawnDatas()
	{
		string spawnDataContent = corrSpawnDataText.text;
		string[] lines = spawnDataContent.Split (new char[] {'\n'});
		float s = player.transform.position.z - corridors [0].transform.position.z;
		float deltaTime = Mathf.Abs(s / corridorV.z); // time from spawn to reach player 
		print ("delta time : " + deltaTime);
		
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
		UpdateSpawnSnakes();
	}

	void FixedUpdate()
	{
		UpdateCorridor ();
		UpdateCollectibles();
		UpdateTrunk();
		UpdateSnakes ();
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
			activeColl.Add(SpawnColl(nextCollSpawnPos.x, nextCollSpawnPos.y, nextCollSpawnType));
			curCollSpawnIndex++;
			CollSpawnData nextCollSpawnData = GetNextCollSpawnData();
			if(nextCollSpawnData != null)
			{
				nextCollSpawnTime = nextCollSpawnData.spawnTime;
				nextCollSpawnPos = nextCollSpawnData.position;
				nextCollSpawnType = nextCollSpawnData.type;
			}
		}
	}

	void UpdateSpawnSnakes()
	{
		if(time >= nextSnakeSpawnTime)
		{
			activeSnakes.Add(SpawnSnake());
			nextSnakeSpawnTime = time + Random.Range(minMaxSnakeSpawnTime.x, minMaxSnakeSpawnTime.y);
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
			CollectibleColliderCheck ccc = coll.GetComponent<CollectibleColliderCheck>();

			if(ccc.IsHit())
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
				if(ccc.type == CollSpawnData.Type.Beat)
				{
					CancelCombo();
				}
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

	void UpdateSnakes()
	{
		for(int i = activeSnakes.Count - 1; i >= 0; i--)
		{
			var snake = activeSnakes[i];
			snake.transform.position += corridorV * Time.fixedDeltaTime;

			if(snake.transform.position.z < -10)
			{
				activeSnakes.Remove(snake);
				Destroy(snake);
				continue;
			}

			if(snake.transform.position.z <= snakeShowUpZ && snake.transform.position.z >= snakeShowUpZ - 3)
			{
				snake.transform.position += snake.transform.up * 35.5f * Time.fixedDeltaTime;
			}
			else if(snake.transform.position.z < snakeShowUpZ - 3)
			{
				var snakeComp = snake.GetComponent<Snake>();
				if(!snakeComp.play)
					snakeComp.play = true;
			}
		}

	}

	GameObject SpawnCorridor(int type)
	{
		GameObject corr = Instantiate (corridors [type]) as GameObject;
		corr.transform.position = new Vector3(0, 1, 50);
		return corr;
	}

	GameObject SpawnColl(float x, float y, CollSpawnData.Type type)
	{
		GameObject coll = null;
//		print ("spawn coll : " + type);
		if(type == CollSpawnData.Type.Melody)
		{
			coll = Instantiate (collectibles[0]) as GameObject;
		}
		else if(type == CollSpawnData.Type.Beat)
		{
			coll = Instantiate (collectibles[1]) as GameObject;
		}
		coll.transform.position = new Vector3(x, y, 50);
		return coll;
	}

	GameObject SpawnSnake()
	{
		GameObject snake = Instantiate(snakeInstance) as GameObject;
		float x = -0.08752429f;
		snake.transform.position = new Vector3(x, 1, 50);
		snake.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
		return snake;
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

	public void IncComboCount(Vector3 position)
	{
		comboCount++;

		//play combo sound
		int type = comboCount <= comboSound.Length ? comboCount - 1 : comboSound.Length - 1;
		AudioSource.PlayClipAtPoint(comboSound[type], mainCam.transform.position);
		//display combo text
		ShowComboPop(type, position);
	}

	public void ShowComboPop(int type, Vector3 position)
	{
		comboPops[type].gameObject.SetActive(true);
		comboPops[type].gameObject.transform.position = position;
		comboPops[type].FadeIn();
	}

	public void CancelCombo()
	{
		comboCount = 0;
	}
}
