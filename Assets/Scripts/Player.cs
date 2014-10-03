using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed;
	public int index;	

	private GameController gameController;
	private Rect bounds;

	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
		bounds = gameController.bounds;
	}

	void Update()
	{
		Vector3 v = Vector3.zero;

		if (index == 0 && gameController.simulateWithKeyboard)
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
		else if(index == 1 && gameController.simulateWithKeyboard)
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
		
		float x = Mathf.Clamp(transform.position.x, bounds.xMin, bounds.xMax);
		float y = Mathf.Clamp(transform.position.y, bounds.yMin, bounds.yMax);
		transform.position = new Vector3(x, y, transform.position.z);
	}

}
