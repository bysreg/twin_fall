using UnityEngine;
using System.Collections;

public class EndingController : MonoBehaviour {
	public MovieTexture movie;
	// Use this for initialization
	void Start () {
		movie = renderer.material.mainTexture as MovieTexture;
		movie.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
