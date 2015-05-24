using UnityEngine;
using System.Collections;

public class PanelsManager : MonoBehaviour {

	public Panel activePanel = Panel.StartGame;

	public enum Panel {
		StartGame,
		RegionSelect,
		Playing,
		Pause,
		GameOver,


		Quitting
	}

	public CanvasGroup tint;
	public CanvasGroup gameUI;
	public CanvasGroup startPanel;
	public CanvasGroup regionSelectorPanel;
	public CanvasGroup pausePanel;

	public CanvasManager checkpointManager;
	public ContinousMovement vehicle;

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
	}

	private void Update() {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			Back();
		}
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
				activePanel = Panel.StartGame;
				checkpointManager.GoBackToMainFromCheckpoint();
				TweenCanvasAlpha.Show(new TweenParameters(startPanel, 0.015f, 1f, 1f, 0f));
				TweenCanvasAlpha.Show(new TweenParameters(regionSelectorPanel, 1f, 0f, 1f, 0f));
				MakeInteractable(startPanel);
				break;

			case(Panel.Playing):
				activePanel = Panel.Pause;
				PauseGame();
				break;
		}
	}

	private void MakeInteractable(CanvasGroup cg) {
		cg.interactable = true;
		cg.blocksRaycasts = true;
	}

	public void PauseGame() {
		vehicle.isPaused = true;
		Time.timeScale = 0.0f;
	}

	public void ResumeGame() {
		Time.timeScale = 1f;
		vehicle.isPaused = false;
	}




	public void StartGame() {
		checkpointManager.StartCoroutine("decreaseCheckpointsVisibility");
		TweenCanvasAlpha.Show(new TweenParameters(gameUI, 0f, 1f, 1f, 1f));
		TweenCanvasAlpha.Show(new TweenParameters(regionSelectorPanel, 1f, 0f, 0.5f, 0f));
		gameUI.interactable = true;
		gameUI.blocksRaycasts = true;
		activePanel = Panel.Playing;
	}
}

