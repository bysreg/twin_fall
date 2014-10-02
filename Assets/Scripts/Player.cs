using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed;

	private GameController gameController;

	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
	}

	void Update()
	{
		if (gameController.simulateWithKeyboard)
		{
			Vector3 v = Vector3.zero;
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
			
			transform.position += v * Time.deltaTime;
		}
	}

}
