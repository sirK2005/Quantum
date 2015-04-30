using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PhaseType
{
	ActionPhase = 0,
	AdvanceCardsPhase = 1,
	StartupPhase = 2,
}

public struct Phase
{
	public PhaseType phaseType;
	public int actionsToSpend;
	public int playerNbr;

	public Phase(PhaseType pType, int actions, int player)
	{
		phaseType = pType;
		actionsToSpend = actions;
		playerNbr = player;
	}
}

public struct MapPlanet
{
	public bool isStartingPlanet;
	public int planetType;
	public Vector2 worldPos;

	public MapPlanet(bool startingPlanet, int type, Vector2 pos)
	{
		isStartingPlanet = startingPlanet;
		planetType = type;
		worldPos = pos;
	}
}

public struct LevelMap
{
	public List<MapPlanet> planets;
	public int cubesPerPlayer;
	public int nbrOfPlayers;
	public string mapName;
	public int minX;
	public int maxX;
	public int minY;
	public int maxY;

	public void AddPlanet(bool isStartingPlanet, int planetType, Vector2 worldPos)
	{
		if(planets == null)
			planets = new List<MapPlanet>();

		planets.Add(new MapPlanet(isStartingPlanet, planetType, worldPos));

		if(worldPos.x < minX)
			minX = (int)worldPos.x;
		if(worldPos.x > maxX)
			maxX = (int)worldPos.x;
		if(worldPos.y < minY)
			minY = (int)worldPos.y;
		if(worldPos.y > maxY)
			maxY = (int)worldPos.y;
	}

	public LevelMap(int players, int cubes, string name)
	{
		planets = new List<MapPlanet>();
		nbrOfPlayers = players;
		cubesPerPlayer = cubes;
		mapName = name;

		minX = 0;
		maxX = 0;
		minY = 0;
		maxY = 0;
	}
}

public enum Actions
{
	None,
	Reconfigure,
	Move_Attack,
	Deploy,
	Research,
	Construct,
	Construct_Domninance,
	Special,
	Free_Attack,
	Swap,
	Carrie,
	Carrie_Pick_Ship,
	Carrie_Drop_Ship,
	Type_Change,
	Cards_Relocate_Cube,
	Cards_Reorganization,
	GameStart_ConstructCube,
	GameStart_Deploy,
}

public struct SelectInfo
{
	public bool ignoreMP;
	public bool allowDiagonal;
	public bool selectTiles;
	public bool selectEnemyShips;
	public bool selectOwnShips;

	public SelectInfo(bool ignMP, bool diagonal, bool tiles, bool enemyShips, bool ownShips)
	{
		ignoreMP = ignMP;
		allowDiagonal = diagonal;
		selectTiles = tiles;
		selectEnemyShips = enemyShips;
		selectOwnShips = ownShips;
	}
}

public class GameLogic : MonoBehaviour {

	// names
	public static string[] shipSpriteNames = {"Ship_1_Starbase", "Ship_2_Carrier", "Ship_3_Destroyer", "Ship_4_Frigate", "Ship_5_Interceptor", "Ship_6_Scout"};
	public static string[] shipNames = {"StarBase", "Carrier", "Destroyer", "Frigate", "Intercpetor", "Scout"};
	public static string[] allianceSpriteNames = {"Leader_1", "Leader_2", "Leader_3", "Leader_4"};
	public static string[] diceSpriteNames = {"dice-six-faces-one_small", "dice-six-faces-two_small", "dice-six-faces-three_small", "dice-six-faces-four_small", "dice-six-faces-five_small", "dice-six-faces-six_small"};
	public static string[] allianceNames = {"Kepler Imperium", "Andromeda Confed.", "Orion Republic", "Vulpes Alliance"};

	// pause
	private bool mPaused;
	public bool Paused {get{return mPaused;} set{mPaused = value; blackOverlay.enabled = value; blackOverlay.collider.enabled = value;} }

	// phase defining/handling stuff
	private List<Phase> mPhaseList;
	public List<Phase> PhaseList { get{return mPhaseList;} }
	public PhaseType CurPhaseType { get{return mPhaseList[0].phaseType;} }
	public Phase CurPhase { get{return mPhaseList[0];} }
	private Actions mCurAction;
	public Actions CurAction { get{return mCurAction;} }
	private Ship mShipToDeploy;
	private int mShipsDeployed = 0;
	private int mNbrOfPlayers;
	public int NbrOfPlayers {get{return mNbrOfPlayers;}}
	private int mActivePlayer;

	// cameras
	public Camera gameCamera;
	public Camera uiCamera;

	// backgrounds
	public GameObject backgroundUpper;
	public GameObject backgroundLower;

	// move related
	private float mCamBoundsXL = -6;
	private float mCamBoundsXR = 6;
	private float mCamBoundsYT = 3;
	private float mCamBoundsYB = -9;
	private float mCamBoundsZL = -6;
	private float mCamBoundsZT = -15;
	private int mMovSpd = 10;
	private float mMovSpdFacUpBack = 1.25f;
	private float mMovSpdFacLowBack = 2f;
	private float mStartDist;
	private float mStartZoom;
	public float mDragFactor {get; set;}

	// selectable holders
	private Ship mShipSelected;
	public Ship ShipSelected { get{return mShipSelected;} }
	private Planet mPlanetSelected;
	public Planet PlanetSelected { get{return mPlanetSelected;} }

	// action related field definitions
	private Ship mCarriedShip;
	private Ship mAttackedShip;
	private SelectInfo mSelectInfo;
	private int mPlanetTypeCubeTakenFrom;
	private bool mCubeTaken;
	private Alliance mAllianceCubeTakenFrom;

	// cards related fields
	private List<GambitCardBase> mGambitCardList;
	private List<GambitCardBase> mCurGambitsAvailable;
	public List<GambitCardBase> CurGambitsAvailable {get {return mCurGambitsAvailable;}}
	private List<CommandoCardBase> mCommandoCardList;
	private List<CommandoCardBase> mCurCommandosAvailable;
	public List<CommandoCardBase> CurCommandosAvailable {get {return mCurCommandosAvailable;}}

	// world elements
	private List<LevelMap> mMaps;
	public LevelMap currentlyPlayedMap {get{return mMaps[mapToPlay];}}
	private List<Planet> mPlanetList;
	public List<Sprite> planetSpriteList;
	public List<Sprite> nebulaSpriteList;
	public List<Sprite> planetTypeSpriteList;
	public int mapToPlay;

