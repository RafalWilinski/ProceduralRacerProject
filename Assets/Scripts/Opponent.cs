using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Opponent : MonoBehaviour {

	public CatmullRomSpline spline;
	private CatmullRomMovement mov;
    private OpponentsHud hud;
    private Gradient gradient;
    private GradientColorKey gradientColor[];
    private GradientAlphaKey gradientAlpha[];

    void Start() {
        hud = (OpponentsHud) GameObject.Find("OpponentsHud").GetComponent<OpponentsHud>();
    }

	public void Create(Vector3 pos, float initSpeed, string name, Texture2D pic) {
	    hud.SetHudNameAndPic(name, pic, this);
		mov = this.gameObject.GetComponent<CatmullRomMovement>();
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		mov.startOffset = spline.GetClosestPointAtSpline(pos);	
		mov.speed = initSpeed;
		mov.DelayedStart();

	}
}
