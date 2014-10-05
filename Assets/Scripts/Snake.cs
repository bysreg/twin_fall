using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

	public float frameTime;
	public Texture2D[] frames;

	private float time;
	private int frameIndex;
	private MeshRenderer meshRenderer;
	public bool play;

	void Awake()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		play = false;
	}

	void Update()
	{
		if(!play)
			return;

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
}
