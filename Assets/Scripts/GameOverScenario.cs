using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOverScenario : MonoBehaviour {

	public CanvasGroup gameoverPanel;
	public CanvasGroup distanceGameOverPanel;
	public CanvasGroup tournamentGameOverPanel;
	public CanvasGroup tapToContinue;

	public List<TimeLineEvent> timelineEvents;
	public GameObject regionMarkerPrefab;
	public GameObject lastRunMarker;
	public Image distanceProgressBar;
	public Text distanceLabel;
	public Text bestDistanceLabel;

	public PanelsManager manager;

	public float initialWaitTime = 1.0f;
	public float timelineAnimationTime = 1.5f;

	private int distance;

	[SerializeField]
	public class TimeLineEvent {
		public string name;
		public int distance;
		public float timelinePlace; 
		public GameObject marker;
		public bool isRevealed;
		
		public TimeLineEvent(string name, int distance) {
			this.name = name;
			this.distance = distance;
		}

		public void Reveal() {
			if(marker != null) {
				if(!isRevealed) {
					isRevealed = true;
					marker.SendMessage("Reveal");
				}
			}
			else {
				Debug.Log("Marker for event "+name+" is missing!");
			}
		}
	}

	public int DistanceTravelled {
		set {
			distance = value;
			if(PlayerPrefs.GetInt("best_distance") < distance) {
				PlayerPrefs.SetInt("best_distance", distance);
			}
		}
	}

	private void Awake() {
		timelineEvents = new List<TimeLineEvent>();
	}

	public void StartScenario() {
		manager.HideCanvasImmediately(tournamentGameOverPanel);
		manager.HideCanvasImmediately(tapToContinue);
		manager.MakeUninteractable(tapToContinue);
		manager.ShowCanvasImmediately(distanceGameOverPanel);
		ComputeEventsPlace();
		StartCoroutine("TimelineAnimation");

		bestDistanceLabel.text = "Best: "+PlayerPrefs.GetInt("best_distance").ToString()+"m";
		if(PlayerPrefs.GetInt("best_distance") == distance) {
			bestDistanceLabel.text = "NEW HIGHSCORE";
		}
	}
	
	public void AddEvent(string name, int distance) {
		timelineEvents.Add(new TimeLineEvent(name, distance));
	}

	public void ShowLeaderboard() {
		manager.MakeUninteractable(tapToContinue);
		manager.MakeInteractable(tournamentGameOverPanel);
		TweenCanvasAlpha.Show(new TweenParameters(distanceGameOverPanel, 1f, 0f, 1f, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(tournamentGameOverPanel, 0f, 1f, 1f, 1f));
	}

	public void ShowSummary() {
		manager.MakeInteractable(gameoverPanel);
		manager.MakeUninteractable(tournamentGameOverPanel);
		TweenCanvasAlpha.Show(new TweenParameters(tournamentGameOverPanel, 1f, 0f, 1f, 0f));
		TweenCanvasAlpha.Show(new TweenParameters(gameoverPanel, 0f, 1f, 1f, 1f));

		foreach(TimeLineEvent t in timelineEvents) {
			Destroy(t.marker);
		}

		timelineEvents = new List<TimeLineEvent>();
	}

	private IEnumerator TimelineAnimation() {
		distanceProgressBar.fillAmount = 0;
		distanceLabel.text = "0M";

		CreateEventMarkers();
		yield return new WaitForSeconds(initialWaitTime);

		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / timelineAnimationTime) {
			for(int i = 0; i < timelineEvents.Count; i++) {
				if(timelineEvents[i].timelinePlace <= f) {
					timelineEvents[i].Reveal();
				}
			}
			distanceProgressBar.fillAmount = f;
			distanceLabel.text = ((int) (f * distance)).ToString() + "M";
			yield return new WaitForEndOfFrame();
		}

		TweenCanvasAlpha.Show(new TweenParameters(tapToContinue, 0f, 1f, 1f, 0f));
		manager.MakeInteractable(tapToContinue);


	}

	private void CreateEventMarkers() {
		for(int i = 0; i < timelineEvents.Count; i++) {
			GameObject g = Instantiate(regionMarkerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			g.transform.parent = distanceGameOverPanel.transform;
			Debug.Log((600f * timelineEvents[i].timelinePlace));
			g.transform.localPosition = new Vector3(-300f + (600f * timelineEvents[i].timelinePlace), -65, 0);
			g.transform.localScale = new Vector3(1,1,1);
			g.GetComponent<RevealEvent>().SetDesc(timelineEvents[i].name, timelineEvents[i].distance);
			timelineEvents[i].marker = g;
		}

		lastRunMarker.GetComponent<RevealEvent>().SetDesc("X\nLast Run", PlayerPrefs.GetInt("last_run_distance"));
		if(PlayerPrefs.GetInt("last_run_distance") > 1000) lastRunMarker.transform.localPosition = new Vector3(-300 + ((PlayerPrefs.GetInt("last_run_distance") * 1.0f / distance) * 600), -15, 0);
		else lastRunMarker.GetComponent<Text>().color = new Color(0,0,0,0);
	}

	private void ComputeEventsPlace() {
		for(int i = 0; i < timelineEvents.Count; i++) {
			timelineEvents[i].timelinePlace = (float) 1.0f * (1.0f * timelineEvents[i].distance) / distance; 
			Debug.Log(timelineEvents[i].distance + " / " + distance + " = " + timelineEvents[i].timelinePlace);
		}
	}
}
