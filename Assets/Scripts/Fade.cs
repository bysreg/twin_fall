using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

	public int fadeSpeed;

	private bool isDone = true;
	private Color matCol;
	private Color newColor;

	private float alpha = 0;
	private float targetAlpha;
	private float velo;
	private int inc;
	private float idleTime;
	private const float FADEOUT_TIME = 2.0f;
	
	void Awake () {
		matCol = renderer.material.color;
	}

	void Update () {

		if (!isDone)
		{
			alpha = renderer.material.color.a + Time.deltaTime * fadeSpeed * inc;
			newColor = new Color(matCol.r, matCol.g, matCol.b, alpha);
			renderer.material.SetColor("_Color", newColor);

			if(inc == 1)
			{
				isDone = (alpha >= 1);
			}
			else if(inc == -1)
			{
				isDone = (alpha <= 0);
			}

			if(isDone)
			{
				idleTime = 0;
			}
		}
		else
		{
			idleTime += Time.deltaTime;

			if(idleTime > FADEOUT_TIME)
			{

			}
		}
	}

	public void FadeIn()
	{
		targetAlpha = 1f;
		isDone = false;
		inc = 1;
	}

	public void FadeOut()
	{
		targetAlpha = 0f;
		isDone = false;
		inc = -1;
	}
}
