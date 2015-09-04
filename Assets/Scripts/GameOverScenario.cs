using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOverScenario : MonoBehaviour {

	public CanvasGroup gameoverPanel;
	public CanvasGroup distanceGameOverPanel;
	public CanvasGroup tournamentGameOverPanel;

	public List<TimeLineEvent> timelineEvents;
	public GameObject regionMarkerPrefab;
	public GameObject lastRunMarker;
	public Image distanceProgressBar;
	public Text distanceLabel;
	public Text bestDistanceLabel;

	public PanelsManager manager;

	public float initialWaitTime = 1.0f;
	public float timelineAnimationTime = 2.5f;

	private int distance;

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
			if(!isRevealed) {
				isRevealed = true;
				marker.SendMessage("Reveal");
			}
		}
	}

	public int DistanceTravelled {
		set {
			distance = value;
		}
	}

	private void Awake() {
		timelineEvents = new List<TimeLineEvent>();
	}

	public void StartScenario() {
		manager.ShowCanvasImmediately(distanceGameOverPanel);
		StartCoroutine("TimelineAnimation");

		bestDistanceLabel.text = PlayerPrefs.GetInt("best_distance").ToString();
		ComputeEventsPlace();
	}
	
	public void AddEvent(string name, int distance) {
		timelineEvents.Add(new TimeLineEvent(name, distance));
	}

	private IEnumerator TimelineAnimation() {
		CreateEventMarkers();
		yield return new WaitForSeconds(initialWaitTime);

		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / timelineAnimationTime) {
			for(int i = 0; i < timelineEvents.Count; i++) {
				if(timelineEvents[i].timelinePlace >= f) {
					timelineEvents[i].Reveal();
				}
			}
			distanceProgressBar.fillAmount = f;
			distanceLabel.text = ((int) (f * distance)).ToString() + "M";
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}

	private void CreateEventMarkers() {
		for(int i = 0; i < timelineEvents.Count; i++) {
			GameObject g = Instantiate(regionMarkerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			g.transform.parent = distanceGameOverPanel.transform;
			g.transform.localPosition = new Vector3(-300 + (600f * timelineEvents[i].timelinePlace), -64, 0);
			g.transform.localScale = new Vector3(1,1,1);
			g.GetComponent<RevealEvent>().SetDesc(timelineEvents[i].name, timelineEvents[i].distance);
			timelineEvents[i].marker = g;
		}

		lastRunMarker.GetComponent<RevealEvent>().SetDesc("Last Run", PlayerPrefs.GetInt("last_run_distance"));
		lastRunMarker.transform.localPosition = new Vector3(-300 + ((PlayerPrefs.GetInt("last_run_distance") * 1.0f / distance) * 600), 0, 0);
	}

	private void ComputeEventsPlace() {
		for(int i = 0; i < timelineEvents.Count; i++) {
			timelineEvents[i].timelinePlace = (float) 1.0f * (1.0f * timelineEvents[i].distance) / distance; 
			Debug.Log(timelineEvents[i].distance + " / " + distance + " = " + timelineEvents[i].timelinePlace);
		}
	}
}