	// alliances related stuff
	private List<Alliance> mAllianceList;
	public List<Alliance> AllianceList { get{return mAllianceList;} }
	public Color[] allianceColors;
	public Alliance ActiveAlliance { get{return AllianceList[mActivePlayer];} }
	private int mAllianceCardsToGain;

	// game object prefabs
	public GameObject prefabShip;
	public GameObject prefabPlanet;
	public GameObject prefabQuantumCube;
	public GameObject prefabScrapyard;
	public GameObject prefabMothership;

	// Singleton
	private static GameLogic mInstance;
	public static GameLogic Instance { get{return mInstance;} }

	// game logic related
	private int mShipsDestroyedThisTurn;
	public int ShipsDestroyedThisTurn { get{return mShipsDestroyedThisTurn;} }
	
	// menue references
	public CircleMenue circleMenue;
	public MessageWindow msgWindow;
	public ShipMenue shipMenue;
	public UISprite blackOverlay;
	public OptionsMenue optionsMenue;
	public AllianceMenue allianceMenue;
	public SelectShipTypeMenue selShipTypeMenue;
	public AttackMenue attMenue;
	public VictoryMenue vicMenue;
	public CardSelectMenue cardSelectMenue;
	public CardInspectMenue cardInspectMenue;
	public SystemControlWindow systemControlWnd;

#region unity_stuff
	void Awake()
	{
		mInstance = this;

		UICamera.fallThrough = this.gameObject;

		Init();
	}

	// Use this for initialization
	void Init() {

		// Get Ressources
		LoadCards();

		// Create World
		CreateWorld();

		// camera setup
		mCamBoundsXL = mMaps[mapToPlay].minX * 6;
		mCamBoundsXR = mMaps[mapToPlay].maxX * 6;
		mCamBoundsYB = mMaps[mapToPlay].minY * 6 - 6;
		mCamBoundsYT = mMaps[mapToPlay].maxY * 6 - 6;
		int distX = Mathf.Abs(mMaps[mapToPlay].minX) + mMaps[mapToPlay].maxX;
		int distY = Mathf.Abs(mMaps[mapToPlay].minY) + mMaps[mapToPlay].maxY;
		int distMax = (distX > distY) ? distX : distY;
		mCamBoundsZT = distMax * -5;

		// setup windows
		//allianceMenue.DeactivateUnusedAllianceInfoWnds(mNbrOfPlayers);

		// calc screen size for pause elements
		UIRoot root = NGUITools.FindInParents<UIRoot>(blackOverlay.gameObject);
		float ratio = (float)root.activeHeight / Screen.height;
		blackOverlay.height = root.activeHeight;
		blackOverlay.width = (int)(Screen.width * ratio);
		BoxCollider boxcol = blackOverlay.collider as BoxCollider;
		boxcol.size = new Vector3(blackOverlay.width, blackOverlay.height, 0);

		shipMenue.UpdateInfo();
		allianceMenue.UpdateInfo();

		Paused = true;

/*#if UNITY_ANDROID
		ResumeFromOptioneMenue(1);
#else
		ResumeFromOptioneMenue(1);
#endif*/
	}

	// Update is called once per frame
	void Update () {
		FetchInput();
	}

#endregion

	public Planet GetPlanetOnPos(Vector2 pos)
	{
		for(int i = 0; i < mPlanetList.Count; i++)
		{
			if(mPlanetList[i].WorldPos == pos)
				return mPlanetList[i];
		}

		return null;
	}

	private void UpdateCameraPos(Vector3 posUpdate)
	{
		float camPosX = gameCamera.transform.localPosition.x;
		float camPosY = gameCamera.transform.localPosition.y;
		float camPosZ = gameCamera.transform.localPosition.z;

		if(camPosX + posUpdate.x < mCamBoundsXL || camPosX + posUpdate.x > mCamBoundsXR)
			return;
		if(camPosY + posUpdate.y < mCamBoundsYB || camPosY + posUpdate.y > mCamBoundsYT)
			return;
		if(camPosZ + posUpdate.z > mCamBoundsZL || camPosZ + posUpdate.z < mCamBoundsZT)
			return;


		gameCamera.transform.localPosition = new Vector3(camPosX + posUpdate.x, camPosY + posUpdate.y, camPosZ + posUpdate.z);

		// backgrounds move parralel with camera
		backgroundUpper.transform.Translate(posUpdate.x / mMovSpdFacUpBack, posUpdate.y / mMovSpdFacUpBack, 0);
		backgroundLower.transform.Translate(posUpdate.x / mMovSpdFacLowBack, posUpdate.y / mMovSpdFacLowBack, 0);
	}

	private void SetCameraZoom(float newZoom)
	{

		float camPosX = gameCamera.transform.localPosition.x;
		float camPosY = gameCamera.transform.localPosition.y;

		if(newZoom > mCamBoundsZL || newZoom < mCamBoundsZT)
			return;

		gameCamera.transform.localPosition = new Vector3(camPosX, camPosY, newZoom);
	}
	
#region keyboard_input

	/// <summary>
	/// Fetches input and reacts accordingly
	/// </summary>
	private void FetchInput()
	{
		if(Input.GetKey(KeyCode.F12))
			Application.CaptureScreenshot("Screenshot_" + System.DateTime.Now.ToString("MM_dd_yyyy") + System.DateTime.Now.ToString("hh_mm_ss") +  ".png");

		if(mPaused)
			return;
		
		if(Input.GetKey(KeyCode.D)) // move camera right
			UpdateCameraPos(new Vector3(Time.deltaTime * mMovSpd, 0, 0));
		if(Input.GetKey(KeyCode.A)) // move camera left
			UpdateCameraPos(new Vector3(-Time.deltaTime * mMovSpd, 0, 0));
		if(Input.GetKey(KeyCode.W)) // move camera up
			UpdateCameraPos(new Vector3(0, Time.deltaTime * mMovSpd, 0));
		if(Input.GetKey(KeyCode.S)) // move camera down
			UpdateCameraPos(new Vector3(0, -Time.deltaTime * mMovSpd, 0));
		if(Input.GetKey(KeyCode.Q)) // zoom in
			UpdateCameraPos(new Vector3(0, 0, Time.deltaTime * mMovSpd));
		if(Input.GetKey(KeyCode.E)) // zoom down
			UpdateCameraPos(new Vector3(0, 0, -Time.deltaTime * mMovSpd));
		if(Input.GetKey(KeyCode.Escape)) // quit game
			ShowOptioneMenue();
	}

#endregion

#region handle_click_events

