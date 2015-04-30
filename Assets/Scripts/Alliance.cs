using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alliance{

	private string mName;
	public string AllianceName { get{return mName;} }

	private Color mColor;
	public Color FlagColor { get{return mColor;} set{mColor = value;} }

	public Planet StartingPlanet {get; set;}

	private List<Ship> mShipList;
	public List<Ship> ShipList { get{return mShipList;} }

	private int mPlayOrderPos;
	public int PlayerOrderPos { get{return mPlayOrderPos;}}

	private int mActionsLeft;
	public int ActionsLeft { get{return mActionsLeft;} set{mActionsLeft = value;} }

	private int mQuantumCubesLeft;
	public int QuantumCubesLeft { get{return mQuantumCubesLeft;} set{mQuantumCubesLeft = value;}}

	private int mResearch;
	public int Research { get{return mResearch;} set{mResearch = value;} }

	private int mDominance;
	public int Dominance { get{return mDominance;} set{mDominance = value;} }

	private Scrapyard mScrapyard;
	public Scrapyard ScrapyardRef { get{return mScrapyard;} }

	private Mothership mAllianceMothership;
	public Mothership AllianceMothership { get{return mAllianceMothership;} }

	private List<CommandoCardBase> mCommandoCardList;
	public List<CommandoCardBase> CommandoCardList { get{return mCommandoCardList;} }

	private int mDomLostOnDefeat;
	public int DomLostOnDefeat { get{return mDomLostOnDefeat;} set{mDomLostOnDefeat = value;} }
	private int mDomWonOnWin;
	public int DomWonOnWin { get{return mDomWonOnWin;} set{mDomWonOnWin = value;} }

	public Ship GetNewShip()
	{
		GameObject newShipObj;
		Ship newShip;

		newShipObj = GameLogic.Instantiate(GameLogic.Instance.prefabShip) as GameObject;
		newShip = newShipObj.GetComponent<Ship>();
		newShip.SetToType(Random.Range(0, 6) + 1);
		newShip.AllianceBelongingTo = this;
		mShipList.Add(newShip);

		newShip.ReturnToScrapyard();

		return newShip;
	}

	public void ResetCommandoCards()
	{
		for(int i = 0; i < mCommandoCardList.Count; i++)
		{
			mCommandoCardList[i].Reset();
		}
	}

	public void AddCommandoCard(CommandoCardBase newCard)
	{
		mCommandoCardList.Add(newCard);
		if(mCommandoCardList.Count > 3)
			mCommandoCardList.RemoveAt(3);
	}

	public void TurnStartReset()
	{
		//--//
	}

	public int GetAmountShipsOnBoard()
	{
		int result = 0;

		for(int i = 0; i < mShipList.Count; i++)
		{
			if(mShipList[i].TilePlacedOn != null)
				result++;
		}

		Debug.Log(AllianceName + ": " + result);

		return result;
	}

	public int GetShipValueTotal()
	{
		int result = 0;
		
		for(int i = 0; i < mShipList.Count; i++)
		{
			result += mShipList[i].ShipType;
		}
		
		Debug.Log(AllianceName + ": " + result);
		
		return result;
	}

	public void Setup(int playOrder, Color col, string name, int cubesToPlay)
	{
		mPlayOrderPos = playOrder;
		mDominance = mResearch = 1;

		mQuantumCubesLeft = cubesToPlay;

		mColor = col;

		mName = name;

		mCommandoCardList = new List<CommandoCardBase>();

		mShipList = new List<Ship>();
		
		/*for(int i = 0; i < 3; i++)
		{
			newShipObj = Instantiate(GameLogic.Instance.prefabShip) as GameObject;
			newShip = newShipObj.GetComponent<Ship>();
			newShip.SetToType(Random.Range(0, 6) + 1);
			newShip.MoveToTile(mStartingPlanet.tileList[i]);
			newShip.AllianceBelongingTo = this;
			mShipList.Add(newShip);
		}*/
	}

	public void PlaceMotherShipAndScrapyard()
	{
		GameObject scrapObj = GameLogic.Instantiate(GameLogic.Instance.prefabScrapyard) as GameObject;
		mScrapyard = scrapObj.GetComponent<Scrapyard>();

		GameObject objMothership = GameLogic.Instantiate(GameLogic.Instance.prefabMothership) as GameObject;
		mAllianceMothership = objMothership.GetComponent<Mothership>();
		mAllianceMothership.AllianceBelongingTo = this;

		mScrapyard.Setup(this);

		if(StartingPlanet.WorldPos.x < 0)
		{
			//prefer left scrapyard
			if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(-1, 0)) == null)
			{
				mScrapyard.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x - 1) * 6, StartingPlanet.WorldPos.y * 6, 0);
				mAllianceMothership.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x - 2) * 6, StartingPlanet.WorldPos.y * 6, 0);
				return;
			}
		}
		else if(StartingPlanet.WorldPos.x > 0)
		{
			//prefer right scrapyard
			if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(+1, 0)) == null) // try to place right
			{
				mScrapyard.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x + 1) * 6, StartingPlanet.WorldPos.y * 6, 0);
				mAllianceMothership.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x + 2) * 6, StartingPlanet.WorldPos.y * 6, 0);
				return;
			}
		}

		// try to place left
		if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(-1, 0)) == null)
		{
			mScrapyard.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x - 1) * 6, StartingPlanet.WorldPos.y * 6, 0);
			mAllianceMothership.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x - 2) * 6, StartingPlanet.WorldPos.y * 6, 0);
		}
		else if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(+1, 0)) == null) // try to place right
		{
			mScrapyard.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x + 1) * 6, StartingPlanet.WorldPos.y * 6, 0);
			mAllianceMothership.transform.localPosition = new Vector3((StartingPlanet.WorldPos.x + 2) * 6, StartingPlanet.WorldPos.y * 6, 0);
		}
		else if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(0, -1)) == null) // try to place top
		{
			mScrapyard.transform.localPosition = new Vector3(StartingPlanet.WorldPos.x * 6, (StartingPlanet.WorldPos.y - 1) * 6, 0);
			mAllianceMothership.transform.localPosition = new Vector3(StartingPlanet.WorldPos.x * 6, (StartingPlanet.WorldPos.y - 2) * 6, 0);
		}
		else if(GameLogic.Instance.GetPlanetOnPos(StartingPlanet.WorldPos + new Vector2(0, +1)) == null) // try to place bottom
		{
			mScrapyard.transform.localPosition = new Vector3(StartingPlanet.WorldPos.x * 6, (StartingPlanet.WorldPos.y + 1) * 6, 0);
			mAllianceMothership.transform.localPosition = new Vector3(StartingPlanet.WorldPos.x * 6, (StartingPlanet.WorldPos.y + 2) * 6, 0);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
