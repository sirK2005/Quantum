using UnityEngine;
using System.Collections;

public class PlanetTile : MonoBehaviour {

	private bool mSelected;
	public bool Selected { get {return mSelected;} set {SetSelection(value);} }
	private bool defaultSelectState = false;

	public Sprite selectionSprite;
	private SpriteRenderer mSelectionSpriteRenderer;


	private Ship mOccupyingShip = null;
	public Ship occupyingShip { get{return mOccupyingShip;} set{mOccupyingShip = value;} }

	public PlanetTile[] neighbors;
	public Planet planetBelongingTo;

	private Color mNormColor = new Color(1, 0.8f, 0, 0.6f);
	private Color mAttColor = new Color(1, 0.1f, 0, 0.6f);

	void Awake()
	{
		mSelectionSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	// Use this for initialization
	void Start () {

		// Init

		Selected = defaultSelectState;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Deselects all neighboring tiles.
	/// </summary>
	public void DeselectNeighboringTiles()
	{
		for(int i = 0; i < 8; i++)
		{
			if(neighbors[i] != null)
			{
				neighbors[i].Selected = false;
				neighbors[i].SetSelectColor(mNormColor);
			}
		}
	}

	/// <summary>
	/// Selects neighboring tiles according to moveinfo given
	/// </summary>
	/// <param name="info">Info.</param>
	public void SelectNeighboringTiles(SelectInfo info)
	{
		// do nothing if we are supposed to select tiles, and occupying ship has no MP left
		if(mOccupyingShip != null)
		{
			if(mOccupyingShip.MPLeft <= 0 && !info.ignoreMP)
				return;
		}

		if(info.allowDiagonal) // check all neighboring tiles
		{
			for(int i = 0; i < 8; i++)
			{
				if(neighbors[i] != null)
				{
					if(neighbors[i].occupyingShip != null) // if there is a ship on tile
					{
						if(info.selectEnemyShips) // we are supposed to select enemy ships
						{
							if(mOccupyingShip.AllianceBelongingTo != neighbors[i].occupyingShip.AllianceBelongingTo) // and there actualy is an enemy ship
							{
								neighbors[i].Selected = true;
								neighbors[i].SetSelectColor(mAttColor);
							}
						}

						if(info.selectOwnShips) // supposed to select own ships
						{
							if(mOccupyingShip.AllianceBelongingTo == neighbors[i].occupyingShip.AllianceBelongingTo)
							{
								neighbors[i].Selected = true;
								neighbors[i].SetSelectColor(mNormColor);
							}
						}
					}
					else // there is no ship on tile
					{
						if(info.selectTiles)
						{
							neighbors[i].Selected = true;
							neighbors[i].SetSelectColor(mNormColor);
						}
					}
				}
			}
		}
		else
		{
			int[] tilesToSelect = { 1, 3, 4, 6 };
			for(int i = 0; i < 4; i++)
			{
				if(neighbors[tilesToSelect[i]] != null)
				{
					if(neighbors[tilesToSelect[i]].occupyingShip != null) // if there is a ship on tile
					{
						if(info.selectEnemyShips) // we are supposed to select enemy ships
						{
							if(mOccupyingShip.AllianceBelongingTo != neighbors[tilesToSelect[i]].occupyingShip.AllianceBelongingTo) // and there actualy is an enemy ship
							{
								neighbors[tilesToSelect[i]].Selected = true;
								neighbors[tilesToSelect[i]].SetSelectColor(mAttColor);
							}
						}
						
						if(info.selectOwnShips) // supposed to select own ships
						{
							if(mOccupyingShip.AllianceBelongingTo == neighbors[tilesToSelect[i]].occupyingShip.AllianceBelongingTo)
							{
								neighbors[tilesToSelect[i]].Selected = true;
								neighbors[tilesToSelect[i]].SetSelectColor(mNormColor);
							}
						}
					}
					else // there is no ship on tile
					{
						if(info.selectTiles)
						{
							neighbors[tilesToSelect[i]].Selected = true;
							neighbors[tilesToSelect[i]].SetSelectColor(mNormColor);
						}
					}
				}
			}
		}
	}

	public bool CheckForShips(bool diagonal, bool enemy, bool own)
	{
		if(diagonal)
		{
			for(int i = 0; i < 8; i++)
			{
				Ship testShip = null;
				if(neighbors[i] != null)
					testShip = neighbors[i].occupyingShip;
				if(testShip != null) // there is a ship on that tile
				{
					if(neighbors[i].occupyingShip.AllianceBelongingTo == GameLogic.Instance.ShipSelected.AllianceBelongingTo) // has same alliance
					{
						if(own)
							return true;
					}
					else
					{
						if(enemy)
							return true;
					}
				}
			}
		}
		else
		{
			int[] tilesToCheck = { 1, 3, 4, 6 };
			for(int i = 0; i < 4; i++)
			{
				Ship testShip = null;
				if(neighbors[tilesToCheck[i]] != null)
					testShip = neighbors[tilesToCheck[i]].occupyingShip;
				if(testShip != null) // there is a ship on that tile
				{
					if(neighbors[tilesToCheck[i]].occupyingShip.AllianceBelongingTo == GameLogic.Instance.ShipSelected.AllianceBelongingTo) // has same alliance
					{
						if(own)
							return true;
					}
					else
					{
						if(enemy)
							return true;
					}
				}
			}
		}
		return false;
	}

	public void SetSelectColor(Color col)
	{
		mSelectionSpriteRenderer.color = col;
	}

	private void SetSelection(bool sel)
	{
		mSelected = sel;

		// update rendered sprite
		if(mSelected)
			mSelectionSpriteRenderer.sprite = selectionSprite;
		else
			mSelectionSpriteRenderer.sprite = null;
	}
}
