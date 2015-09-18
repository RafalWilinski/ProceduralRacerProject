using UnityEngine;
using System.Collections;

public class SublimeCrashModule : MonoBehaviour {

	public ContinousMovement vehicle;
	public Transform vehicleTransform;
	public CatmullRomSpline spline;
	public ThemeManager themes;
	public float damageDistance;
	public int consecFramesToDeath;

	private int consecDamage;

	void Update () {
		if(themes.currentThemeIndex == 5 && !vehicle.isGameOver) {
			float dist = spline.NearestDistanceToSpline(vehicleTransform.position);
			if(dist > damageDistance) {
				Debug.Log("Distance: "+dist);
				consecDamage++;
				if(consecDamage >= consecFramesToDeath) {
					vehicle.OnCollision(vehicleTransform.position + Vector3.forward);
				}
			}
			else {
				consecDamage = 0;
			}
		}
	}
}
