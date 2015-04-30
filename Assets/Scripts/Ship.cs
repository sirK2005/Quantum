using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	private bool mSelected;
	public bool selected { get{return mSelected;} set{SetSelection(value);} }

	private SpriteRenderer mSelectionSpriteRenderer;
	public Sprite selectionSprite;

	private PlanetTile mTilePlacedOn;
	public PlanetTile TilePlacedOn { get{return mTilePlacedOn;} set{mTilePlacedOn = value;} }

	private Planet mPlanetPlacedOn;
	public Planet PlanetPlacedOn { get{return mPlanetPlacedOn;} set{mPlanetPlacedOn = value;} }

	private Alliance mAllianceBelongingTo;
	public Alliance AllianceBelongingTo { get{return mAllianceBelongingTo;} set{SetAlliance(value);} }

	private static Vector3[] DICE_SIDES_ROTATION = {new Vector3(90, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 90, 0), new Vector3(0, 270, 0), new Vector3(0, 180, 0), new Vector3(270, 0, 0)};

	private int mType;
	public int ShipType {get{return mType;}}

	private int mMovePoints;
	private int mMPLeft;
	public int MPLeft { get{return mMPLeft;} set{mMPLeft = value;} }

	private int mPower;
	public int Power { get{return mPower;} }

	public GameObject dice;
	public GameObject[] diceSides;

	private bool mIsInScrapyard;

	private Color mColor;
	public Color Color { get{return mColor;} set{SetDiceColor(value);}}

	private bool mMovedThisTurn;
	public bool MovedThisTurn 
	{ 
		get
		{
			return mMovedThisTurn;
		} 
		set
		{
			mMovedThisTurn = value;
			if(!value)
				spriteShipCanMove.color = colAbilityActive;
			else
				spriteShipCanMove.color = colAbilityInactive;
		}
	}

	private bool mUsedSpecialThisTurn;
	public bool UsedSpecialThisTurn
	{ 
		get
		{
			return mUsedSpecialThisTurn;
		} 
		set
		{
			mUsedSpecialThisTurn = value;
			if(!value)
				SpriteShipCanSpecial.color = colAbilityActive;
			else
				SpriteShipCanSpecial.color = colAbilityInactive;
		}
	}
	
	private bool mReorganizedThisTurn;
	public bool ReorganizedThisTurn { get{return mReorganizedThisTurn;} set{mReorganizedThisTurn = value;}}

	public ParticleSystem swapEffect;

	public Color colAbilityActive;
	public Color colAbilityInactive;

	public SpriteRenderer spriteShipCanMove;
	public SpriteRenderer SpriteShipCanSpecial;

	// Use this for initialization
	void Start () {
		// Init
		mSelectionSpriteRenderer = gameObject.transform.GetComponentInChildren<SpriteRenderer>();
		selected = false;
		//mMovedThisTurn = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetSelection(bool sel)
	{
		mSelected = sel;
		
		// update rendered sprite
		/*if(mSelected)
			mSelectionSpriteRenderer.sprite = selectionSprite;
		else
			mSelectionSpriteRenderer.sprite = null;*/
	}

	private void SetDiceColor(Color col)
	{
		mColor = col;
		for(int i = 0; i < 6; i++)
		{
			diceSides[i].renderer.material.color = col;
		}
	}

	private void SetAlliance(Alliance alliance)
	{
		mAllianceBelongingTo = alliance;
		SetDiceColor(alliance.FlagColor);
	}

	public void Reset()
	{
		mMPLeft = mMovePoints;
		MovedThisTurn = false;
		UsedSpecialThisTurn = false;
		mReorganizedThisTurn = false;
	}

	public void ReturnToScrapyard()
	{
		if(mTilePlacedOn != null)
			mTilePlacedOn.occupyingShip = null;

		mTilePlacedOn = null;

		Reconfigure();

		AllianceBelongingTo.ScrapyardRef.AddShip(this);
	}

	public void SetToType(int side)
	{
		dice.transform.localEulerAngles = DICE_SIDES_ROTATION[side - 1];
		mType = side;
		mMovePoints = side;
		mPower = side;
	}

	public void Reconfigure()
	{
		int newType = mType;
		// Get Type Different from original type
		while(newType == mType)
		{
			newType = Random.Range(0,6) + 1;
		}

		// Set new Type
		SetToType(newType);
	}

	/// <summary>
	/// Deploys ship to given tile, like move, but not use up an MP
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void DeployToTile(PlanetTile tile)
	{
		// delete reference on old tile
		if(mTilePlacedOn != null)
			mTilePlacedOn.occupyingShip = null;

		// set reference and pos on new tile
		mTilePlacedOn = tile;
		mPlanetPlacedOn = tile.planetBelongingTo;
		mTilePlacedOn.occupyingShip = this;
		transform.localPosition = new Vector3(tile.transform.localPosition.x + mPlanetPlacedOn.WorldPos.x * 6, tile.transform.localPosition.y + mPlanetPlacedOn.WorldPos.y * 6, 0);
	
		AllianceBelongingTo.ScrapyardRef.RemoveShip(this);
	}

	/// <summary>
	/// Moves ship to given tile
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void MoveToTile(PlanetTile tile)
	{
		// delete reference on old tile
		if(mTilePlacedOn != null)
			mTilePlacedOn.occupyingShip = null;

		// set reference and pos on new tile
		mTilePlacedOn = tile;
		mPlanetPlacedOn = tile.planetBelongingTo;
		mTilePlacedOn.occupyingShip = this;
		transform.localPosition = new Vector3(tile.transform.localPosition.x + mPlanetPlacedOn.WorldPos.x * 6, tile.transform.localPosition.y + mPlanetPlacedOn.WorldPos.y * 6, 0);

		// reduce Movement Points left by 1
		if(mMPLeft > 0)
			mMPLeft -= 1;
	}
}
