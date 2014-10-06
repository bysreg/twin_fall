using UnityEngine;
using System.Collections;

public class RestartController : MonoBehaviour {

	float time;

	// Use this for initialization
	void Start () {
		time = PlayerPrefs.GetFloat ("FinalTime");
		SetTime (time);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTime(int time)
	{
		blitNumber(time%60);
		blitNumber (time/10, 1);
		blitNumber(time%10, 2);
	}
	
	public void blitNumber(int digit, int position)
	{
		GUITexture min = GameObject.Find("HUD/MinTex").GetComponent<GUITexture>();
		GUITexture sec1 = GameObject.Find("HUD/SecTex1").GetComponent<GUITexture>();
		GUITexture sec2 = GameObject.Find("HUD/SecTex2").GetComponent<GUITexture>();
		if (position == 1){
			switch(digit)
			{
			case 0:
				min.texture = Resources.Load ("0") as Texture;
				break;
			case 1:
				min.texture = Resources.Load ("1") as Texture;
				break;
			case 2:
				min.texture = Resources.Load ("2") as Texture;
				break;
			case 3:
				min.texture = Resources.Load ("3") as Texture;
				break;
			case 4:
				min.texture = Resources.Load ("4") as Texture;
				break;
			case 5:
				min.texture = Resources.Load ("5") as Texture;
				break;
			case 6:
				min.texture = Resources.Load ("6") as Texture;
				break;
			case 7:
				min.texture = Resources.Load ("7") as Texture;
				break;
			case 8:
				min.texture = Resources.Load ("8") as Texture;
				break;
			case 9:
				min.texture = Resources.Load ("9") as Texture;
				break;
			}
		}
		else if (position == 2){
			switch(digit)
			{
			case 0:
				sec1.texture = Resources.Load ("0") as Texture;
				break;
			case 1:
				sec1.texture = Resources.Load ("1") as Texture;
				break;
			case 2:
				sec1.texture = Resources.Load ("2") as Texture;
				break;
			case 3:
				sec1.texture = Resources.Load ("3") as Texture;
				break;
			case 4:
				sec1.texture = Resources.Load ("4") as Texture;
				break;
			case 5:
				sec1.texture = Resources.Load ("5") as Texture;
				break;
			case 6:
				sec1.texture = Resources.Load ("6") as Texture;
				break;
			case 7:
				sec1.texture = Resources.Load ("7") as Texture;
				break;
			case 8:
				sec1.texture = Resources.Load ("8") as Texture;
				break;
			case 9:
				sec1.texture = Resources.Load ("9") as Texture;
				break;
			}
		}
		else if (position == 3){
			switch(digit)
			{
			case 0:
				sec2.texture = Resources.Load ("0") as Texture;
				break;
			case 1:
				sec2.texture = Resources.Load ("1") as Texture;
				break;
			case 2:
				sec2.texture = Resources.Load ("2") as Texture;
				break;
			case 3:
				sec2.texture = Resources.Load ("3") as Texture;
				break;
			case 4:
				sec2.texture = Resources.Load ("4") as Texture;
				break;
			case 5:
				sec2.texture = Resources.Load ("5") as Texture;
				break;
			case 6:
				sec2.texture = Resources.Load ("6") as Texture;
				break;
			case 7:
				sec2.texture = Resources.Load ("7") as Texture;
				break;
			case 8:
				sec2.texture = Resources.Load ("8") as Texture;
				break;
			case 9:
				sec2.texture = Resources.Load ("9") as Texture;
				break;
			}
		}
}
