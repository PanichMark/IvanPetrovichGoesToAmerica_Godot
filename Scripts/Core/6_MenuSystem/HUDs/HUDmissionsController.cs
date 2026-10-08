using Godot;
using System.Collections;
public partial class HUDmissionsController : Node
{
	private MenuManager _menuManager;
	private Node _canvasHUDmissions;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private Node _HUDmission;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private LocalizationManager _localizationManager;
	private Node _textNewMissionGoal;
	private Label _textComponentNewMissionGoal;
	private Node _textNewMissionGoalDisplay;
	private Label _textComponentNewMissionGoalDisplay;

	private Node _textCurrentMissionGoal;
	private Label _textComponentCurrentMissionGoal;

	private string _textGoal;

	public void Initialize(
		GameController gameController,
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		Node canvasHUDmissions,
		ViewModelPauseMenu viewModelPauseMenu,
		ViewModelHUDMission viewModelHUDMission)
	{
		_gameController = gameController;
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasHUDmissions = canvasHUDmissions;
		_HUDmission = viewModelHUDMission.HUDmission;

		_textNewMissionGoal = viewModelHUDMission.TextNewMissionGoal;
		_textComponentNewMissionGoal = viewModelHUDMission.TextNewMissionGoal.GetNodeOrNull<Label>();
		_textNewMissionGoalDisplay = viewModelHUDMission.TextNewMissionGoalDisplay;
		_textComponentNewMissionGoalDisplay = viewModelHUDMission.TextNewMissionGoalDisplay.GetNodeOrNull<Label>();

		_textCurrentMissionGoal = viewModelPauseMenu.TextCurrentMissionGoalDisplay;
		_textComponentCurrentMissionGoal = _textCurrentMissionGoal.GetNodeOrNull<Label>();

		_menuManager.OnOpenPauseMenu += HideCanvasHUDmissions;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDmissions;
		_menuManager.OnOpenCutsceneMenu += HideCanvasHUDmissions;
		_menuManager.OnCloseCutsceneMenu += ShowCanvasHUDmissions;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDmission;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDmission;

		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += HideCanvasHUDmissions;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDmissions;

		_gameSceneManager.OnEndLoadingGameplayScene += () =>
		{
			/*
			if (SceneManager.sceneCount > 1)
			{
				//GD.Print("MISSIONBREUH");
				//GD.Print(SceneManager.GetSceneAt(1).name);

				if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
				{
					_textComponentCurrentMissionGoal.text = _textGoal;
					//GD.Print("SHOW");
				}
				else
				{
					//._textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
					_textComponentCurrentMissionGoal.text = null;
					//GD.Print("DONT SHOW");
				}
			}
			*/
		};

		_gameController.OnPlayerEarlyDeath += HideCanvasHUDmissions;
	}

	public void SetCurrentMissionGoalText(string textGoal)
	{
		_textGoal = textGoal;

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				_textComponentCurrentMissionGoal.text = _textGoal;
			}
			else
			{
				//._textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
				_textComponentCurrentMissionGoal.text = null;
			}
		}
	}

	public void ShowNewMissionGoalHUDnotification(string textGoal, bool isNewGoal)
	{
		if (SceneManager.sceneCount > 1)
		{
			GD.Print("NEW MISSION NOTIFICATION!");
		StopAllCoroutines();

		
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
			{
				StartCoroutine(ShowNewMissionGoalHUDnotificationCoroutine(textGoal, isNewGoal));
				//GD.Print("SHOW");
			}
		}
	}

	private IEnumerator ShowNewMissionGoalHUDnotificationCoroutine(string textGoal, bool isNewGoal)
	{
		_textNewMissionGoal .Set("visible", true);
		_textNewMissionGoalDisplay .Set("visible", true);

		if (isNewGoal)
		{
			_textComponentNewMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_New");
		}
		else
		{
			_textComponentNewMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_MissionGoal_Current");
		}

		_textComponentNewMissionGoalDisplay.text = textGoal;

		yield return new WaitForSeconds(3);

		HideNewMissionGoalHUDnotification();
	}

	private void HideNewMissionGoalHUDnotification()
	{
		_textNewMissionGoal .Set("visible", false);
		_textNewMissionGoalDisplay .Set("visible", false);
	}

	private void ShowCanvasHUDmissions()
	{
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOrEndGameTitlesActive && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded)
		{
			_canvasHUDmissions .Set("visible", true);

			GD.Print("Show canvasMissions");
		}
	}

	private void HideCanvasHUDmissions()
	{
		HideNewMissionGoalHUDnotification();

		_canvasHUDmissions .Set("visible", false);

		GD.Print("Hide canvasMissions");
	}

	private void ShowHUDmission()
	{
		_HUDmission .Set("visible", true);
	}

	private void HideHUDmission()
	{
		_HUDmission .Set("visible", false);
	}
}