	/// <summary>
	/// Things to do if a tile was clicked.
	/// </summary>
	/// <param name="tile">Tile.</param>
	private void HandleTileClicked(PlanetTile tile)
	{
		if(CurPhaseType == PhaseType.StartupPhase)
		{
			if(tile.Selected)
				ResolveStartDeploy(tile);

			return;
		}

		// move ship if one is selected, and tile is adjacent
		if(mShipSelected != null)
		{
			if(tile.Selected)
			{
				if(mCurAction == Actions.Move_Attack || mCurAction == Actions.Free_Attack || mCurAction == Actions.Carrie)
				{
					// and there isn't already a ship on the tile
					if(tile.occupyingShip == null)
					{
						MoveShip(tile);
					}
					else // if there is a ship, attack it, if it does not belong to same alliance
					{
						if(tile.occupyingShip.AllianceBelongingTo != mShipSelected.AllianceBelongingTo)
						{
							Attack(tile.occupyingShip); // attack 
						}
					}
				}
				else if(mCurAction == Actions.Deploy)
				{
					DeployShip(tile);
				}
				else if(mCurAction == Actions.Carrie_Drop_Ship)
				{
					ResolveDropShip(tile);
				}
			}
			else
				if(mCurAction == Actions.None)
					DeselectShip();
		}
	}

	/// <summary>
	/// Deals with stuff that happens when the user clicks on a planet
	/// </summary>
	/// <param name="planet">Planet.</param>
	private void HandlePlanetClicked(Planet planet)
	{
		if(CurPhaseType == PhaseType.StartupPhase)
		{
			if(mCurAction == Actions.GameStart_ConstructCube)
			{
				ResolveStartingPlanetClicked(planet);
			}
		}
		else 
		{
			if(mCurAction == Actions.Construct_Domninance)
			{
				ResolveDominanceConstruct(planet);
			}
			
			if(mCurAction == Actions.Construct)
			{
				ResolveConstruct(planet);
			}
			
			if(mCurAction == Actions.Cards_Relocate_Cube)
			{
				if(mCubeTaken)
				{
					ResolveRelocateCube(planet.RelocateCube(mAllianceCubeTakenFrom, mPlanetTypeCubeTakenFrom));
				}
			}
		}

	}

	/// <summary>
	/// Things to do if a ship was clicked.
	/// </summary>
	/// <param name="ship">Ship.</param>
	private void HandleShipClicked(Ship ship)
	{
		if(CurPhaseType == PhaseType.StartupPhase)
			return;

		// do nothing if we resolve dominance
		if(mCurAction == Actions.Construct_Domninance)
			return;

		// if no ship is currently select, select clicked ship
		if(mShipSelected == null)
		{
			SelectShip(ship);
		}
		else
		{
			// if clicked ship is curSelected ship, than do nothing
			if(ship == mShipSelected)
				return;

			if(mCurAction == Actions.Move_Attack || mCurAction == Actions.Free_Attack)
			{
				// if clicked ship is in range of ship
				if(ship.TilePlacedOn != null)
				{
					PlanetTile tile = ship.TilePlacedOn;
					if(tile.Selected) // if it is of another alliance, attack ship
					{
						if(ship.AllianceBelongingTo != mShipSelected.AllianceBelongingTo)
						{
							Attack(ship); // attack
						}
					}
				}
			}
			else if(mCurAction == Actions.Carrie_Pick_Ship)
			{
				ResolveShipPickedForCarry(ship);
			}
			else if(mCurAction == Actions.Swap)
			{
				ResolveSwap(ship);
			}
			else
			{
				DeselectShip();
				SelectShip(ship);
			}
		}
	}

	/// <summary>
	/// Handles stuff when q cube has been clicked
	/// </summary>
	/// <param name="">.</param>
	public void HandleQuantumCubeClicked(QuantumCube cube)
	{
		if(CurPhaseType != PhaseType.AdvanceCardsPhase)
			return;
		if(mCurAction == Actions.Cards_Relocate_Cube)
		{
			if(!mCubeTaken)
			{
				Planet tempPlanet = cube.PlanetPlacedOn;
				mPlanetTypeCubeTakenFrom = tempPlanet.PlanetType;
				mCubeTaken = true;
				mAllianceCubeTakenFrom = cube.AllianceBelongingTo;
				tempPlanet.RemoveQuantumCube(cube);
			}
		}
			
	}

	/// <summary>
	/// Things to be done if nothing was clicked
	/// </summary>
	private void HandleNothingClicked()
	{
		// Deselect ship if its not supposed to move
		if(mShipSelected != null)
		{
			if(mCurAction != Actions.Move_Attack)
				DeselectShip();
		}
	}

	void OnClick() // handle mouse clicks that NGUI didnt catch
	{
		if(mPaused || UICamera.isDragging)
			return;

		HandleMouseBtnPresses((UICamera.currentTouchID == -1), (UICamera.currentTouchID == -2));
	}

	void OnPress(bool state)
	{
		if(!state)
		{
			mStartDist = 0;
			mStartZoom = 0;
			return;
		}

		if(Input.touchCount == 2)
		{
			mStartZoom = gameCamera.transform.localPosition.z;

			mStartDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
		}
	}


	void OnDrag (Vector2 delta)
	{
		if(mPaused)
			return;

		// calc pinch and resulting zoom
		if(Input.touchCount == 2)
		{
			// current distance between fingers
			float curDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

			// delta between current and starting distance
			float deltaDist = curDist - mStartDist;

			// Set Camera zoom
			SetCameraZoom(mStartZoom + deltaDist * mDragFactor);
		}
		else // calc drag
		{
			UpdateCameraPos(new Vector3(-delta.x * mDragFactor, -delta.y  * mDragFactor, 0));
		}
	}

