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
		if(Input.GetKeyUp(KeyCode.UpArrow)) NextRegion();
		if(Input.GetKeyUp(KeyCode.DownArrow)) PreviousRegion();
	}

	public void NextRegion() {
		if(currentRegion < regions.Count-1) {
			currentRegion++;
			themeManager.LerpTo(currentRegion);
			StartCoroutine(TweenSpeed());
		}
	}

	public void PreviousRegion() {
		if(currentRegion > 0) {
			currentRegion--;
			themeManager.LerpTo(currentRegion);
			StartCoroutine(TweenSpeed());
		}
	}

	private IEnumerator TweenSpeed() {
		float deltaDistance = (regions[currentRegion]._t-checkpointPosAddition) - myPos;
		while( Mathf.Abs(deltaDistance) > maxGap ) {
			deltaDistance = (regions[currentRegion]._t-checkpointPosAddition) - myPos;
			m.menuFwdSpeed = movementSpeed * deltaDistance;
			yield return new WaitForEndOfFrame();
		}
		m.menuFwdSpeed = 0.0025f;

	}
}
