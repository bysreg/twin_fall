using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

	public int fadeInSpeed;
	public int fadeOutSpeed;

	private bool isDone = true;
	private Color matCol;
	private Color newColor;

	private float alpha = 0;
	private float targetAlpha;
	private float velo;
	private int inc;
	private float idleTime;
	public float fadeoutTime = 2.0f;
	
	void Awake () {
		matCol = renderer.material.color;
	}

	void Update () {
		if (!isDone)
		{
			if(inc == 1)
			{
				alpha = renderer.material.color.a + Time.deltaTime * fadeInSpeed * inc;
				newColor = new Color(matCol.r, matCol.g, matCol.b, alpha);
				renderer.material.SetColor("_Color", newColor);
				isDone = (alpha >= 1);
			}
			else if(inc == -1)
			{
				alpha = renderer.material.color.a + Time.deltaTime * fadeOutSpeed * inc;
				newColor = new Color(matCol.r, matCol.g, matCol.b, alpha);
				renderer.material.SetColor("_Color", newColor);
				isDone = (alpha <= 0);
			}

			if(isDone)
			{
				if(inc == 1)
				{
					idleTime = 0;
					alpha = 1;
				}
				else
				{
					alpha = 0;
				}
			}
		}
		else if(idleTime <= fadeoutTime)
		{
			idleTime += Time.deltaTime;

			if(idleTime > fadeoutTime)
			{
				FadeOut();
			}
		}
	}

	public void FadeIn()
	{
		targetAlpha = 1f;
		isDone = false;
		inc = 1;
		idleTime = 0;
	}

	public void FadeOut()
	{
		targetAlpha = 0f;
		isDone = false;
		inc = -1;
		idleTime = 0;
	}
}
