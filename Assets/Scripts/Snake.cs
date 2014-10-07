using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

	public float frameTime;
	public Texture2D[] frames;
	public AudioClip hissSound;

	private float time;
	private int frameIndex;
	private MeshRenderer meshRenderer;
	public bool play;
	private bool snakePlaySound;

	void Awake()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		play = false;
	}

	void Update()
	{
		if(!play)
			return;

		PlaySound();

		time += Time.deltaTime;
		if(time >= frameTime)
		{
			time -= frameTime;

			//change frame
			frameIndex++;
			if(frameIndex >= frames.Length)
			{
				play = false;
				return;
			}

			//print (frameIndex);
			meshRenderer.material.mainTexture = frames[frameIndex];
		}
	}

	void PlaySound()
	{
		if(!snakePlaySound)
		{
			snakePlaySound = true;
			AudioSource.PlayClipAtPoint(hissSound, transform.position);
		}
	}
}
