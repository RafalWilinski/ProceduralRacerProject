using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelsManager : MonoBehaviour {

	public Panel activePanel = Panel.StartGame;
	public ControlType controlType = ControlType.MouseAndKeyboard;

	public enum Panel {
		StartGame,
		RegionSelect,
		Playing,
		Pause,
		GameOver,
		Settings,
		ControlsSettings,
		RewindPanel,
		Quitting,
		About
	}

	public enum ControlType {
		MouseAndKeyboard,
		Joystick,
		Accelerometer
	}

	public struct CanvasAndDelay {
		public CanvasGroup cg;
		public float delay;
	}

	public bool isDebug;

	public GameObject cam;
	public CanvasGroup tint;
	public CanvasGroup gameUI;
	public CanvasGroup startPanel;
	public CanvasGroup regionSelectorPanel;
	public CanvasGroup pausePanel;
	public CanvasGroup regionIntroduce;
	public CanvasGroup settingsPanel;
	public CanvasGroup controlsPanel;
	public CanvasGroup rewindPanel;
	public CanvasGroup gameoverPanel;
	public CanvasGroup distanceGameOverPanel;
	public CanvasGroup tournamentGameOverPanel;
	public CanvasGroup firstRegionCanvas;
	public CanvasGroup aboutCanvas;
	public CanvasGroup aboutCanvasContent;

	public GameObject gameUIFirstSelectedButton;
	public GameObject settingsFirstSelectedButton;
	public GameObject gameOverFirstSelectedButton;

	public ControllerConnectedModal aboutModal;	

	public Text selectedGameObjectLabel;

	public CanvasManager checkpointManager;
	public ContinousMovement vehicle;
	public RegionSelector regionSelector;
	public CheckpointsCreator checkpointsCreator;
	public GameOverScenario gameOverScenario;

	public float alphaAnimationTime = 0.4f;


	//Singleton
	private static PanelsManager _instance;

	public static PanelsManager Instance {
		get {
			return _instance;
		}
	}

	private void Awake() {
		_instance = this;
		TweenCanvasAlpha.Show(new TweenParameters(tint, 1f, 0f, 2f, 1f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0.015f, 1f, 2f, 2f));

		EventSystem.current.SetSelectedGameObject (gameUIFirstSelectedButton);
	}

	private void Update() {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			Back();
		}

//		else if(Input.Get)

		if(activePanel == Panel.RegionSelect) {
			if(Input.GetKeyUp(KeyCode.UpArrow)) regionSelector.NextRegion();
			if(Input.GetKeyUp(KeyCode.DownArrow)) regionSelector.PreviousRegion();
		}

		if(isDebug) selectedGameObjectLabel.text = "Selected: "+EventSystem.current.currentSelectedGameObject;
	}

	public void SetActivePanel(Panel panel) {
		Debug.Log("New active panel: "+panel.ToString());
		this.activePanel = panel;
	}

	public void Back() {
		Debug.Log("Back "+activePanel.ToString());
		switch(activePanel) {
			case(Panel.StartGame):
				TweenCanvasAlpha.Show(new TweenParameters(tint, 0f, 1f, 2f, 1f));
				activePanel = Panel.Quitting;
				Application.Quit();
				break;

			case(Panel.RegionSelect):
				BackFromCheckpoints();
				EventSystem.current.SetSelectedGameObject(gameUIFirstSelectedButton);
				break;

			case(Panel.Playing):
				PauseGame();
				break;

			case(Panel.Pause):
				ResumeGame();
				break;

			case(Panel.Settings):
				BackFromSettings();
				EventSystem.current.SetSelectedGameObject(gameUIFirstSelectedButton);
				break;

			case(Panel.ControlsSettings):
				BackFromControlSettings();
				break;

			case(Panel.RewindPanel):
				vehicle.GameOver();
				break;

			case(Panel.GameOver):
				BackFromGameOverPanel();
				break;

			case(Panel.About):
				BackFromAboutPanel();
				break;
		}
	}

	public void MakeInteractable(CanvasGroup cg) {
		cg.interactable = true;
		cg.blocksRaycasts = true;
	}

	public void MakeUninteractable(CanvasGroup cg) {
		cg.interactable = false;
		cg.blocksRaycasts = false;
	}

	public void ShowCanvasImmediately(CanvasGroup cg) {
		MakeInteractable(cg);
		cg.alpha = 1;
	}

	private void HideCanvasImmediately(CanvasGroup cg) {
		MakeUninteractable(cg);
		cg.alpha = 0;
	}

	private void MakeUninteractable(CanvasGroup cg, float delay) {
		CanvasAndDelay cnd = new CanvasAndDelay();
		cnd.cg = cg;
		cnd.delay = delay;

		StartCoroutine("WaitAndMakeUninteractable", cnd);

	}	

	private IEnumerator WaitAndMakeUninteractable(CanvasAndDelay cnd) {
		yield return new WaitForSeconds(cnd.delay);
		MakeUninteractable(cnd.cg);
	}

	private IEnumerator ReloadLevel(float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel(Application.loadedLevel);
	}

	//Canvas Specific function

	public void PauseGame() {
		vehicle.isPaused = true;
		Time.timeScale = 0.0f;
		activePanel = Panel.Pause;
		HideCanvasImmediately(regionIntroduce);
		ShowCanvasImmediately(pausePanel);
	}

	public void ResumeGame() {
		Time.timeScale = 1f;
		vehicle.isPaused = false;
		activePanel = Panel.Playing;
		HideCanvasImmediately(pausePanel);
	}

	public void StartGame() {
		checkpointManager.StartCoroutine("decreaseCheckpointsVisibility");
		TweenCanvasAlpha.Show(new TweenParameters(gameUI, 0f, 1f, alphaAnimationTime, 1f));
		TweenCanvasAlpha.Show(new TweenParameters(regionSelectorPanel, 1f * regionSelectorPanel.alpha, 0f, alphaAnimationTime, 0f));
		gameUI.interactable = true;
		gameUI.blocksRaycasts = true;
		activePanel = Panel.Playing;
	}

	public void ShowAbout() {
		StopCoroutine("AboutShowScenario");
		StartCoroutine("AboutShowScenario");
	}

	private IEnumerator AboutShowScenario() {
		aboutCanvasContent.alpha = 0;
		activePanel = Panel.About;
		ShowCanvasImmediately(aboutCanvas);
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 1f, 0f, alphaAnimationTime, 0f));
		LeanTween.moveLocal (cam, Vector3.zero, 0.75f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, Vector3.zero, 0.75f).setEase( LeanTweenType.easeInOutCubic );
		yield return new WaitForSeconds(1f);
		aboutModal.Show();
		yield return new WaitForSeconds(1f);
		TweenCanvasAlpha.Show(new TweenParameters(aboutCanvasContent, 0f, 1f, alphaAnimationTime, 1f));
		MakeInteractable(aboutCanvasContent);
		
	}

	public void ShowCheckpoints() {
		activePanel = Panel.RegionSelect;
		MakeUninteractable(startPanel);
		MakeInteractable(regionSelectorPanel);
		checkpointsCreator.MakeActive();
		MakeInteractable(firstRegionCanvas);
		TweenCanvasAlpha.Show(new TweenParameters(regionSelectorPanel, 0f, 1f, alphaAnimationTime, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 1f, 0f, alphaAnimationTime, 0f));
	}

	public void BackFromCheckpoints() {
		checkpointsCreator.MakeInactive();
		activePanel = Panel.StartGame;
		checkpointManager.GoBackToMainFromCheckpoint();
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0.015f, 1f, 1f, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(regionSelectorPanel, 1f, 0f, 1f, 0f));
		MakeInteractable(startPanel);
	}

	public void ShowSettings() {
		//StartCoroutine("TweenMenuFwdSpeed");
		activePanel = Panel.Settings;
		MakeUninteractable(startPanel);
		MakeInteractable(settingsPanel);
		TweenCanvasAlpha.Show(new TweenParameters(settingsPanel, 0f, 1f, alphaAnimationTime, 1f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 1f, 0f, alphaAnimationTime, 0f));

		EventSystem.current.SetSelectedGameObject(settingsFirstSelectedButton);

		LeanTween.moveLocal (cam, Vector3.zero, 0.75f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, Vector3.zero, 0.75f).setEase( LeanTweenType.easeInOutCubic );
	}

	public void BackFromSettings() {
		activePanel = Panel.StartGame;
		MakeUninteractable(settingsPanel);
		MakeInteractable(startPanel);
		TweenCanvasAlpha.Show(new TweenParameters(settingsPanel, 1f, 0f, alphaAnimationTime, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0f, 1f, alphaAnimationTime, 1f));

		EventSystem.current.SetSelectedGameObject(gameUIFirstSelectedButton);

		LeanTween.moveLocal (cam, new Vector3(0, 27.4f, -32f), 0.75f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, new Vector3(40f,0f,0f), 0.75f).setEase( LeanTweenType.easeInOutCubic );
	}

	public void ShowControls() {
		activePanel = Panel.ControlsSettings;
		MakeUninteractable(settingsPanel);
		MakeInteractable(controlsPanel);
		TweenCanvasAlpha.Show(new TweenParameters(controlsPanel, 0f, 1f, alphaAnimationTime, 1f));
		TweenCanvasAlpha.Show(new TweenParameters(settingsPanel, 1f, 0f, alphaAnimationTime, 0f));
	}

	public void BackFromControlSettings() {
		activePanel = Panel.StartGame;
		MakeUninteractable(controlsPanel);
		MakeInteractable(startPanel);
		TweenCanvasAlpha.Show(new TweenParameters(controlsPanel, 1f, 0f, alphaAnimationTime, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0f, 1f, alphaAnimationTime, 1f));

		EventSystem.current.SetSelectedGameObject(gameUIFirstSelectedButton);

		LeanTween.moveLocal (cam, new Vector3(0, 27.4f, -32f), 0.75f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, new Vector3(40f,0f,0f), 0.75f).setEase( LeanTweenType.easeInOutCubic );
	}

	public void ShowRewindPanel() {
		activePanel = Panel.RewindPanel;
		ShowCanvasImmediately(rewindPanel);
	}

	public void BackFromRewindPanel() {
		vehicle.GameOver();
	}

	private void HideRewindPanel() {
		HideCanvasImmediately(rewindPanel);
	}

	public void ShowGameOverPanel() {
		activePanel = Panel.GameOver;
		gameOverScenario.StartScenario();
		HideRewindPanel();
		TweenCanvasAlpha.Show(new TweenParameters(tint, 1f, 0f, 2f, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(gameUI, 1f, 0f, 1f, 0f));
		// ShowCanvasImmediately(gameoverPanel);
	}

	public void HideGameOverPanel() {
		activePanel = Panel.Playing;
		HideRewindPanel();
		TweenCanvasAlpha.Show(new TweenParameters(gameoverPanel, 1f, 0f, 1f, 0f));
		MakeUninteractable(gameoverPanel, 0.5f);
	}

	public void BackFromGameOverPanel() {
		TweenCanvasAlpha.Show(new TweenParameters(tint, 0f, 1f, 1f, 0f));
		StartCoroutine(ReloadLevel(1));
	}

	public void BackFromAboutPanel() {
		StopCoroutine("AboutShowScenario");
		activePanel = Panel.StartGame;
		LeanTween.moveLocal (cam, new Vector3(0, 27.4f, -32f), 0.75f).setEase( LeanTweenType.easeInOutCubic);
		LeanTween.rotate (cam, new Vector3(40f,0f,0f), 0.75f).setEase( LeanTweenType.easeInOutCubic );
		MakeInteractable(startPanel);
		MakeUninteractable(aboutCanvas);
		MakeUninteractable(aboutCanvasContent);

		TweenCanvasAlpha.Show(new TweenParameters(aboutCanvas, 1f, 0f, alphaAnimationTime, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0f, 1f, alphaAnimationTime, 1f));
	}
}

