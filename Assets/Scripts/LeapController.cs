using UnityEngine;
using System.Collections;
using Leap;

public class LeapController : MonoBehaviour {
	public GameObject Bob;
	public GameObject Pew;

	public Vector3 bobPosition;
	public Vector3 pewPosition;

	private Camera _mainCam;
	private LeapManager _leapManager;

	private static Controller _leapController = new Controller();
	private static Frame _currentFrame = Frame.Invalid;

	private int oldHand1Id = 0, oldHand2Id = 0;

	// Use this for initialization
	void Start () {
		_mainCam = (GameObject.FindGameObjectWithTag("MainCamera")as GameObject).GetComponent(typeof(Camera)) as Camera;
		_leapManager = (GameObject.Find("LeapManager")as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
	}
	
	// Update is called once per frame
	void Update () {
		_currentFrame = _leapController.Frame();
		if(_leapManager != null) { 
			if(_leapManager.pointerAvailible)
			{
				Hand hand1, hand2;
				int hand1Id, hand2Id;
				hand1Id = _currentFrame.Hands[0].Id;
				hand2Id = _currentFrame.Hands[1].Id;


				if (hand1Id == oldHand2Id || hand2Id == oldHand1Id)
				{
					hand1 = _currentFrame.Hands[1];
					hand2 = _currentFrame.Hands[0];

				}
				else
				{
					hand1 = _currentFrame.Hands[0];
					hand2 = _currentFrame.Hands[1];

					oldHand1Id = hand1Id;	//OldHand is updated only when the hands haven't swapped, because if they have,
					oldHand2Id = hand2Id;	//we need to remember the actual orientation
				}

				if (hand1 != Hand.Invalid)
				{
					Finger pointer_finger = LeapManager.pointingFigner(hand1);
					bobPosition = pointer_finger.TipPosition.ToUnityTranslated();
					bobPosition = new Vector3(Bob.transform.position.x, Bob.transform.position.y, 0);
				}


				if (hand2 != Hand.Invalid)
				{
					Finger pointer_finger = LeapManager.pointingFigner(hand2);
					pewPosition = pointer_finger.TipPosition.ToUnityTranslated();
					pewPosition = new Vector3(Pew.transform.position.x, Pew.transform.position.y, 0);
				}
			}
			else
			{
			}
		}
	}
}
