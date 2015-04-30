using UnityEngine;
using System.Collections;

public class CC_Cerebral : CommandoCardBase {

	public CC_Cerebral ()
	{
		mTitle = "Cerebral";
		mDescription = "Free Action: Reduce Dominance by 1, Increase Research by 3.";
		mActType = ActivationType.OnUse;
	}
	
	public override void Action ()
	{
		if(GameLogic.Instance.AllianceLooseDominance(GameLogic.Instance.ActiveAlliance, 1))
		{
			GameLogic.Instance.AllianceGainResearch(3);
			UsedThisTurn = true;
		}
		else
		{
			GameLogic.Instance.msgWindow.SetMessage("Cannot reduce Dominance below 1!");
		}
	}
}
