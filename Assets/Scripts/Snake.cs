using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour {

	public float frameTime;
	public Texture2D[] frames;

	private float time;
	private int frameIndex;
	private MeshRenderer meshRenderer;

	void Awake()
	{
		meshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	void Update()
	{
		time += Time.deltaTime;
		if(time >= frameTime)
		{
			time -= frameTime;

			//change frame
			frameIndex = (frameIndex + 1) % frames.Length;
			meshRenderer.material.mainTexture = frames[frameIndex];
		}
	}
}
