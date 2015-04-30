using UnityEngine;
using System.Collections;

public class GC_Aggression : GambitCardBase {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public GC_Aggression()
	{
		mTitle = "Aggression";
		mDescription = "Immediately gain 2 Dominance.";
	}

	public override void Action()
	{
		GameLogic.Instance.AllianceGainDominance(2);
	}
}
