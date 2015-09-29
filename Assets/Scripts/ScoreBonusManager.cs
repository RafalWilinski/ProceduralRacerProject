using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreBonusManager : MonoBehaviour {

	public GameObject scoreBonusPrefab;
	public Transform parent;
	public Vector3 offset;
	public float spacing;
	public float dieTime;

	public List<ScoreBonusObject> bonuses;

	private static ScoreBonusManager _instance;
	public static ScoreBonusManager Instance {
		get {
			return _instance;
		}
	}



	public void AddScore(int score, string cause) {
		GameObject g = Instantiate(scoreBonusPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		g.transform.parent = parent;
		g.transform.localRotation = Quaternion.identity;
		g.transform.localPosition = offset + new Vector3(0f, spacing * 1.33f * bonuses.Count, 0f);
		g.transform.localScale = new Vector3(1,1,1);
		ScoreBonusObject s = g.AddComponent<ScoreBonusObject>() as ScoreBonusObject;
		s.Assign(score, cause);
		bonuses.Add(s);
	}

	public void SiftDown() {
		bonuses.RemoveAt(0);
		foreach(ScoreBonusObject bonus in bonuses) {
			Debug.Log("Flying to: "+ (offset + new Vector3(0, spacing, 0) * bonuses.IndexOf(bonus) ) );
			LeanTween.moveY(bonus.gameObject, 40f + spacing/3 * bonuses.IndexOf(bonus), 0.2f).setEase( LeanTweenType.easeInQuad );
		}
	}

	private void Awake() {
		_instance = this;
		// StartCoroutine("Demo");
	}

	IEnumerator Demo() {
		while(true) {
			AddScore(100, "xDD");
			yield return new WaitForSeconds(Random.Range(0.2f, 0.9f));
		}
	}
}
