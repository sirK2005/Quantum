using UnityEngine;
using System.Collections;

public class VictoryMenue : ScreenBase {

	public UISprite sprLeader;

	public UILabel lblAllianceName;

	// Use this for initialization
	void Start () {
		Hide ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Show ()
	{
		base.Show();

		sprLeader.spriteName = GameLogic.allianceSpriteNames[GameLogic.Instance.ActiveAlliance.PlayerOrderPos];
		lblAllianceName.text = GameLogic.allianceNames[GameLogic.Instance.ActiveAlliance.PlayerOrderPos];
	}
}
