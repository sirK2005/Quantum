using UnityEngine;
using System.Collections;

public enum ActivationType
{
	Passive,
	TurnStart,
	TurnEnd,
	OnUse,
}

public class CommandoCardBase : CardBase{

	protected string mTitle;
	public string Title {get{return mTitle;}}
	
	protected string mDescription;
	public string Description {get{return mDescription;} }

	protected ActivationType mActType;
	public ActivationType ActType {get{return mActType;}}

	public bool UsedThisTurn {get; set;}

	public CommandoCardBase()
	{
		mCardType = CardType.Commando;
	}

	public virtual void Reset()
	{
		UsedThisTurn = false;
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
