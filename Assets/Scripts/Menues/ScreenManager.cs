using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour {

	private UIPanel[] mUIWindows;

	// Use this for initialization
	void Start () {
		//mUIWindows = new List<UIPanel>();

		// get windows
		mUIWindows = gameObject.GetComponentsInChildren<UIPanel>(true);

		for(int i = 0; i < mUIWindows.Length; i++)
		{
			mUIWindows[i].gameObject.SetActive(true);
		}
	}
}
