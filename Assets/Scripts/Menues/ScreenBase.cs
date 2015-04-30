using UnityEngine;
using System.Collections;

public class ScreenBase : MonoBehaviour {

	public bool startsHidden;

	private UIRoot mRoot;
	
	protected bool mIsShown;
	
	protected virtual void Awake()
	{
		mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
	}

	protected virtual void Start()
	{
		if(startsHidden)
			Hide();
		else
			Show();
	}

	public virtual void Hide()
	{
		transform.Translate(0, mRoot.activeHeight, 0);
		mIsShown = false;
	}
	
	public virtual void Show()
	{
		if(!mIsShown){
			transform.Translate(0, -mRoot.activeHeight, 0);
			mIsShown = true;
		}
	}
}
