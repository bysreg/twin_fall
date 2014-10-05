using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed;

	private GameController gameController;
	private LeapController leapController;
	private GameObject parentObject;

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
	private Transform model;
	private Transform combinedModel;

	private Animator animator;

	//private float revoVel;

	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		leapController = GameObject.Find ("LeapController").GetComponent<LeapController>();

		parentObject = GameObject.Find("parentObject");

		playerZPos = transform.position.z;
		maxSqrRadius = maxRadius * maxRadius;
		centerMovableArea = new Vector3(gameController.GetMainCam().transform.position.x, gameController.GetMainCam().transform.position.y, playerZPos);
		index = (name == "Bob" ? 0 : 1);
		revoDirection = new Vector3(Random.Range(100, 200), Random.Range(100, 200), Random.Range(100, 200));

		model = transform.GetChild(0);
		combinedModel = transform.GetChild (1);

		animator = model.GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		if(gameController.simulateWithKeyboard)
		{
			Vector3 v = Vector3.zero;

			if (index == 0)
			{
				if(Input.GetKey(KeyCode.A))
				{
					//left
					v.x = -speed;
				}
				
				if(Input.GetKey(KeyCode.D))
				{
					//right
					v.x = speed;
				}
				
				if(Input.GetKey(KeyCode.W))
				{
					//up
					v.y = speed;
				}
				
				if(Input.GetKey(KeyCode.S))
				{
					//down
					v.y = -speed;
				}
			}
			else if(index == 1)
			{
				if(Input.GetKey(KeyCode.LeftArrow))
				{
					//left
					v.x = -speed;
				}
				
				if(Input.GetKey(KeyCode.RightArrow))
				{
					//right
					v.x = speed;
				}
				
				if(Input.GetKey(KeyCode.UpArrow))
				{
					//up
					v.y = speed;
				}
				
				if(Input.GetKey(KeyCode.DownArrow))
				{
					//down
					v.y = -speed;
				}
			}

			transform.position += v * Time.fixedDeltaTime;
		}
		else
		{
			if(index == 0)
			{
				transform.position = new Vector3(leapController.bobPosition.x, leapController.bobPosition.y, playerZPos);
			}
			else if(index == 1)
			{
				transform.position = new Vector3(leapController.pewPosition.x, leapController.pewPosition.y, playerZPos);
			}
		}

		if (index == 0)
						leapController.bobPosition = transform.position;
				else if (index == 1)
						leapController.pewPosition = transform.position;

		//check radius
//		if(index == 0)
//			print ((transform.position - centerMovableArea).sqrMagnitude);
		if((transform.position - centerMovableArea).sqrMagnitude > maxSqrRadius)
		{
			Ray ray = new Ray(centerMovableArea, transform.position - centerMovableArea);
			//Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
			transform.position = ray.GetPoint(maxRadius);
		}
		if (leapController.proximity < 1)
		{
			parentObject.SetActive(true);
			transform.parent = parentObject.transform;

			transform.localPosition = (index == 0) ? transform.parent.forward*0.3f: -transform.parent.forward*0.3f;
			keepEyeContact();

		}
		else if (leapController.proximity > 2){
			animator.SetBool("isCombined", false);
			model.localScale = new Vector3(15f, 15f, 15f);
			combinedModel.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			transform.parent = null;
			Revolute();
		}
		else{
			animator.SetBool("isCombined", true);
			model.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			combinedModel.localScale = new Vector3(15f, 15f, 15f);
			RotateToCombine();
		}
	}

	void keepEyeContact()
	{
		GameObject otherBird;
		if (index == 0)
			otherBird = GameObject.Find("Pew/CombineBird");
		else
			otherBird = GameObject.Find("Bob/CombineBird");

		combinedModel.transform.LookAt (otherBird.transform.position + otherBird.transform.up*2);
	}

	void RotateToCombine()
	{
		Quaternion newRotation;
		if (index == 0){
			if (leapController.bobIsRight)
				newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (290, 270, 0), leapController.proximity-1);
			else
				newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (38, 94, 0), leapController.proximity-1);
			combinedModel.rotation = newRotation;
		}
		else if (index == 1){
			if (leapController.bobIsRight)
				newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (38, 94, 0), leapController.proximity-1);
			else
				newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (290, 270, 0), leapController.proximity-1);
			combinedModel.rotation = newRotation;
		}
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

		model.Rotate(revoDirection * Time.fixedDeltaTime);
	}
}
