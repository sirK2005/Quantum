using UnityEngine;
using System.Collections;

public class LowerMenue : MonoBehaviour {

	public UILabel lblActionsLeft;
	public UILabel lblResearch;
	public UILabel lblFame;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lblActionsLeft.text = GameLogic.Instance.ActiveAlliance.ActionsLeft.ToString();
		lblResearch.text = GameLogic.Instance.ActiveAlliance.Research.ToString();
		lblFame.text = GameLogic.Instance.ActiveAlliance.Dominance.ToString();
	}
}
