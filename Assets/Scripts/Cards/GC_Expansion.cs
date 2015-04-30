using UnityEngine;
using System.Collections;

public class GC_Expansion : GambitCardBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GC_Expansion()
	{
		mTitle = "Expansion";
		mDescription = "Add a ship to your fleet and deploy it.";
	}
	
	public override void Action()
	{
		if(GameLogic.Instance.ActiveAlliance.ShipList.Count < 5)
		{
			Ship newShip = GameLogic.Instance.ActiveAlliance.GetNewShip();
			newShip.ReturnToScrapyard();
			GameLogic.Instance.DeselectShip();
			GameLogic.Instance.SelectShip(newShip);
			GameLogic.Instance.StartDeployingSelectedShip();
		}
		else
			GameLogic.Instance.msgWindow.SetMessage("You cannot have more than 5 ships total.");
	}
	
}
