using UnityEngine;
using System.Collections;

public class Mothership : MonoBehaviour {

	private Alliance mAllianceBelongingTo;
	public Alliance AllianceBelongingTo { get{return mAllianceBelongingTo;} set{SetAlliance(value);} }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetAlliance (Alliance all)
	{
		mAllianceBelongingTo = all;
	}
}
