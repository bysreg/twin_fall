using UnityEngine;
using System.Collections;

public class ParentController : MonoBehaviour {

	private GameController gameController;
	private LeapController leapController;
	private GameObject Bob, Pew;
	private GameObject mainCam;

	private Rect bounds;
	private float playerZPos;
	private float maxSqrRadius;
	private float maxRadius = 3.7f;
	private Vector3 centerMovableArea;
	private int index;
	private Vector3 revoDirection;
	private Vector3 targetRevoDirection;
	private float changeRevoTime = 4.0f;
	private float revoTime;

	// Use this for initialization
	void Start () {
		leapController = GameObject.Find ("LeapController").GetComponent<LeapController>();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = leapController.meanPosition;
		float z = mainCam.transform.position.z + gameController.playerZDistanceFromCamera;
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
		transform.Rotate (2f, 2f, 2f);
		//Revolute ();
	}

	void Revolute()
	{
		revoTime += Time.fixedDeltaTime;
		
		if(revoTime >= changeRevoTime)
		{
			revoTime -= changeRevoTime;
			targetRevoDirection = new Vector3(Random.Range(100, 200), Random.Range(100, 200), Random.Range(100, 200));
			revoDirection = targetRevoDirection;
			//revoVel = 0f;
		}
		
		transform.Rotate(revoDirection * Time.fixedDeltaTime);
	}
}
