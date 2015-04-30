using UnityEngine;
using System.Collections;

public class GambitCardBase : CardBase{

	protected string mTitle;
	public string Title {get{return mTitle;}}

	protected string mDescription;
	public string Description {get{return mDescription;}}

	public GambitCardBase()
	{
		mCardType = CardType.Gambit;
	}

	public virtual void Action()
	{
	}

	public virtual void OnGet()
	{
	}

	public virtual void OnTake()
	{
	}
}