	public void HandleMouseBtnPresses(bool lftMBtnPressed, bool rightMBtnPressed)
	{
		Ray rayMousePos = gameCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayHit;
		
		//Debug.Log("Sending Ray at " + rayMousePos);
		if ( Physics.Raycast(rayMousePos, out rayHit) )
		{
			//Debug.Log("Hit something");
			
			// get a tile
			MonoBehaviour[] components =  rayHit.transform.GetComponents<MonoBehaviour>();
			
			// test if a tile has been clicked
			PlanetTile tempTile = null;
			foreach(MonoBehaviour component in components)
			{
				tempTile = component as PlanetTile;
				if(tempTile != null)
					break;
			}
			// handle tile being clicked
			if(tempTile != null)
			{
				HandleTileClicked(tempTile);
			}
			
			// test if a ship has been clicked
			Ship tempShip = null;
			foreach(MonoBehaviour component in components)
			{
				tempShip = component as Ship;
				if(tempShip != null)
					break;
			}
			
			// handle ship being clicked
			if(tempShip != null)
			{
				HandleShipClicked(tempShip);
			}

			// test if a planet has been clicked
			Planet tempPlanet = null;
			foreach(MonoBehaviour component in components)
			{
				tempPlanet = component as Planet;
				if(tempPlanet != null)
					break;
			}
			
			// handle ship being clicked
			if(tempPlanet != null)
			{
				HandlePlanetClicked(tempPlanet);
			}

			// test if a QuantumCube has been clicked
			QuantumCube tempCube = null;
			foreach(MonoBehaviour component in components)
			{
				tempCube = component as QuantumCube;
				if(tempCube != null)
					break;
			}
			
			// handle ship being clicked
			if(tempCube != null)
			{
				HandleQuantumCubeClicked(tempCube);
			}
		}
		else
		{
			HandleNothingClicked();
		}
	}

#endregion

#region option_menue

	public void ResumeFromOptioneMenue(float newScale)
	{
		float scale = newScale * 0.3f;
		
		//uiCamera.transform.localScale = new Vector3(1 + scale, 1 + scale, 1);
		
		optionsMenue.Hide();
		Paused = false;
		
	}
	public void ShowOptioneMenue()
	{
		optionsMenue.Show();
		Paused = true;
	}

#endregion

#region ShipHandling

	/// <summary>
	/// Currently selected ship attacks given Ship.
	/// </summary>
	/// <param name="attackedShip">Attacked ship.</param>
	private void Attack(Ship attackedShip)
	{
		Paused = true;
		attMenue.Show();
		mAttackedShip = attackedShip;
		attMenue.CalculateAttack(mShipSelected, attackedShip);
	}

	public void ResolveAttackShip(bool attResult, bool stay)
	{
		PlanetTile tempTile = mAttackedShip.TilePlacedOn;
		attMenue.Hide();

		mShipSelected.TilePlacedOn.DeselectNeighboringTiles();
		StopShip();

		if(attResult)
		{
			// Attacker has won
			AllianceLooseDominance(mAttackedShip.AllianceBelongingTo, mAttackedShip.AllianceBelongingTo.DomLostOnDefeat);
			AllianceGainDominance(ActiveAlliance.DomWonOnWin);
			mAttackedShip.ReturnToScrapyard();
			mShipsDestroyedThisTurn++;
		}

		if(!stay)
		{
			MoveShip(tempTile);
		}

		Paused = false;
	}

	/// <summary>
	/// Move a ship to a tile an update tile selection
	/// </summary>
	/// <param name="tile">Tile.</param>
	private void MoveShip(PlanetTile tile)
	{
		if(mShipSelected.TilePlacedOn != null)
			mShipSelected.TilePlacedOn.DeselectNeighboringTiles();

		mShipSelected.MoveToTile(tile);
		mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);

		if(mShipSelected.MPLeft <= 0)
			StopShip();
	}

	public void StopShip()
	{
		if(mCurAction == Actions.Carrie)
			ResolveStopCarrying();
		else
		{
			if(mCurAction == Actions.Move_Attack)
				mShipSelected.MovedThisTurn = true;
			if(mCurAction == Actions.Move_Attack || mCurAction == Actions.Free_Attack)
				mCurAction = Actions.None;
			mShipSelected.MPLeft = 0;
			mShipSelected.TilePlacedOn.DeselectNeighboringTiles();
			shipMenue.UpdateInfo();
		}
	}

	/// <summary>
	/// set curSelect ship to given ship and deselect old ship, if
	/// onbe was selected
	/// </summary>
	/// <param name="ship">Ship.</param>
	public void SelectShip(Ship ship)
	{
		// only change selection if player clicked on ship that belongs to his alliance
		if(ship.AllianceBelongingTo != ActiveAlliance)
			return;

		if(mShipSelected != null)
			DeselectShip();

		// setup selection
		ship.selected = true;
		mShipSelected = ship;

		// update shipMenue
		shipMenue.UpdateInfo();

		// show shipMenue
		circleMenue.Show();

		// mark tiles ship can move to
		//PlanetTile shipTile = ship.TilePlacedOn;
		//shipTile.SetSelctionNeighboringTiles(true, false);
	}

	/// <summary>
	/// Deselect curSelectedShip
	/// </summary>
	public void DeselectShip()
	{
		if(mShipSelected == null)
			return;

		if(mShipSelected.TilePlacedOn != null)
			mShipSelected.TilePlacedOn.DeselectNeighboringTiles();

		mShipSelected.selected = false;
		mShipSelected = null;

		if(mCurAction == Actions.Move_Attack)
			mCurAction = Actions.None;

		shipMenue.UpdateInfo();

		circleMenue.Hide();
	}
#endregion

