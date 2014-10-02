using UnityEngine;
using System.Collections;
using Leap;

public class tests : MonoBehaviour {
	public GameObject Bob;
	public GameObject Pew;

	private Camera _mainCam;
	private LeapManager _leapManager;

	private static Controller _leapController = new Controller();
	private static Frame _currentFrame = Frame.Invalid;



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
				Hand hand;
				hand = _currentFrame.Hands[0];
				if (hand != Hand.Invalid)
				{
					Finger pointer_finger = LeapManager.pointingFigner(hand);
					Bob.transform.position = pointer_finger.TipPosition.ToUnityTranslated();
				}
				/*
				foreach (Hand hand in _currentFrame.Hands)
				{
					Finger pointer_finger = LeapManager.pointingFigner(hand);
					Bob.transform.position = pointer_finger.TipPosition.ToUnityTranslated();
				}
				*/

				hand = _currentFrame.Hands[1];
				if (hand != Hand.Invalid)
				{
					Finger pointer_finger = LeapManager.pointingFigner(hand);
					Pew.transform.position = pointer_finger.TipPosition.ToUnityTranslated();
				}
			}
			else
			{
			}
		}
	}
}
