using UnityEngine;
using System.Collections;

public class CC_Righteous : CommandoCardBase {

	public CC_Righteous()
	{
		mTitle = "Righteous";
		mDescription = "You do not loose Dominance when lossing a fight.";
		mActType = ActivationType.TurnStart;
	}

	public override void Action ()
	{
		GameLogic.Instance.ActiveAlliance.DomLostOnDefeat = 0;
	}
}
