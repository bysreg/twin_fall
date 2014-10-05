using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed;

	private GameController gameController;
	private LeapController leapController;
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

	private Animator animator;

	//private float revoVel;

	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		leapController = GameObject.Find ("LeapController").GetComponent<LeapController>();
		playerZPos = transform.position.z;
		maxSqrRadius = maxRadius * maxRadius;
		centerMovableArea = new Vector3(gameController.GetMainCam().transform.position.x, gameController.GetMainCam().transform.position.y, playerZPos);
		index = (name == "Bob" ? 0 : 1);
		revoDirection = new Vector3(Random.Range(100, 200), Random.Range(100, 200), Random.Range(100, 200));

		model = transform.GetChild(0);

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

		if (leapController.proximity > 2){
			animator.SetBool("isCombined", false);
			Revolute();
		}
		else{
			animator.SetBool("isCombined", true);
			RotateToCombine();
		}
	}

	void RotateToCombine()
	{
		if (index == 0){
			Quaternion newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (290, 270, 0), leapController.proximity-1);
			Debug.Log (leapController.proximity-1);
			model.rotation = newRotation;
		}
		else if (index == 1){
			Quaternion newRotation = Quaternion.Lerp (model.transform.rotation, Quaternion.Euler (38, 94, 0), leapController.proximity-1);
			model.rotation = newRotation;
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
