using UnityEngine;
using System.Collections;

public class ParentControl : MonoBehaviour {
	
	public float speed;

	private GameObject parentObject;
	
	private Rect bounds;
	private float playerZPos;
	private float maxSqrRadius;
	private float maxRadius = 3.7f;
	private Vector3 centerMovableArea;
	private Vector3 revoDirection;
	private Vector3 targetRevoDirection;
	private float changeRevoTime = 4.0f;
	private float revoTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Revolute();
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
