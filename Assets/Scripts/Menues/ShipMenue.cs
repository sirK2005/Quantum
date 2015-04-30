using UnityEngine;
using System.Collections;

public class ShipMenue : MonoBehaviour {

	public UIButton btnMove;
	public UIButton btnReconfig;
	public UIButton btnDeploy;
	public UIButton btnSpecial;

	public UISprite sprBtnMove;
	public UISprite sprBtnSpecial;
	public UISprite sprShip;

	public UILabel lblTitle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		btnMove.enabled = false;
		btnReconfig.enabled = false;
		btnDeploy.enabled = false;
		btnSpecial.enabled = false;

		if(GameLogic.Instance.ShipSelected != null)
		{
			// stop btns
			if(GameLogic.Instance.CurAction == Actions.Carrie)
			{
				btnSpecial.enabled = true;
			}

			if(GameLogic.Instance.CurAction == Actions.Move_Attack)
			{
				btnMove.enabled = true;
			}

			if(GameLogic.Instance.ActiveAlliance.ActionsLeft <= 0)
			{
				Ship selShip = GameLogic.Instance.ShipSelected;
				if(selShip.ShipType == 1 || selShip.ShipType == 6 || selShip.ShipType == 3 || selShip.ShipType == 4)
					if(!GameLogic.Instance.ShipSelected.UsedSpecialThisTurn)
						btnSpecial.enabled = true;
				
				return;
			}

			if(GameLogic.Instance.CurAction == Actions.None)
			{
				if(GameLogic.Instance.ShipSelected.TilePlacedOn == null)
				{
					btnDeploy.enabled = true;
				}
				else
				{
					if(!GameLogic.Instance.ShipSelected.MovedThisTurn)
						btnMove.enabled = true;
					if(!GameLogic.Instance.ShipSelected.UsedSpecialThisTurn)
						btnSpecial.enabled = true;
				}

				btnReconfig.enabled = true;
			}
		}
	}

	public void UpdateInfo()
	{
		if(GameLogic.Instance.ShipSelected == null)
		{
			lblTitle.text = "Ship - Actions";
			sprShip.spriteName = "";
			sprBtnMove.spriteName = "divert_small";
		}
		else
		{
			if(GameLogic.Instance.CurAction == Actions.Carrie)
				sprBtnSpecial.spriteName = "cancel_small";

			sprBtnMove.spriteName = "divert_small";
			sprShip.spriteName = GameLogic.shipSpriteNames[GameLogic.Instance.ShipSelected.ShipType - 1];
			lblTitle.text = GameLogic.shipNames[GameLogic.Instance.ShipSelected.ShipType - 1] + " - Actions";
		}
	}

	public void MoveButtonPressed()
	{
		if(GameLogic.Instance.CurAction == Actions.None)
		{
			sprBtnMove.spriteName = "cancel_small";
			GameLogic.Instance.ResolveAction(Actions.Move_Attack);
		}
		else
		{
			sprBtnMove.spriteName = "divert_small";
			GameLogic.Instance.StopShip();
		}

	}
	
	public void DeployButtonPressed()
	{
		GameLogic.Instance.ResolveAction(Actions.Deploy);
	}
	
	public void ReconfigButtonPressed()
	{
		GameLogic.Instance.ResolveAction(Actions.Reconfigure);
	}

	public void SpecialButtonPressed()
	{
		if(GameLogic.Instance.CurAction == Actions.Carrie)
		{
			GameLogic.Instance.ResolveStopCarrying();
		}
		else
			GameLogic.Instance.ResolveAction(Actions.Special);
	}

}
