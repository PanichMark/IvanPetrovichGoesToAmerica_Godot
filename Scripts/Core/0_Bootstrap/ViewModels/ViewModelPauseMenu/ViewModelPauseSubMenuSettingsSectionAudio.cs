using Godot;
public class ViewModelPauseSubMenuSettingsSectionAudio
{
	public Node[] ButtonsChangeLanguage = new Node[2];
	public Node TextChangeLanguage;

	public Node SliderVolumeGeneral;
	public Node NumberSliderVolumeGeneral;
	public Node TextSliderVolumeGeneral;

	public Node SliderVolumeEnvironment;
	public Node NumberSliderVolumeEnvironment;
	public Node TextSliderVolumeEnvironment;

	public Node SliderVolumeEffects;
	public Node NumberSliderVolumeEffects;
	public Node TextSliderVolumeEffects;

	public Node SliderVolumeVoices;
	public Node NumberSliderVolumeVoices;
	public Node TextSliderVolumeVoices;

	public Node SliderVolumeMusicAmbience;
	public Node NumberSliderVolumeMusicAmbience;
	public Node TextSliderVolumeMusicAmbience;

	public Node SliderVolumeMusicIngame;
	public Node NumberSliderVolumeMusicIngame;
	public Node TextSliderVolumeMusicIngame;

	public ViewModelPauseSubMenuSettingsSectionAudio(Bootstrap bootstrap, Node canvas)
	{
		ButtonsChangeLanguage[0] = bootstrap.FindDeepNode(canvas, "ButtonChangeLanguageRussian");
		ButtonsChangeLanguage[1] = bootstrap.FindDeepNode(canvas, "ButtonChangeLanguageEnglish");

		TextChangeLanguage = bootstrap.FindDeepNode(canvas, "TextChangeLanguage");

		SliderVolumeGeneral = bootstrap.FindDeepNode(canvas, "SliderVolumeGeneral");
		NumberSliderVolumeGeneral = bootstrap.FindDeepNode(canvas, "NumberVolumeGeneral");
		TextSliderVolumeGeneral = bootstrap.FindDeepNode(canvas, "TextVolumeGeneral");

		SliderVolumeEnvironment = bootstrap.FindDeepNode(canvas, "SliderVolumeEnvironment");
		NumberSliderVolumeEnvironment = bootstrap.FindDeepNode(canvas, "NumberVolumeEnvironment");
		TextSliderVolumeEnvironment = bootstrap.FindDeepNode(canvas, "TextVolumeEnvironment");

		SliderVolumeEffects = bootstrap.FindDeepNode(canvas, "SliderVolumeEffects");
		NumberSliderVolumeEffects = bootstrap.FindDeepNode(canvas, "NumberVolumeEffects");
		TextSliderVolumeEffects = bootstrap.FindDeepNode(canvas, "TextVolumeEffects");

		SliderVolumeVoices = bootstrap.FindDeepNode(canvas, "SliderVolumeVoices");
		NumberSliderVolumeVoices = bootstrap.FindDeepNode(canvas, "NumberVolumeVoices");
		TextSliderVolumeVoices = bootstrap.FindDeepNode(canvas, "TextVolumeVoices");

		SliderVolumeMusicAmbience = bootstrap.FindDeepNode(canvas, "SliderVolumeMusicAmbience");
		NumberSliderVolumeMusicAmbience = bootstrap.FindDeepNode(canvas, "NumberVolumeMusicAmbience");
		TextSliderVolumeMusicAmbience = bootstrap.FindDeepNode(canvas, "TextVolumeMusicAmbience");

		SliderVolumeMusicIngame = bootstrap.FindDeepNode(canvas, "SliderVolumeMusicIngame");
		NumberSliderVolumeMusicIngame = bootstrap.FindDeepNode(canvas, "NumberVolumeMusicIngame");
		TextSliderVolumeMusicIngame = bootstrap.FindDeepNode(canvas, "TextVolumeMusicIngame");
	}
}