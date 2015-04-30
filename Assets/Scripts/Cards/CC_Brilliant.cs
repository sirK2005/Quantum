using UnityEngine;
using System.Collections;

public class CC_Brilliant : CommandoCardBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public CC_Brilliant()
	{
		mTitle = "Brilliant";
		mDescription = "Gain 2 Research at the sart of your turn.";
		mActType = ActivationType.TurnStart;
	}

	public override void Action()
	{
		GameLogic.Instance.AllianceGainResearch(2);
	}
}
