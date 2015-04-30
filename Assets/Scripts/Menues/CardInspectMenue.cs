using UnityEngine;
using System.Collections;

public class CardInspectMenue : ScreenBase {

	public UILabel lblTitle;
	public UILabel lblDescrip;

	public UIButton btnUseCard;

	private CommandoCardBase mCardInspected;
	public CommandoCardBase CardToInspect {get; set;}

	public override void Show ()
	{
		base.Show ();

		lblTitle.text = CardToInspect.Title;
		lblDescrip.text = CardToInspect.Description;

		if(CardToInspect.ActType == ActivationType.OnUse && CardToInspect.UsedThisTurn == false)
			btnUseCard.enabled = true;
		else
			btnUseCard.enabled = false;
	}

	public void UseCardPressed()
	{
		GameLogic.Instance.ResolveCardUsed(CardToInspect);
	}

	public void ResumcePressed()
	{
		GameLogic.Instance.ResumeFromInspect();
	}

}
