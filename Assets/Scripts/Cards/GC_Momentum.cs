using UnityEngine;
using System.Collections;

public class GC_Momentum : GambitCardBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GC_Momentum()
	{
		mTitle = "Momentum";
		mDescription = "Immediately take another turn with 2 Actions.";
	}
	
	public override void Action()
	{
		GameLogic.Instance.AddPhaseAtBeginning(new Phase(PhaseType.ActionPhase, 2, GameLogic.Instance.ActiveAlliance.PlayerOrderPos));
	}
}
