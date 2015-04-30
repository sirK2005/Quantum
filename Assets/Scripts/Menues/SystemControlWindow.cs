using UnityEngine;
using System.Collections;

public class SystemControlWindow : MonoBehaviour {

	public UISprite[] sprAllianceBars;

	// Use this for initialization
	void Start () {
		for(int i = GameLogic.Instance.NbrOfPlayers; i < 4; i++)
		{
			sprAllianceBars[i].enabled = false;
		}
		for(int i = 0; i < GameLogic.Instance.NbrOfPlayers; i++)
		{
			sprAllianceBars[i].color = GameLogic.Instance.AllianceList[i].FlagColor;
		}
	}
	
	// Update is called once per frame
	public void UpdateInfo () {

		int yShift = 0;
		LevelMap currentMap = GameLogic.Instance.currentlyPlayedMap;
		int[] control = new int[4];
		int totalControlledPlantes = 0;
		// Calculate System Control for each player
		for(int i = 0; i < GameLogic.Instance.NbrOfPlayers; i++)
		{
			control[i] = currentMap.cubesPerPlayer - GameLogic.Instance.AllianceList[i].QuantumCubesLeft;
			totalControlledPlantes += control[i];
		}

		//adjust sprSize according to overall controll
		for(int i = 0; i < GameLogic.Instance.NbrOfPlayers; i++)
		{
			if(totalControlledPlantes > 0)
			{
				if(control[i] == 0)
				{
					sprAllianceBars[i].enabled = false;
					continue;
				}
				else
				{
					sprAllianceBars[i].enabled = true;
				}

				float controlInPercent = (float)control[i] / (float)totalControlledPlantes;
				sprAllianceBars[i].transform.localPosition = new Vector3(-160 + yShift, -50, 0);

				//if(i == 0 || i == GameLogic.Instance.NbrOfPlayers)
				//	sprAllianceBars[i].width = (int)(controlInPercent * 320 + 10);
				//else
					sprAllianceBars[i].width = (int)(controlInPercent * 320 + 20);

				yShift += sprAllianceBars[i].width - 20;
			}
		}

	}
}
