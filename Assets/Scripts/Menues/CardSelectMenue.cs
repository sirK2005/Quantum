using UnityEngine;
using System.Collections;

public class CardSelectMenue : ScreenBase {

	public UIToggle[] tglGambitCards;
	public UILabel[] lblGambitCardNames;

	public UIToggle[] tglCommandoCards;
	public UILabel[] lblCommandoCardNames;

	public UILabel lblCardTitle;
	public UILabel lblCardDescrip;

	public int GetGambitSelected
	{
		get
		{
			for(int i = 0; i < 3; i++)
			{
				if(tglGambitCards[i].value)
				   return i;
			}
			//Debug.Log("nothing selected?");
			return -1;
		}
	}

	public int GetCommandoSelected
	{
		get
		{
			for(int i = 0; i < 3; i++)
			{
				if(tglCommandoCards[i].value)
					return i;
			}
			//Debug.Log("nothing selected?");
			return -1;
		}
	}

	public override void Show()
	{
		base.Show();

		// setup card texts
		if(GetGambitSelected != -1)
		{
			lblCardTitle.text = GameLogic.Instance.CurGambitsAvailable[GetGambitSelected].Title;
			lblCardDescrip.text = GameLogic.Instance.CurGambitsAvailable[GetGambitSelected].Description;
		}
		else
		{
			lblCardTitle.text = GameLogic.Instance.CurCommandosAvailable[GetCommandoSelected].Title;
			lblCardDescrip.text = GameLogic.Instance.CurCommandosAvailable[GetCommandoSelected].Description;
		}

		// setup card titles
		for(int i = 0; i < 3; i++)
		{
			lblGambitCardNames[i].text = GameLogic.Instance.CurGambitsAvailable[i].Title;
		}
		for(int i = 0; i < 3; i++)
		{
			lblCommandoCardNames[i].text = GameLogic.Instance.CurCommandosAvailable[i].Title;
		}
	}

	public void UpdateCardSelected()
	{
		if(GameLogic.Instance.CurGambitsAvailable == null)
			return;

		if(GetGambitSelected != -1)
		{
			lblCardTitle.text = GameLogic.Instance.CurGambitsAvailable[GetGambitSelected].Title;
			lblCardDescrip.text = GameLogic.Instance.CurGambitsAvailable[GetGambitSelected].Description;
		}
		if(GetCommandoSelected != -1)
		{
			lblCardTitle.text = GameLogic.Instance.CurCommandosAvailable[GetCommandoSelected].Title;
			lblCardDescrip.text = GameLogic.Instance.CurCommandosAvailable[GetCommandoSelected].Description;
		}
	}

	public void TakeCardPressed()
	{
		if(GetGambitSelected != -1)
			GameLogic.Instance.ResolveCardsSelected(GameLogic.Instance.CurGambitsAvailable[GetGambitSelected], GetGambitSelected);
		if(GetCommandoSelected != -1)
			GameLogic.Instance.ResolveCardsSelected(GameLogic.Instance.CurCommandosAvailable[GetCommandoSelected], GetCommandoSelected);
	}
}
