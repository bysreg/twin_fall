using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour {

	public MovieTexture movie;
	// Use this for initialization
	void Start () {
		movie = renderer.material.mainTexture as MovieTexture;
		movie.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > movie.duration)
			Application.LoadLevel(1);
	}
}
