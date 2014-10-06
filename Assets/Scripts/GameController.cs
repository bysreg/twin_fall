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
	public AudioClip[] comboSound;

	private GameObject player;
	private GameObject player2;
	private GameObject mainCam;
	private float playerZPos;

	private float time;

	private GameObject[] collectibles;
	private List<GameObject> activeColl;
	private List<CollSpawnData> collSpawnDatas;
	private int curOldestCollIndex;
	private int curCollSpawnIndex;
	private GameObject[] trunks;
	private GameObject curTrunk;
	private GameObject nextTrunk;
	private GameObject parent;
	private bool isFinished;

	//corridors
	private GameObject[] corridors;
	private int[] corridorsHole;
	private Vector3[][] corrHolePositions; // world position
	private List<GameObject> activeCorridors;
	private List<CorridorSpawnData> corrSpawnDatas;
	private int curCorrSpawnIndex;
	private int curOldestCorrIndex;
	private CorridorSpawnData nextCorrSpawn;
	private readonly Vector3 corrSpawnPoint = new Vector3(0, 1, 50);

	private float possibleFinishedTime;

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
		public int type;
		public Vector3[] holeWPos;
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
		corridorsHole = new int[corridors.Length];
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
		playerZPos = player.transform.position.z;

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
		InitializeCombos();
	}

	void InitializeCombos()
	{
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

		//corridor holes
		corrHolePositions = new Vector3[corridors.Length][];
		for(int i = 0; i < corridors.Length; i++)
		{
			int holeCount = corridors[i].transform.Find("Holes").childCount;
			corridorsHole[i] = holeCount;
			corrHolePositions[i] = new Vector3[holeCount];
			
			for(int j = 0; j < holeCount; j++)
			{
				corrHolePositions[i][j] = corridors[i].transform.Find("Holes/" + (j + 1)).transform.localPosition;
			}
		}

		foreach(string line in lines)
		{
			if(line.Length == 0)
			{
				continue;
			}
			
			CorridorSpawnData corrSpawnData = new CorridorSpawnData();
			string[] splits = line.Split(new char[] {' '});
			corrSpawnData.spawnTime = float.Parse(splits[0]) - deltaTime;
			corrSpawnData.hitTime = float.Parse(splits[0]);
			corrSpawnData.type = Random.Range(0, corridors.Length);

			DetermineHolePos(ref corrSpawnData);

			corrSpawnDatas.Add(corrSpawnData);
		}

		nextCorrSpawn = GetNextCorrSpawnData();
	}

	void DetermineHolePos(ref CorridorSpawnData data)
	{
		//need to spawn beat feathers
		int corrType = data.type;
		int holeCount = corridorsHole[corrType];
		GameObject corr = corridors[data.type];
		Vector3 oriCorrPos = corr.transform.position;
		corr.transform.position = corrSpawnPoint;

		if(holeCount < 3)
		{
			data.holeWPos = new Vector3[holeCount];

			for(int i=0; i < holeCount; i++)
			{
				//convert local pos to world pos
				Vector3 worldBeatPos = corr.transform.TransformPoint(corrHolePositions[corrType][i]);
				data.holeWPos[i] = worldBeatPos;
			}
		}
		else if(holeCount == 3) // for now
		{
			data.holeWPos = new Vector3[2];
			int first = Random.Range(0, holeCount);
			int second = Random.Range(1, holeCount - 1);
			second = (first + second) % holeCount;
			
			//first beat
			Vector3 worldBeatPos = corr.transform.TransformPoint(corrHolePositions[corrType][first]);
			data.holeWPos[0] = worldBeatPos;

			//second beat
			worldBeatPos = corr.transform.TransformPoint(corrHolePositions[corrType][second]);
			data.holeWPos[1] = worldBeatPos;

		}

		corridors[data.type].transform.position = oriCorrPos;
	}

	void Update()
	{
		time += Time.deltaTime;

		UpdateSpawnCorr();
		UpdateSpawnCollectibles();
		UpdateSpawnSnakes();

		Credits ();
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
		if(nextCorrSpawn == null)
			return;

		if(time >= nextCorrSpawn.spawnTime && curCorrSpawnIndex < corrSpawnDatas.Count)
		{
			int corrType = nextCorrSpawn.type;

			GameObject corr = SpawnCorridor(corrType);
			activeCorridors.Add(corr);

			foreach(var holeWPos in nextCorrSpawn.holeWPos)
			{
				activeColl.Add(SpawnColl(holeWPos.x, holeWPos.y, CollSpawnData.Type.Beat));
			}

			curCorrSpawnIndex++;
			nextCorrSpawn = GetNextCorrSpawnData();
			if(nextCorrSpawn == null)
			{
				// TODO : mark corr as finished
			}
		}
	}

	void UpdateSpawnCollectibles()
	{
		if(time >= nextCollSpawnTime && curCollSpawnIndex < collSpawnDatas.Count)
		{
			if(nextCorrSpawn != null)
			{
				// if there will be two holes on incoming wall. just randomly select one hole
				int random = Random.Range(0, nextCorrSpawn.holeWPos.Length);
				//decide which one is the closer one to the hole
				Vector3 diff1 = nextCorrSpawn.holeWPos[random] - player.transform.position;
				Vector3 diff2 = nextCorrSpawn.holeWPos[random] - player2.transform.position;
				float sqrDist1 = diff1.sqrMagnitude;
				float sqrDist2 = diff2.sqrMagnitude;
				float x, y;
				if(sqrDist1 < sqrDist2)
				{
					x = diff1.x / 2.0f + player.transform.position.x;
					y = diff1.y / 2.0f + player.transform.position.y;
				}
				else
				{
					x = diff2.x / 2.0f + player.transform.position.x;
					y = diff2.y / 2.0f + player.transform.position.y;
				}
				//print (x + " " + y);

				activeColl.Add(SpawnColl(x, y, nextCollSpawnType));
			}
			else
			{
				activeColl.Add(SpawnColl(0, 0, nextCollSpawnType));
			}

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
		corr.transform.position = corrSpawnPoint;
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
		if(nextCorrSpawn != null)
			return nextCorrSpawn.spawnTime;

		return 0;
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
		ChangeComboNumber (comboCount);

		//play combo sound
		int type = comboCount <= comboSound.Length ? comboCount - 1 : comboSound.Length - 1;
		AudioSource.PlayClipAtPoint(comboSound[type], mainCam.transform.position);
		//display combo text
		ShowComboPop(type, position);
	}

	public void ShowComboPop(int type, Vector3 position)
	{
		var combo = comboPops[type];
		combo.gameObject.SetActive(true);
		switch(type)
		{
		case 0:
			combo.gameObject.transform.position = new Vector3(position.x, position.y, playerZPos - 0.1f);
			break;
		case 1:
			combo.gameObject.transform.position = new Vector3(position.x, position.y, playerZPos - 0.2f);
			break;
		case 2:
			combo.gameObject.transform.position = new Vector3(position.x, position.y, playerZPos - 0.3f);
			break;
		case 3:
		default:
			combo.gameObject.transform.position = new Vector3(position.x, position.y, playerZPos - 0.4f);
			break;
		}
		combo.FadeIn();
	}

	public void CancelCombo()
	{
		comboCount = 0;
		ChangeComboNumber (comboCount);
	}

	public void ChangeComboNumber(int combo)
	{
		if (combo < 10) {
			blitNumber (combo, 1);
		}
		else{
			blitNumber(combo/10, 1);
			blitNumber (combo%10, 2);
		}
	}

	public void blitNumber(int digit, int position)
	{
		GUITexture zero = GameObject.Find("ComboTextures/0").GetComponent<GUITexture>();
		GUITexture one = GameObject.Find("ComboTextures/1").GetComponent<GUITexture>();
		if (position == 1){
			switch(digit)
			{
				case 0:
					zero.texture = Resources.Load ("0") as Texture;
					break;
				case 1:
					zero.texture = Resources.Load ("1") as Texture;
					break;
				case 2:
					zero.texture = Resources.Load ("2") as Texture;
					break;
				case 3:
					zero.texture = Resources.Load ("3") as Texture;
					break;
				case 4:
					zero.texture = Resources.Load ("4") as Texture;
					break;
				case 5:
					zero.texture = Resources.Load ("5") as Texture;
					break;
				case 6:
					zero.texture = Resources.Load ("6") as Texture;
					break;
				case 7:
					zero.texture = Resources.Load ("7") as Texture;
					break;
				case 8:
					zero.texture = Resources.Load ("8") as Texture;
					break;
				case 9:
					zero.texture = Resources.Load ("9") as Texture;
					break;
			}
		}
		else if (position == 2){
			switch(digit)
			{
			case 0:
				one.texture = Resources.Load ("0") as Texture;
				break;
			case 1:
				one.texture = Resources.Load ("1") as Texture;
				break;
			case 2:
				one.texture = Resources.Load ("2") as Texture;
				break;
			case 3:
				one.texture = Resources.Load ("3") as Texture;
				break;
			case 4:
				one.texture = Resources.Load ("4") as Texture;
				break;
			case 5:
				one.texture = Resources.Load ("5") as Texture;
				break;
			case 6:
				one.texture = Resources.Load ("6") as Texture;
				break;
			case 7:
				one.texture = Resources.Load ("7") as Texture;
				break;
			case 8:
				one.texture = Resources.Load ("8") as Texture;
				break;
			case 9:
				one.texture = Resources.Load ("9") as Texture;
				break;
			}
		}
	}

	public void Credits()
	{
		if (isFinished){
			GUITexture credits = GameObject.Find("Credits/Credits").GetComponent<GUITexture>();
			GUITexture Aiden = GameObject.Find("Credits/Aiden").GetComponent<GUITexture>();
			GUITexture Hilman = GameObject.Find("Credits/Hilman").GetComponent<GUITexture>();
			GUITexture Jake = GameObject.Find("Credits/Jake").GetComponent<GUITexture>();
			GUITexture Rachel = GameObject.Find("Credits/Rachel").GetComponent<GUITexture>();
			GUITexture Vivek = GameObject.Find("Credits/Vivek").GetComponent<GUITexture>();

			float creditTime = Time.time - possibleFinishedTime;

			if (creditTime < 2);
			else if (creditTime < 3){
				credits.guiTexture.enabled = true;
				credits.transform.localScale = new Vector3(creditTime-2, creditTime-2, creditTime-2);
			}
			else if (creditTime < 4)
			{}
			else if (creditTime < 5)
			{
				credits.enabled = false;
			}

			else if (creditTime < 6)
			{
				credits.enabled = false;
				Aiden.enabled = true;
				Aiden.transform.localScale = new Vector3(creditTime-5, creditTime-5, creditTime-5);
			}
			else if (creditTime < 7);
			else if (creditTime < 8)
				Aiden.enabled = false;

			else if (creditTime < 9)
			{
				Aiden.enabled = false;
				Hilman.enabled = true;
				Hilman.transform.localScale = new Vector3(creditTime-8, creditTime-8, creditTime-8);
			}
			else if (creditTime < 10);
			else if (creditTime < 11)
				Hilman.enabled = false;

			else if (creditTime < 12)
			{
				credits.enabled = false;
				Jake.enabled = true;
				Jake.transform.localScale = new Vector3(creditTime-11, creditTime-11, creditTime-11);
			}
			else if (creditTime < 13);
			else if (creditTime < 14)
				Jake.enabled = false;

			else if (creditTime < 15)
			{
				credits.enabled = false;
				Rachel.enabled = true;
				Rachel.transform.localScale = new Vector3(creditTime-14, creditTime-14, creditTime-14);
			}
			else if (creditTime < 16);
			else if (creditTime < 17)
				Rachel.enabled = false;

			else if (creditTime < 18)
			{
				credits.enabled = false;
				Vivek.enabled = true;
				Vivek.transform.localScale = new Vector3(creditTime-17, creditTime-17, creditTime-17);
			}
			else if (creditTime < 19);
			else if (creditTime < 20)
				Vivek.enabled = false;
			else if (creditTime > 20)
				Application.LoadLevel(2);

		}
		else
			possibleFinishedTime = Time.time;
	}
}
