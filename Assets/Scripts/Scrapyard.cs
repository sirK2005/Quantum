using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scrapyard : MonoBehaviour {

	private Alliance mAllianceBelongingTo;
	public Alliance AllianceBelongingTo { get{return mAllianceBelongingTo;} }

	private Vector3 mShipPosOffset = new Vector3(+0.5f, -2, 0);

	private List<Ship> mShipList;

	// Use this for initialization
	void Start () {
		mShipList = new List<Ship>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Setup(Alliance all)
	{
		mAllianceBelongingTo = all;
	}

	public void AddShip(Ship ship)
	{
		int listPos = mShipList.Count;

		// set shipPos
		ship.transform.localPosition = transform.localPosition + mShipPosOffset + new Vector3(0, listPos * 2, 0);

		mShipList.Add(ship);
	}

	public void RemoveShip(Ship ship)
	{
		int remShip = 0;
		for(int i = 0; i < mShipList.Count; i++)
		{
			if(mShipList[i] == ship)
			   remShip = i;
		}

		mShipList.RemoveAt(remShip);

		RearrangeShips();
	}

	private void RearrangeShips()
	{
		for(int i = 0; i < mShipList.Count; i++)
		{
			mShipList[i].transform.localPosition = transform.localPosition + mShipPosOffset + new Vector3(0, i * 2, 0);
		}
	}
}
