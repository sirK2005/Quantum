using UnityEngine;
using System.Collections;

public enum CardType
{
	Gambit = 0,
	Commando = 1,
}


public class CardBase {

	protected CardType mCardType;
	public CardType TypeOfCard { get{return mCardType;} }

}
