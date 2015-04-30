using UnityEngine;
using System.Collections;

public class SmallAllianceInfoMenue : MonoBehaviour {

	public UILabel lblResearch;
	public UILabel lblDominance;
	public UILabel lblCubesLeft;

	public UISprite sprLeader;

	public Alliance AllianceToRepresent { get; set; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void UpdateInfo()
	{
		lblResearch.text = AllianceToRepresent.Research.ToString();
		lblDominance.text = AllianceToRepresent.Dominance.ToString();
		lblCubesLeft.text = AllianceToRepresent.QuantumCubesLeft.ToString();

		sprLeader.spriteName = GameLogic.allianceSpriteNames[AllianceToRepresent.PlayerOrderPos];
	}
}
