using UnityEngine;
using System.Collections;

public class GC_Relocate : GambitCardBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GC_Relocate()
	{
		mTitle = "Relocate";
		mDescription = "Relocate a quantum cube of another alliance.";
	}
	
	public override void Action()
	{
		GameLogic.Instance.StartRelocateCube();
	}
}
