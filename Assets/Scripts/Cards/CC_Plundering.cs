using UnityEngine;
using System.Collections;

public class CC_Plundering : CommandoCardBase {

	public CC_Plundering()
	{
		mTitle = "Plundering";
		mDescription = "If you destroey at least one ship this turn, gain 3 Research.";
		mActType = ActivationType.TurnEnd;
	}
	
	public override void Action ()
	{
		if(GameLogic.Instance.ShipsDestroyedThisTurn > 0)
			GameLogic.Instance.AllianceGainResearch(3);
	}
}