#region Phase progression

	public void ProgressPhase()
	{
		DeselectShip();

		ResolveCommandoOnTurnEndEffects();

		int oldPlayer = CurPhase.playerNbr;

		mPhaseList.RemoveAt(0);

		if(CurPhaseType == PhaseType.StartupPhase)
			return;

		// if we switched to a new player, add new phases for old
		// player in phase list
		if(oldPlayer != CurPhase.playerNbr)
		{
			mPhaseList.Add(new Phase(PhaseType.ActionPhase, 3, oldPlayer));
			mPhaseList.Add(new Phase(PhaseType.AdvanceCardsPhase, 0, oldPlayer));
		}

		if(CurPhaseType == PhaseType.ActionPhase)
		{
			// phase setup
			mActivePlayer = CurPhase.playerNbr;
			mAllianceCardsToGain = 0;

			// reset all ship of new player
			foreach(Ship ship in ActiveAlliance.ShipList)
			{
				ship.Reset();
			}

			ActiveAlliance.ActionsLeft = CurPhase.actionsToSpend;
			ActiveAlliance.DomLostOnDefeat = 1;
			ActiveAlliance.DomWonOnWin = 1;
			ActiveAlliance.ResetCommandoCards();

			mCurAction = Actions.None;
			mShipsDestroyedThisTurn = 0;

			msgWindow.SetMessage(ActiveAlliance.AllianceName + ": " + CurPhaseType.ToString());

			ResolveCommandoOnTurnStartEffects();

			allianceMenue.UpdateInfo();
		}
		else if(CurPhaseType == PhaseType.AdvanceCardsPhase)
		{
			ActiveAlliance.ActionsLeft = 0;
			CheckAllianceBreakthrough();
			if(mAllianceCardsToGain > 0)
			{
				Paused = true;
				cardSelectMenue.Show();
			}
			else
				ProgressPhase();
		}
	}

	private void ResolveCommandoOnTurnEndEffects()
	{
		for(int i = 0; i < ActiveAlliance.CommandoCardList.Count; i++)
		{
			if(ActiveAlliance.CommandoCardList[i].ActType == ActivationType.TurnEnd)
				ActiveAlliance.CommandoCardList[i].Action();
		}
	}

	private void ResolveCommandoOnTurnStartEffects()
	{
		for(int i = 0; i < ActiveAlliance.CommandoCardList.Count; i++)
		{
			if(ActiveAlliance.CommandoCardList[i].ActType == ActivationType.TurnStart)
				ActiveAlliance.CommandoCardList[i].Action();
		}
	}

	public void EndTurn()
	{
		ProgressPhase();
	}

	public void ResolveStartUp()
	{
		mCurAction = Actions.GameStart_ConstructCube;
		Paused = false;
		SelectAllFreeStartingPlanets();
		msgWindow.SetMessage("Select starting planet.");
	}

	public void StartGame()
	{
		Alliance allLowestShipValue = mAllianceList[0];

		for(int i = 0; i < mNbrOfPlayers; i++)
		{
			if(mAllianceList[i].GetShipValueTotal() < allLowestShipValue.GetShipValueTotal())
			   allLowestShipValue = mAllianceList[i];
		}
		int startingAll = allLowestShipValue.PlayerOrderPos;

		for(int i = 0; i < mNbrOfPlayers; i++)
		{
			mPhaseList.Add(new Phase(PhaseType.ActionPhase, 3, (startingAll + i) % mNbrOfPlayers));
			mPhaseList.Add(new Phase(PhaseType.AdvanceCardsPhase, 0, (startingAll + i) % mNbrOfPlayers));
		}

		ProgressPhase();
	}

	public void BeginShipStartupDeploy()
	{
		mCurAction = Actions.GameStart_Deploy;
		mShipToDeploy = ActiveAlliance.GetNewShip();
		msgWindow.SetMessage(ActiveAlliance.AllianceName + ": Deploy a " + shipNames[mShipToDeploy.ShipType - 1] + "(" + mShipToDeploy.ShipType + ")");
		for(int i = 0; i < mPlanetList.Count; i++)
			mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, true, false);
	}

	public void AddPhaseAtBeginning(Phase p)
	{
		List<Phase> tempList = new List<Phase>();
		tempList.Add(PhaseList[0]);
		tempList.Add(p);
		for(int i = 1; i < mPhaseList.Count; i++)
		{
			tempList.Add(PhaseList[i]);
		}

		mPhaseList = tempList;
	}

	private void CheckForVictory()
	{
		if(ActiveAlliance.QuantumCubesLeft <= 0)
		{
			Paused = true;
			vicMenue.Show();
		}
	}
#endregion

