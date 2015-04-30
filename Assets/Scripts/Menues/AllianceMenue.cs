using UnityEngine;
using System.Collections;

public class AllianceMenue : MonoBehaviour {
	
	public UIButton btnResearch;
	public UIButton btnConstruct;

	public UIButton btnEndTurn;

	public UILabel lblResearch;
	public UILabel lblDominance;
	public UILabel lblAllianceName;
	public UILabel lblActionsLeft;
	public UILabel lblCubesLeft;

	public UIButton[] btnCommandoCards;
	public UILabel[] lblCommandoCardTitles;

	public UISprite sprLeader;
	public UISprite sprConstruct;

	public SmallAllianceInfoMenue[] enemyInfoWnds;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update button.enable depending on selected items and action states
	void Update () {
		btnResearch.enabled = false;
		btnConstruct.enabled = false;
		btnEndTurn.enabled = false;

		if(GameLogic.Instance.CurPhaseType == PhaseType.StartupPhase)
			return;

		if(GameLogic.Instance.CurAction == Actions.Construct || GameLogic.Instance.CurAction == Actions.Construct_Domninance)
		{
			btnConstruct.enabled = true;
			return;
		}

		if(GameLogic.Instance.ActiveAlliance.ActionsLeft == 0)
		{
			btnEndTurn.enabled = true;
			return;
		}

		if(GameLogic.Instance.CurAction != Actions.None)
			return;

		btnEndTurn.enabled = true;
		btnResearch.enabled = true;
		btnConstruct.enabled = true;
	}

	public void UpdateInfo()
	{
		Alliance tempAll = GameLogic.Instance.ActiveAlliance;

		//lblResearch.text = tempAll.Research.ToString();
		//lblDominance.text = tempAll.Dominance.ToString();
		//lblCubesLeft.text = tempAll.QuantumCubesLeft.ToString();
		//lblActionsLeft.text = tempAll.ActionsLeft.ToString();
		//lblAllianceName.text = tempAll.AllianceName;
		sprLeader.spriteName = GameLogic.allianceSpriteNames[tempAll.PlayerOrderPos];

		// update other alliances windows
		/*int allianceCnter = GameLogic.Instance.ActiveAlliance.PlayerOrderPos + 1;
		for(int a = 0; a < 3; a++)
		{
			if(allianceCnter >= GameLogic.Instance.NbrOfPlayers)
				allianceCnter = 0;
			//if(GameLogic.Instance.AllianceList[allianceCnter] != GameLogic.Instance.ActiveAlliance)
			//{
			enemyInfoWnds[a].AllianceToRepresent = GameLogic.Instance.AllianceList[allianceCnter];
			enemyInfoWnds[a].UpdateInfo();
			//}
			allianceCnter++;
		}*/

		// update card list info
		int i;

		for(i = 0; i < tempAll.CommandoCardList.Count; i++)
		{
			btnCommandoCards[i].enabled = true;
			lblCommandoCardTitles[i].text = tempAll.CommandoCardList[i].Title;
		}

		for(; i < 3; i++)
		{
			btnCommandoCards[i].enabled = false;
			lblCommandoCardTitles[i].text = "<no card>";
		}

		if(GameLogic.Instance.CurAction == Actions.None)
			sprConstruct.spriteName = "ice-cube_small";;
	}

	public void ResearchButtonPressed()
	{
		GameLogic.Instance.ResolveAction(Actions.Research);
	}

	public void ConstructButtonPressed()
	{
		if(GameLogic.Instance.CurAction == Actions.None)
		{
			GameLogic.Instance.ResolveAction(Actions.Construct);
			if(GameLogic.Instance.CurAction == Actions.Construct)
				sprConstruct.spriteName = "cancel_small";
		}
		else
		{
			GameLogic.Instance.StopConstructing();
			sprConstruct.spriteName = "ice-cube_small";
		}
	}

	public void DeactivateUnusedAllianceInfoWnds(int playerNbr)
	{
		for(int i = playerNbr - 1; i < 3; i++)
		{
			enemyInfoWnds[i].Deactivate();
		}
	}

	public void EndTurnButtonPressed()
	{
		GameLogic.Instance.EndTurn();
	}

	public void Card1Clicked()
	{
		GameLogic.Instance.InspectCard(0);
	}

	public void Card2Clicked()
	{
		GameLogic.Instance.InspectCard(1);
	}

	public void Card3Clicked()
	{
		GameLogic.Instance.InspectCard(2);
	}

}
