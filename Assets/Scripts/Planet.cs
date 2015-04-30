using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

	private Vector2 mWorldPos;
	public Vector2 WorldPos { get{ return mWorldPos; } set{ SetToPos(value); } }

	public SpriteRenderer planetSprite;
	public SpriteRenderer nebulaSprite;
	public SpriteRenderer typeSprite;
	public SpriteRenderer selectSprite;
	public Sprite selSprite;

	private static int[] ORBITAL_TILES = {1, 3, 4, 6};

	private List<Vector3> cubeOffsets;

	private int mNbrOfPossibleCubes;

	public bool IsStartingPlanet {get; set;}

	private bool mSelected;
	public bool Selected 
	{
		get
		{
			return mSelected;
		}
		set
		{
			mSelected = value; 
			if(mSelected)
				selectSprite.sprite = selSprite;
			else
				selectSprite.sprite = null;
		}
	}

	public List<PlanetTile> tileList;

	private bool mIsStartingPlanet;

	private int mType;
	public int PlanetType { get{return mType;} }

	private int[] mAllianceInfluence = {0,0,0,0};
	public int[] AllianceInfluence { get{return mAllianceInfluence;} }

	private List<QuantumCube> mQuantumCubeList;
	public List<QuantumCube> QuantumCubeList { get{return mQuantumCubeList;} }

	public bool HasNoCubes { get{return mQuantumCubeList.Count == 0;} }


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateInfluences()
	{
		int[] tilePos = {1, 3, 4, 6};

		mAllianceInfluence[0] = 0;
		mAllianceInfluence[1] = 0;
		mAllianceInfluence[2] = 0;
		mAllianceInfluence[3] = 0;


		for(int i = 0; i < 4; i++)
		{
			Ship checkShip = tileList[tilePos[i]].occupyingShip;
			if(checkShip != null)
			{
				mAllianceInfluence[checkShip.AllianceBelongingTo.PlayerOrderPos] += checkShip.Power;
			}
		}
	}

	public void Setup(Vector2 pos, Sprite planet, Sprite nebula, int type, bool startingPlanet)
	{
		mType = type;
		typeSprite.sprite = GameLogic.Instance.planetTypeSpriteList[type - 7];

		Selected = false;

		mQuantumCubeList = new List<QuantumCube>();
		mNbrOfPossibleCubes = mType - 6;

		planetSprite.sprite = planet;
		nebulaSprite.sprite = nebula;
		SetToPos(pos);

		IsStartingPlanet = startingPlanet;

		// offsets for quantum cubes
		cubeOffsets = new List<Vector3>();
		if(mType == 7)
		{
			cubeOffsets.Add(new Vector3(0, 0, 0));
		}
		else if(mType == 8)
		{
			cubeOffsets.Add(new Vector3(-0.5f, 0, 0));
			cubeOffsets.Add(new Vector3(0.5f, 0, 0));
		}
		else if(mType == 9)
		{
			cubeOffsets.Add(new Vector3(-0.5f, -0.5f, 0));
			cubeOffsets.Add(new Vector3(0.5f, -0.5f, 0));
			cubeOffsets.Add(new Vector3(0, 0.25f, 0));
		}
		else if(mType == 10)
		{
			cubeOffsets.Add(new Vector3(-0.5f, -0.5f, 0));
			cubeOffsets.Add(new Vector3(0.5f,-0.5f, 0));
			cubeOffsets.Add(new Vector3(0.5f, 0.25f, 0));
			cubeOffsets.Add(new Vector3(0.5f, 0.25f, 0));
		}
	}

	public bool AllianceHasCube(Alliance all)
	{
		for(int i = 0; i < mQuantumCubeList.Count; i++)
		{
			if(mQuantumCubeList[i].AllianceBelongingTo == all)
				return true;
		}

		return false;
	}

	public int MarkTilesForDeploy(Alliance all, bool sel, bool ignoreShips)
	{
		int returnValue = 0;

		if(!AllianceHasCube(all))
			return 0;

		for(int i = 0; i < 4; i++)
		{
			if(tileList[ORBITAL_TILES[i]].occupyingShip == null || ignoreShips)
			{
				tileList[ORBITAL_TILES[i]].Selected = sel;
				returnValue++;
			}
		}

		return returnValue;
	}

	public int RelocateCube(Alliance all, int minType)
	{
		// type is too high
		if(mType > minType)
			return -1;

		// already buil a cube on this planet
		for(int a = 0; a < mQuantumCubeList.Count; a++)
		{
			if(mQuantumCubeList[a].AllianceBelongingTo == all)
				return -2;
		}

		// create cube
		GameObject cubeObj = Instantiate(GameLogic.Instance.prefabQuantumCube) as GameObject;
		QuantumCube cube = cubeObj.GetComponent<QuantumCube>();
		
		Vector3 newPos =  new Vector3(transform.localPosition.x + cubeOffsets[mQuantumCubeList.Count].x, transform.localPosition.y  + cubeOffsets[mQuantumCubeList.Count].y, 0);
		cube.Setup(all, newPos, this);
		
		QuantumCubeList.Add(cube);

		return 0;
	}

	/// <summary>
	/// Searches for a cube belingong to given alliance
	/// </summary>
	/// <returns>Found cube</returns>
	/// <param name="all">All.</param>
	public QuantumCube GetAllianceCube(Alliance all)
	{
		for(int a = 0; a < mQuantumCubeList.Count; a++)
		{
			if(mQuantumCubeList[a].AllianceBelongingTo == all)
				return mQuantumCubeList[a];
		}

		return null;
	}

	/// <summary>
	/// Removes given quantum cube from play and cube list
	/// </summary>
	/// <returns><c>true</c>, if quantum cube is in list and could be removed, <c>false</c> otherwise.</returns>
	/// <param name="cube">Cube.</param>
	public bool RemoveQuantumCube(QuantumCube cube)
	{
		for(int a = 0; a < mQuantumCubeList.Count; a++)
		{
			if(mQuantumCubeList[a] == cube)
			{
				QuantumCube tempCube = mQuantumCubeList[a];
				mQuantumCubeList.RemoveAt(a);
				Destroy(tempCube.gameObject);
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Constructs, if possible, a quantum cube.
	/// </summary>
	/// <returns>0 - if cube could be built, -1 if influence isn't matching, -2 if there is no more room left on planet, -3 if alliance has already a cube on planet </returns>
	/// <param name="all">Alliance that wants to build a cube</param>
	/// <param name="ignoreInfl">If set to <c>true</c> ignores influence check.</param>
	public int ConstructCube(Alliance all, bool ignoreInfl)
	{
		// calc influece of alliance
		int allInfl = 0;

		for(int i = 0; i < 4; i++)
		{
			if(tileList[ORBITAL_TILES[i]].occupyingShip != null)
			{
				Ship occShip = tileList[ORBITAL_TILES[i]].occupyingShip;
				if(occShip.AllianceBelongingTo == all)
					allInfl += occShip.Power;
			}
		}

		// not correct influence
		if(allInfl != mType && ignoreInfl == false)
			return -1;

		// max amount of cubes built
		if(mQuantumCubeList.Count >= mNbrOfPossibleCubes)
			return -2;

		// already buil a cube on this planet
		for(int a = 0; a < mQuantumCubeList.Count; a++)
		{
			if(mQuantumCubeList[a].AllianceBelongingTo == all)
				return -3;
		}

		// create cube
		GameObject cubeObj = Instantiate(GameLogic.Instance.prefabQuantumCube) as GameObject;
		QuantumCube cube = cubeObj.GetComponent<QuantumCube>();

		Vector3 newPos =  new Vector3(transform.localPosition.x + cubeOffsets[mQuantumCubeList.Count].x, transform.localPosition.y  + cubeOffsets[mQuantumCubeList.Count].y, 0);
		cube.Setup(all, newPos, this);

		all.QuantumCubesLeft--;

		QuantumCubeList.Add(cube);

		return 0;
	}

	public void SetToPos(Vector2 newPos)
	{
		transform.localPosition = new Vector3(newPos.x * 6, newPos.y * 6, 0);
		mWorldPos = newPos;

		// set tile references for neighboring planets
		// neighbor north:
		Planet planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x, newPos.y + 1));
		if(planet != null)
		{
			//TL
			tileList[0].neighbors[1] = planet.tileList[5];
			planet.tileList[5].neighbors[6] = tileList[0];
			tileList[0].neighbors[2] = planet.tileList[6];
			planet.tileList[6].neighbors[5] = tileList[0];

			//T
			tileList[1].neighbors[0] = planet.tileList[5];
			planet.tileList[5].neighbors[7] = tileList[1];
			tileList[1].neighbors[1] = planet.tileList[6];
			planet.tileList[6].neighbors[6] = tileList[1];
			tileList[1].neighbors[2] = planet.tileList[7];
			planet.tileList[7].neighbors[5] = tileList[1];

			//TR
			tileList[2].neighbors[0] = planet.tileList[6];
			planet.tileList[6].neighbors[7] = tileList[2];
			tileList[2].neighbors[1] = planet.tileList[7];
			planet.tileList[7].neighbors[6] = tileList[2];
		}
		// neighbor east:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x + 1, newPos.y));
		if(planet != null)
		{
			//TR
			tileList[2].neighbors[4] = planet.tileList[0];
			planet.tileList[0].neighbors[3] = tileList[2];
			tileList[2].neighbors[7] = planet.tileList[3];
			planet.tileList[3].neighbors[0] = tileList[2];
			
			//R
			tileList[4].neighbors[2] = planet.tileList[0];
			planet.tileList[0].neighbors[5] = tileList[4];
			tileList[4].neighbors[4] = planet.tileList[3];
			planet.tileList[3].neighbors[3] = tileList[4];
			tileList[4].neighbors[7] = planet.tileList[5];
			planet.tileList[5].neighbors[0] = tileList[4];
			
			//BR
			tileList[7].neighbors[2] = planet.tileList[3];
			planet.tileList[3].neighbors[5] = tileList[7];
			tileList[7].neighbors[4] = planet.tileList[5];
			planet.tileList[5].neighbors[3] = tileList[7];
		}
		// neighbor south:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x, newPos.y - 1));
		if(planet != null)
		{
			//BL
			tileList[5].neighbors[6] = planet.tileList[0];
			planet.tileList[0].neighbors[1] = tileList[5];
			tileList[5].neighbors[7] = planet.tileList[1];
			planet.tileList[1].neighbors[0] = tileList[5];
			
			//B
			tileList[6].neighbors[5] = planet.tileList[0];
			planet.tileList[0].neighbors[2] = tileList[6];
			tileList[6].neighbors[6] = planet.tileList[1];
			planet.tileList[1].neighbors[1] = tileList[6];
			tileList[6].neighbors[7] = planet.tileList[2];
			planet.tileList[2].neighbors[0] = tileList[6];
			
			//BR
			tileList[7].neighbors[5] = planet.tileList[1];
			planet.tileList[1].neighbors[2] = tileList[7];
			tileList[7].neighbors[6] = planet.tileList[2];
			planet.tileList[2].neighbors[1] = tileList[7];
		}
		// neighbor west:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x - 1, newPos.y));
		if(planet != null)
		{
			//TL
			tileList[0].neighbors[3] = planet.tileList[2];
			planet.tileList[2].neighbors[4] = tileList[0];
			tileList[0].neighbors[5] = planet.tileList[4];
			planet.tileList[4].neighbors[2] = tileList[0];
			
			//L
			tileList[3].neighbors[0] = planet.tileList[2];
			planet.tileList[2].neighbors[7] = tileList[3];
			tileList[3].neighbors[3] = planet.tileList[4];
			planet.tileList[4].neighbors[4] = tileList[3];
			tileList[3].neighbors[5] = planet.tileList[7];
			planet.tileList[7].neighbors[2] = tileList[3];
			
			//BL
			tileList[5].neighbors[0] = planet.tileList[4];
			planet.tileList[4].neighbors[7] = tileList[5];
			tileList[5].neighbors[3] = planet.tileList[7];
			planet.tileList[7].neighbors[4] = tileList[5];
		}
		// neighbor northwest:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x - 1, newPos.y + 1));
		if(planet != null)
		{
			//TL
			tileList[0].neighbors[0] = planet.tileList[7];
			planet.tileList[7].neighbors[7] = tileList[0];
		}
		// neighbor northeast:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x + 1, newPos.y + 1));
		if(planet != null)
		{
			//TR
			tileList[2].neighbors[2] = planet.tileList[5];
			planet.tileList[5].neighbors[5] = tileList[2];
		}
		// neighbor southwest:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x - 1, newPos.y - 1));
		if(planet != null)
		{
			//BL
			tileList[5].neighbors[5] = planet.tileList[2];
			planet.tileList[2].neighbors[2] = tileList[5];
		}
		// neighbor southeast:
		planet = GameLogic.Instance.GetPlanetOnPos(new Vector2(newPos.x + 1, newPos.y - 1));
		if(planet != null)
		{
			//BR
			tileList[7].neighbors[7] = planet.tileList[0];
			planet.tileList[0].neighbors[0] = tileList[7];
		}
	}
}
