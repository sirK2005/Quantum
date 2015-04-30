using UnityEngine;
using System.Collections;

public class GameBeginMenue : ScreenBase {

	// Use this for initialization
	void Start () {
	
	}

	public void LetsGoPressed()
	{
		GameLogic.Instance.ResolveStartUp();
		Hide ();
	}
}