#region action handling

	// Cube construction //
	public void ResolveDominanceConstruct(Planet planet)
	{
		int res = planet.ConstructCube(ActiveAlliance, true);
		if(res == 0)
		{
			mCurAction = Actions.None;
			allianceMenue.UpdateInfo();
			mAllianceCardsToGain++;
			CheckForVictory();
			systemControlWnd.UpdateInfo();
		}
		else if(res == -2)
			msgWindow.SetMessage("Planet has no more space left.");
		else if(res == -3)
			msgWindow.SetMessage("You already have a Quantum Cube built on this planet.");
	}


	public void ResolveConstruct(Planet planet)
	{
		int res = planet.ConstructCube(ActiveAlliance, false);
		if(res == 0)
		{
			mCurAction = Actions.None;
			ActiveAlliance.ActionsLeft -= 2;
			allianceMenue.UpdateInfo();
			mAllianceCardsToGain++;
			CheckForVictory();
			systemControlWnd.UpdateInfo();
		}
		else if(res == -1)
			msgWindow.SetMessage("Your ship influece does not match, the planet type.");
		else if(res == -2)
			msgWindow.SetMessage("Planet has no more space left.");
		else if(res == -3)
			msgWindow.SetMessage("You already have a Quantum Cube built on this planet.");
	}

	public void StopConstructing()
	{
		mCurAction = Actions.None;
	}

	// Ship Deployment //

	private void DeployShip(PlanetTile tile)
	{
		for(int i = 0; i < mPlanetList.Count; i++)
			mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, false, true);
		mShipSelected.DeployToTile(tile);
		mCurAction = Actions.None;
	}

	private void ResolveStartDeploy(PlanetTile tile)
	{
		for(int i = 0; i < mPlanetList.Count; i++)
			mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, false, true);
		mShipToDeploy.DeployToTile(tile);
		mShipsDeployed++;

		if(mShipsDeployed < 3)
		{
			mShipToDeploy = ActiveAlliance.GetNewShip();
			msgWindow.SetMessage(ActiveAlliance.AllianceName + ": Deploy a " + shipNames[mShipToDeploy.ShipType - 1] + "(" + mShipToDeploy.ShipType + ")");
			for(int i = 0; i < mPlanetList.Count; i++)
				mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, true, false);
		}
		else
		{
			mShipsDeployed = 0;
			mActivePlayer++;
			
			if(mActivePlayer > mNbrOfPlayers - 1)
			{
				mActivePlayer = 0;
				// begin game
				StartGame();
			}
			else
			{
				// other faction place ships
				allianceMenue.UpdateInfo();
				mShipToDeploy = ActiveAlliance.GetNewShip();
				msgWindow.SetMessage(ActiveAlliance.AllianceName + ": Deploy a " + shipNames[mShipToDeploy.ShipType - 1] + "(" + mShipToDeploy.ShipType + ")");
				for(int i = 0; i < mPlanetList.Count; i++)
					mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, true, false);
			}
		}
	}

	// Swap //

	public void ResolveSwap(Ship ship)
	{
		if(ship.AllianceBelongingTo != ActiveAlliance)
		{
			msgWindow.SetMessage("You can only swap with ships belonging to your alliance.");
			return;
		}
		PlanetTile tempTile = mShipSelected.TilePlacedOn;
		mShipSelected.MoveToTile(ship.TilePlacedOn);
		mShipSelected.swapEffect.Play();
		ship.TilePlacedOn = null;
		ship.MoveToTile(tempTile);
		ship.swapEffect.Play();
		mCurAction = Actions.None;
	}

	// Type Changing //

	public void ResolveTypeCanceled()
	{
		mCurAction = Actions.None;
		Paused = false;
		selShipTypeMenue.Hide();
	}

	public void ResolveTypePicked(int typeChange)
	{
		Paused = false;
		mCurAction = Actions.None;
		selShipTypeMenue.Hide();
		mShipSelected.SetToType(mShipSelected.ShipType + typeChange);
	}

	// Carry //

	public void ResolveShipPickedForCarry(Ship ship)
	{
		mShipSelected.TilePlacedOn.DeselectNeighboringTiles();
		mCarriedShip = ship;
		mCarriedShip.TilePlacedOn.occupyingShip = null;
		mCarriedShip.TilePlacedOn = null;
		mCarriedShip.transform.Translate(-1000, 0, 0);
		mCurAction = Actions.Carrie;
		mSelectInfo = new SelectInfo(true, false, true, false, false);
		mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);
	}

	public void ResolveStopCarrying()
	{
		mCurAction = Actions.Carrie_Drop_Ship;
		mShipSelected.MPLeft = 0;
		mShipSelected.MovedThisTurn = true;
		mShipSelected.TilePlacedOn.DeselectNeighboringTiles();
		mSelectInfo = new SelectInfo(true, true, true, false, false);
		mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);
		msgWindow.SetMessage("Pick a place to drop your carried ship.");
	}

	public void ResolveDropShip(PlanetTile tile)
	{
		mCarriedShip.MoveToTile(tile);
		mCarriedShip = null;
		mShipSelected.TilePlacedOn.DeselectNeighboringTiles();
		mCurAction = Actions.None;
	}

	public bool StartDeployingSelectedShip()
	{
		// todo: calc if deploy is possible first
		int selectedTiles = 0;
		for(int i = 0; i < mPlanetList.Count; i++)
			selectedTiles += mPlanetList[i].MarkTilesForDeploy(ActiveAlliance, true, false);

		if(selectedTiles > 0)
		{
			mCurAction = Actions.Deploy;
			msgWindow.SetMessage("Deploy your ship on a free orbital tile.");
			return true;
		}
		else
		{
			msgWindow.SetMessage("You have no free orbital tile. Ship is left in scrapyard.");
			return false;
		}
	}

	// Basic Action Handling //
	
	public void ResolveAction(Actions act)
	{
		if(act == Actions.Move_Attack)
		{
			mCurAction = Actions.Move_Attack;
			mShipSelected.Reset();
			mSelectInfo = new SelectInfo(false, false, true, true, false);
			mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);

			ActiveAlliance.ActionsLeft--;
		}
		else if(act == Actions.Reconfigure)
		{
			mShipSelected.Reconfigure();
			msgWindow.SetMessage("Your ship has been reconfigured. It's now a " + shipNames[mShipSelected.ShipType - 1] + " (" + mShipSelected.ShipType + ")");
			shipMenue.UpdateInfo();
			ActiveAlliance.ActionsLeft--;
		}
		else if(act == Actions.Research)
		{
			AllianceGainResearch(1);
			ActiveAlliance.ActionsLeft--;
		}
		else if(act == Actions.Construct)
		{
			if(ActiveAlliance.ActionsLeft < 2)
			{
				msgWindow.SetMessage("You need 2 Actions to construct a Quantum-Cube.");
				return;
			}

			mCurAction = Actions.Construct;
		}
		else if(act == Actions.Deploy)
		{
			if(StartDeployingSelectedShip()) // there is a tile to deploy ship left
				ActiveAlliance.ActionsLeft--;
		}
		else if(act == Actions.Special)
		{
			// free reconfigure
			if(ShipSelected.ShipType == 6)
			{
				mShipSelected.Reconfigure();
				msgWindow.SetMessage("Your ship has been reconfigured. It's now a " + shipNames[mShipSelected.ShipType - 1] + " (" + mShipSelected.ShipType + ")");
				shipMenue.UpdateInfo();
			}
			else if(ShipSelected.ShipType == 5) // move diagonally
			{
				mShipSelected.Reset();
				mSelectInfo = new SelectInfo(false, true, true, true, false);
				mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);
				mCurAction = Actions.Move_Attack;

				ActiveAlliance.ActionsLeft--;
			}
			else if(ShipSelected.ShipType == 4) // change to 3/5
			{
				mCurAction = Actions.Type_Change;
				selShipTypeMenue.Show();
				Paused = true;
			}
			else if(ShipSelected.ShipType == 3) // swap
			{
				mCurAction = Actions.Swap;
				msgWindow.SetMessage("Select a ship to swap places with");
			}
			else if(ShipSelected.ShipType == 2) // Carry
			{
				if(mShipSelected.TilePlacedOn.CheckForShips(true, false, true)) // there is a ship to carry
				{
					mCurAction = Actions.Carrie_Pick_Ship;
					mSelectInfo = new SelectInfo(true, true, false, false, true);
					mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);
					msgWindow.SetMessage("Pick a ship to carry");
					ActiveAlliance.ActionsLeft--;
				}
				else
				{
					msgWindow.SetMessage("There is no ship to carry in any adjacent tiles.");
				}
			}
			else if(ShipSelected.ShipType == 1) // free attack
			{
				if(mShipSelected.TilePlacedOn.CheckForShips(false, true, false))
				{
					mSelectInfo = new SelectInfo(true, false, false, true, false);
					mShipSelected.TilePlacedOn.SelectNeighboringTiles(mSelectInfo);
					
					mCurAction = Actions.Free_Attack;
				}
				else
					msgWindow.SetMessage("No enemy ships to attack in the area.");
			}

			mShipSelected.UsedSpecialThisTurn = true;

		}

		allianceMenue.UpdateInfo();
	}

