using UnityEngine;
using System.Collections;

public class CircleMenue : MonoBehaviour {

	public UILabel movingPointsLeftLabel;

	public UIPanel panel;

	public Camera gameCamera;
	public Camera nGUICamera;

	private bool mShow;

	// Use this for initialization
	void Start () {
		// disable menue elements
		Hide();
	}

	public void Show()
	{
		// show btns
		mShow = true;
		panel.enabled = true;
	}

	public void Hide()
	{
		mShow = false;
		//panel.enabled = false;

		// disable menue elements
		transform.Translate(-100, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(!mShow)
			return;

		float scaleFac = -6 / GameLogic.Instance.gameCamera.transform.localPosition.z;
		transform.localScale = new Vector3(scaleFac, scaleFac, 1);

		if(GameLogic.Instance.ShipSelected != null)
		{
			// Follow Ship
			Ship selShip = GameLogic.Instance.ShipSelected;
			Vector3 screenPos = gameCamera.WorldToViewportPoint(selShip.transform.position);
			
			ObjectFollowPos(transform, screenPos, new Vector2(0, 0));

			movingPointsLeftLabel.enabled = true;
			movingPointsLeftLabel.text = GameLogic.Instance.ShipSelected.MPLeft.ToString();
		}
	}

	// makes a transform follow a gameobject
	private void ObjectFollowPos(Transform objTrans, Vector3 screenPos, Vector2 offset)
	{
		objTrans.position = nGUICamera.ViewportToWorldPoint(screenPos);
		screenPos = objTrans.localPosition;
		screenPos.x = Mathf.RoundToInt(screenPos.x + offset.x);
		screenPos.y = Mathf.RoundToInt(screenPos.y + offset.y);
		screenPos.z = 0f;
		objTrans.localPosition = screenPos;
	}

}
