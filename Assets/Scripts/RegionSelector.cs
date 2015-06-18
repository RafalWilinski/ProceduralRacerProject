using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegionSelector : MonoBehaviour {

	public ContinousMovement m;
	public ThemeManager themeManager;
	public List<CatmullRomMovement> regions;
	public int currentRegion;
	public float myPos;
	public float movementSpeed;
	public float maxGap;
	public float checkpointPosAddition;
	
	void Update () {
		myPos = m._t;
	}	

	public void NextRegion() {
		if(currentRegion < regions.Count-1) {
			currentRegion++;
			themeManager.FakeLerpTo(currentRegion);
			StartCoroutine(TweenSpeed());
		}
	}

	public void PreviousRegion() {
		if(currentRegion > 0) {
			currentRegion--;
			themeManager.FakeLerpTo(currentRegion);
			StartCoroutine(TweenSpeed());
		}
	}

	private IEnumerator TweenSpeed() {
		float deltaDistance = (regions[currentRegion]._t-checkpointPosAddition) - myPos;
		while( Mathf.Abs(deltaDistance) > maxGap ) {
			deltaDistance = (regions[currentRegion]._t-checkpointPosAddition) - myPos;
			m.menuFwdSpeed = movementSpeed * deltaDistance;
			yield return new WaitForEndOfFrame();
//			Debug.Log("TweenSpeed");
		}
		m.menuFwdSpeed = 0.0025f;
	}
}
