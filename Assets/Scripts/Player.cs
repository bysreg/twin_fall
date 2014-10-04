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

	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		leapController = GameObject.Find ("LeapController").GetComponent<LeapController>();
		playerZPos = transform.position.z;
		maxSqrRadius = maxRadius * maxRadius;
		centerMovableArea = new Vector3(gameController.GetMainCam().transform.position.x, gameController.GetMainCam().transform.position.y, playerZPos);
		index = (name == "Bob" ? 0 : 1);
	}

	void Update()
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

			transform.position += v * Time.deltaTime;
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

		//check radius
		if(index == 0)
			print ((transform.position - centerMovableArea).sqrMagnitude);
		if((transform.position - centerMovableArea).sqrMagnitude > maxSqrRadius)
		{
			Ray ray = new Ray(centerMovableArea, transform.position - centerMovableArea);
			//Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
			transform.position = ray.GetPoint(maxRadius);
		}
	}
}