#endregion

#region cards
	public void LoadCards()
	{
		mGambitCardList = new List<GambitCardBase>();
		mCurGambitsAvailable = new List<GambitCardBase>();
		mCommandoCardList = new List<CommandoCardBase>();
		mCurCommandosAvailable = new List<CommandoCardBase>();

		mGambitCardList.Add(new GC_Aggression());
		mGambitCardList.Add(new GC_Momentum());
		mGambitCardList.Add(new GC_Expansion());
		mGambitCardList.Add(new GC_Relocate());

		mCommandoCardList.Add(new CC_Brilliant());
		mCommandoCardList.Add(new CC_Arrogant());
		mCommandoCardList.Add(new CC_Righteous());
		mCommandoCardList.Add(new CC_Plundering());
		mCommandoCardList.Add(new CC_Tyrannical());
		mCommandoCardList.Add(new CC_Cerebral());

		DrawCards ();
	}

	public void DrawCards()
	{
		// flll up to 3 gambit cards
		int cardsToDrawGambit = 3 - mCurGambitsAvailable.Count;
		for(int i = 0; i < cardsToDrawGambit; i++)
		{
			int randomedCard = Random.Range(0, mGambitCardList.Count);
			mCurGambitsAvailable.Add(mGambitCardList[randomedCard]);
		}

		// flll up to 3 commando cards
		int cardsToDrawCommando = 3 - mCurCommandosAvailable.Count;
		for(int i = 0; i < cardsToDrawCommando; i++)
		{
			int randomedCard = Random.Range(0, mCommandoCardList.Count);
			mCurCommandosAvailable.Add(mCommandoCardList[randomedCard]);
		}
	}

	public void ResolveCardsSelected(CardBase card, int cardID)
	{
		Paused = false;
		cardSelectMenue.Hide();

		if(card.TypeOfCard == CardType.Gambit)
		{
			mCurGambitsAvailable[cardID].Action();
			
			// delete selected card
			mCurGambitsAvailable.RemoveAt(cardID);
		}
		else if(card.TypeOfCard == CardType.Commando)
		{
			ActiveAlliance.AddCommandoCard(mCurCommandosAvailable[cardID]);
			mCurCommandosAvailable.RemoveAt(cardID);
		}

		// draw new cards
		DrawCards();

		allianceMenue.UpdateInfo();
	}

	public void ResolveCardUsed(CommandoCardBase card)
	{
		Paused = false;
		cardInspectMenue.Hide();
		card.Action();
	}

	public void InspectCard(int cardID)
	{
		Paused = true;
		cardInspectMenue.CardToInspect = ActiveAlliance.CommandoCardList[cardID];
		cardInspectMenue.Show();
	}

	public void ResumeFromInspect()
	{
		Paused = false;
		cardInspectMenue.Hide();
	}

#endregion

#region game_logics
	public void AllianceGainDominance(int value)
	{
		ActiveAlliance.Dominance += value;
		msgWindow.SetMessage("You gained " + value + " Dominance.");
		if(ActiveAlliance.Dominance >= 6)
		{
			ActiveAlliance.Dominance = 1;
			mCurAction = Actions.Construct_Domninance;
			msgWindow.SetMessage("Dominance exceeded 5! Construct a free cube on any planet.");
		}

		allianceMenue.UpdateInfo();
	}

	public void StartRelocateCube()
	{
		msgWindow.SetMessage("Pick a cube to relocate.");
		mCurAction = Actions.Cards_Relocate_Cube;
	}
	
	public void ResolveRelocateCube(int result)
	{
		if(result == -1)
			msgWindow.SetMessage("Planet type is greater than the planet the cube was taken from.");
		else if(result == -2)
			msgWindow.SetMessage("There is already a quantum cube belonging to this alliance.");
		else
		{
			mAllianceCubeTakenFrom = null;
			mPlanetTypeCubeTakenFrom = -1;
			mCubeTaken = false;
			mCurAction = Actions.None;
		}
	}

	public bool AllianceLooseDominance(Alliance all, int value)
	{
		int tempDom = all.Dominance;
		all.Dominance -= value;
		if(all.Dominance <= 0)
		{
			all.Dominance = tempDom;
			return false;
		}

		allianceMenue.UpdateInfo();
		return true;
	}

	public void AllianceGainResearch(int value)
	{
		msgWindow.SetMessage("You earned " + value + " Research.");
		ActiveAlliance.Research += value;
		if(ActiveAlliance.Research >= 6)
			ActiveAlliance.Research = 6;

		allianceMenue.UpdateInfo();
	}

	public bool AllianceLooseResearch(Alliance all, int value)
	{
		int tempRes = all.Research;
		all.Research -= value;
		if(all.Research <= 0)
		{
			all.Research = tempRes;
			return false;
		}

		allianceMenue.UpdateInfo();
		return true;
	}

	public void CheckAllianceBreakthrough()
	{
		if(ActiveAlliance.Research >= 6)
		{
			// breakthrough
			mAllianceCardsToGain++;
			ActiveAlliance.Research = 1;
		}
	}


#endregion

