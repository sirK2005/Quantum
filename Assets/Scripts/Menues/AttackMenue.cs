using UnityEngine;
using System.Collections;

public class AttackMenue : ScreenBase {

	public UISprite sprAttRollDice;
	public UISprite sprAttShipDice;
	public UISprite sprDefRollDice;
	public UISprite sprDefShipDice;
	public UISprite sprAttackPic;
	public UISprite sprDefenderPic;

	public UILabel lblTitle;
	public UILabel lblOutcome;

	public UIButton btnOccupy;

	private bool mAttResult;

	void Start()
	{
		Hide();
	}


	public void CalculateAttack(Ship attacker, Ship defender)
	{
		int def = Random.Range(0, 6) + 1;
		int att = Random.Range(0, 6) + 1;
		
		int attTotal = att + attacker.Power;
		int defTotal = def + defender.Power;

		// title
		lblTitle.text = attacker.AllianceBelongingTo.AllianceName + " vs. " + defender.AllianceBelongingTo.AllianceName;

		// attacker sprite stuff
		sprAttRollDice.spriteName = GameLogic.diceSpriteNames[att - 1];
		sprAttShipDice.spriteName = GameLogic.diceSpriteNames[attacker.Power - 1];
		sprAttShipDice.color = attacker.AllianceBelongingTo.FlagColor;
		sprAttackPic.spriteName = GameLogic.shipSpriteNames[attacker.ShipType - 1];

		// defender sprite stuff
		sprDefRollDice.spriteName = GameLogic.diceSpriteNames[def - 1];
		sprDefShipDice.spriteName = GameLogic.diceSpriteNames[defender.Power - 1];
		sprDefShipDice.color = defender.AllianceBelongingTo.FlagColor;
		sprDefenderPic.spriteName = GameLogic.shipSpriteNames[defender.ShipType - 1];

		if(attTotal <= defTotal) // victory
		{
			lblOutcome.text = "Attack won!";
			mAttResult = true;
			btnOccupy.enabled = true;
		}
		else
		{
			lblOutcome.text = "Attack repelled!";
			mAttResult = false;
			btnOccupy.enabled = false;

		}
	}

	public void StayBtnPressed()
	{
		GameLogic.Instance.ResolveAttackShip(mAttResult, true);
	}

	public void OccupyBtnPressed()
	{
		GameLogic.Instance.ResolveAttackShip(mAttResult, false);
	}

}
