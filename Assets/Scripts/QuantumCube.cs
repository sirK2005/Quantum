using UnityEngine;
using System.Collections;

public class QuantumCube : MonoBehaviour {

	public GameObject[] diceSides;

	private Alliance mAllianceBelongingTo;
	public Alliance AllianceBelongingTo { get{return mAllianceBelongingTo;} }

	private Planet mPlanetPlacedOn;
	public Planet PlanetPlacedOn { get{return mPlanetPlacedOn;} }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Setup(Alliance all, Vector3 pos, Planet plan)
	{
		mAllianceBelongingTo = all;
		transform.localPosition = pos;
		SetColor(all.FlagColor);
		mPlanetPlacedOn = plan;
	}

	private void SetColor(Color col)
	{
		for(int i = 0; i < 6; i++)
		{
			diceSides[i].renderer.material.color = col;
		}
	}
}