#region Planet_Handling
	public void SelectAllFreeStartingPlanets()
	{
		for(int i = 0; i < mPlanetList.Count; i++)
		{
			if(mPlanetList[i].IsStartingPlanet == true && mPlanetList[i].HasNoCubes)
			{
				mPlanetList[i].Selected = true;
			}
		}
	}
	
	public void DeselectAllPlanets()
	{
		for(int i = 0; i < mPlanetList.Count; i++)
		{
			mPlanetList[i].Selected = false;
		}
	}

	public void CreateWorld()
	{
		// get maps
		mMaps = new List<LevelMap>();
		LoadMapOne();
		LoadMapTwo();
		LoadMapThree();
		LoadMapFour();
		// create planets
		mPlanetList = new List<Planet>();
		GameObject newPlanetObj;
		Planet newPlanet;

		MapPlanet planetToCreate;

		mNbrOfPlayers = mMaps[mapToPlay].nbrOfPlayers;

		for(int i = 0; i < mMaps[mapToPlay].planets.Count; i++)
		{
			newPlanetObj = Instantiate(prefabPlanet) as GameObject;
			newPlanet = newPlanetObj.GetComponent<Planet>();
			planetToCreate = mMaps[mapToPlay].planets[i];
			newPlanet.Setup(planetToCreate.worldPos, planetSpriteList[Random.Range(0, planetSpriteList.Count)], nebulaSpriteList[Random.Range(0, nebulaSpriteList.Count)], planetToCreate.planetType, planetToCreate.isStartingPlanet);
			mPlanetList.Add(newPlanet);
		}

		// Create Alliances
		mAllianceList = new List<Alliance>();
		GameObject newAlObj;
		Alliance newAlliance;
		
		for(int i = 0; i < mNbrOfPlayers; i++)
		{
			//newAlObj = Instantiate(prefabPlayer) as GameObject;
			newAlliance = new Alliance();
			newAlliance.Setup(i, allianceColors[i], allianceNames[i], mMaps[mapToPlay].cubesPerPlayer);
			//mPlanetList[startPlanPos[i]].ConstructCube(newAlliance, true);
			mAllianceList.Add(newAlliance);
		}

		// first phase is startup
		mPhaseList = new List<Phase>();
		mPhaseList.Add(new Phase(PhaseType.StartupPhase, 0, 0));
	}

	public void ResolveStartingPlanetClicked(Planet planet)
	{
		if(!planet.IsStartingPlanet)
		{
			msgWindow.SetMessage("You have to select a starting planet!");
			return;
		}

		if(planet.ConstructCube(ActiveAlliance, true) == 0)
		{
			ActiveAlliance.StartingPlanet = planet;
			ActiveAlliance.PlaceMotherShipAndScrapyard();

			DeselectAllPlanets();
			// cube could be contructed, so go to next player
			mActivePlayer++;
			if(mActivePlayer > mNbrOfPlayers - 1)
			{
				// go to deploy ships state
				mActivePlayer = 0;
				BeginShipStartupDeploy();
			}
			else
			{
				SelectAllFreeStartingPlanets();
			}
			allianceMenue.UpdateInfo();
			systemControlWnd.UpdateInfo();
		}
	}

	public void LoadMapOne()
	{
		LevelMap newMap = new LevelMap(2, 7, "Terra Major");

		newMap.AddPlanet(true, 8, new Vector2(0, -2));

		newMap.AddPlanet(false, 7, new Vector2(-1,-1));
		newMap.AddPlanet(false, 8, new Vector2(0,-1));
		newMap.AddPlanet(false, 7, new Vector2(1,-1));

		newMap.AddPlanet(false, 8, new Vector2(-1,0));
		newMap.AddPlanet(false, 7, new Vector2(0,0));
		newMap.AddPlanet(false, 8, new Vector2(1,0));

		newMap.AddPlanet(false, 7, new Vector2(-1,1));
		newMap.AddPlanet(false, 8, new Vector2(0,1));
		newMap.AddPlanet(false, 7, new Vector2(1,1));

		newMap.AddPlanet(true, 8, new Vector2(0, 2));

		mMaps.Add(newMap);
	}

	public void LoadMapTwo()
	{
		LevelMap newMap = new LevelMap(2, 5, "Null Hypothesis");

		newMap.AddPlanet(true, 7, new Vector2(0, -1));
		newMap.AddPlanet(false, 8, new Vector2(-2, 0));
		newMap.AddPlanet(false, 8, new Vector2(-1, 0));
		newMap.AddPlanet(false, 7, new Vector2(0, 0));
		newMap.AddPlanet(false, 8, new Vector2(1, 0));
		newMap.AddPlanet(false, 8, new Vector2(2, 0));
		newMap.AddPlanet(true, 7, new Vector2(0, 1));

		mMaps.Add(newMap);
	}

	public void LoadMapThree()
	{
		LevelMap newMap = new LevelMap(3, 6, "Equinox");

		newMap.AddPlanet(false, 7, new Vector2(-1, 1));
		newMap.AddPlanet(false, 7, new Vector2(1, 1));

		newMap.AddPlanet(false, 7, new Vector2(-2, 0));
		newMap.AddPlanet(false, 8, new Vector2(-1, 0));
		newMap.AddPlanet(true, 9, new Vector2(0, 0));
		newMap.AddPlanet(false, 8, new Vector2(1, 0));
		newMap.AddPlanet(false, 7, new Vector2(2, 0));

		newMap.AddPlanet(true, 9, new Vector2(-1, -1));
		newMap.AddPlanet(false, 8, new Vector2(0, -1));
		newMap.AddPlanet(true, 9, new Vector2(1, -1));

		newMap.AddPlanet(false, 7, new Vector2(0, -2));
		
		mMaps.Add(newMap);
	}

	public void LoadMapFour()
	{
		LevelMap newMap = new LevelMap(4, 7, "Gordian Knot");
		
		newMap.AddPlanet(true, 7, new Vector2(-1, 2));
		newMap.AddPlanet(false, 7, new Vector2(0, 2));

		newMap.AddPlanet(false, 8, new Vector2(0, 1));

		newMap.AddPlanet(true, 7, new Vector2(-3, 0));
		newMap.AddPlanet(false, 10, new Vector2(-1, 0));
		newMap.AddPlanet(false, 10, new Vector2(0, 0));
		newMap.AddPlanet(false, 8, new Vector2(1, 0));
		newMap.AddPlanet(false, 7, new Vector2(2, 0));

		newMap.AddPlanet(false, 7, new Vector2(-3, -1));
		newMap.AddPlanet(false, 8, new Vector2(-2, -1));
		newMap.AddPlanet(false, 10, new Vector2(-1, -1));
		newMap.AddPlanet(false, 10, new Vector2(0, -1));
		newMap.AddPlanet(true, 7, new Vector2(2, -1));

		newMap.AddPlanet(false, 8, new Vector2(-1, -2));

		newMap.AddPlanet(false, 7, new Vector2(-1, -3));
		newMap.AddPlanet(true, 7, new Vector2(0, -3));

		mMaps.Add(newMap);
	}
#endregion
}
