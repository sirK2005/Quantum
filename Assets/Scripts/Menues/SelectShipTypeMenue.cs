using UnityEngine;
using System.Collections;

public class SelectShipTypeMenue : ScreenBase {

	public UISprite sprLowerDice;
	public UISprite sprLowerShip;
	public UISprite sprHigherDice;
	public UISprite sprHigherShip;

	public UISprite sprShipDice;
	public UISprite sprShipPic;
	
	// Use this for initialization
	void Start () {
		Hide ();
	}

	public override void Show()
	{
		if(!mIsShown)
		{
			base.Show();

			int lowType = GameLogic.Instance.ShipSelected.ShipType - 2;
			if(lowType < 0)
				lowType = 5;
			int highType = GameLogic.Instance.ShipSelected.ShipType;
			if(highType > 5)
				highType = 1;

			sprShipDice.spriteName = GameLogic.diceSpriteNames[GameLogic.Instance.ShipSelected.ShipType - 1];
			sprShipDice.color = GameLogic.Instance.ActiveAlliance.FlagColor;
			sprShipPic.spriteName = GameLogic.shipSpriteNames[GameLogic.Instance.ShipSelected.ShipType - 1];

			sprLowerDice.spriteName = GameLogic.diceSpriteNames[lowType];
			sprLowerShip.spriteName = GameLogic.shipSpriteNames[lowType];
			sprHigherDice.spriteName = GameLogic.diceSpriteNames[highType];
			sprHigherShip.spriteName = GameLogic.shipSpriteNames[highType];
		}
	}

	public void LowerTypePressed()
	{
		GameLogic.Instance.ResolveTypePicked(-1);
	}

	public void HigherTypePressed()
	{
		GameLogic.Instance.ResolveTypePicked(+1);
	}
}
