using UnityEngine;
using System.Collections;

public class CC_Arrogant : CommandoCardBase {

	public CC_Arrogant()
	{
		mTitle = "Arrogant";
		mDescription = "Gain 1 Action, if you have more ships than any other alliance.";
		mActType = ActivationType.TurnStart;
	}
	
	public override void Action()
	{
		Alliance mostShipAll = GameLogic.Instance.ActiveAlliance;
		for(int i = 0; i < GameLogic.Instance.AllianceList.Count; i++)
		{
			if(GameLogic.Instance.AllianceList[i] == GameLogic.Instance.ActiveAlliance)
				continue;

			if(GameLogic.Instance.AllianceList[i].GetAmountShipsOnBoard() >= mostShipAll.GetAmountShipsOnBoard())
				mostShipAll = GameLogic.Instance.AllianceList[i];
		}

		Debug.Log(mostShipAll.AllianceName);

		if(mostShipAll == GameLogic.Instance.ActiveAlliance)
		{
			GameLogic.Instance.ActiveAlliance.ActionsLeft++;
		}
	}
}
