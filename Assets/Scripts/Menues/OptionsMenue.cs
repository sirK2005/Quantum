using UnityEngine;
using System.Collections;

public class OptionsMenue : ScreenBase {

	public UISlider zoomSpdSlider;
	public UILabel sliderLabel;
	public UIToggle tglMusic;

	public AudioSource music;

	// Use this for initialization
	protected override void Start () {

		base.Start();
		tglMusic.value = !music.mute;
	}

	public void SliderUpdate()
	{
		GameLogic.Instance.mDragFactor = zoomSpdSlider.value / 10;
		sliderLabel.text = zoomSpdSlider.value.ToString();
	}

	public void OnQuitPressed()
	{
		Application.Quit();
	}

	public void OnResumePressed()
	{
		GameLogic.Instance.ResumeFromOptioneMenue(zoomSpdSlider.value);
	}

	public void SwitchMusicPressed()
	{
		music.mute = !music.mute;
	}

}
