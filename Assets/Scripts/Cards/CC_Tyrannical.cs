using UnityEngine;
using System.Collections;

public class CC_Tyrannical : CommandoCardBase {

	public CC_Tyrannical()
	{
		mTitle = "Tyrannical";
		mDescription = "Free Action: Reduce Research by 1, Increase Dominance by 1.";
		mActType = ActivationType.OnUse;
	}
	
	public override void Action ()
	{
		if(GameLogic.Instance.AllianceLooseResearch(GameLogic.Instance.ActiveAlliance, 1))
		{
			GameLogic.Instance.AllianceGainDominance(1);
			UsedThisTurn = true;
		}
		else
		{
			GameLogic.Instance.msgWindow.SetMessage("Cannot reduce Research below 1!");
		}
	}
}
