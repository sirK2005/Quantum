using UnityEngine;
using System.Collections;

public class MessageWindow : MonoBehaviour {

	public UISprite sprBack;
	public UILabel lblMsg;
	public UIPanel panel;

	private float mVisibility;
	private float mDefaultVisibil = 3.0f;
	private float mFadeSpd = 0.5f;

	// Use this for initialization
	void Start () {
		mVisibility = 0;

	}
	
	// Update is called once per frame
	void Update () {
		if(mVisibility > 0)
		{
			sprBack.height = lblMsg.height + 10;
			sprBack.width = lblMsg.width + 10;
			panel.alpha = mVisibility -= mFadeSpd * Time.deltaTime;
		}
	}

	public void SetMessage(string msg)
	{
		lblMsg.text = msg;

		mVisibility = mDefaultVisibil;
	}
}
