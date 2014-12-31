using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PigeonCoopToolkit.Effects.Trails;

public class Opponent : MonoBehaviour {

	public CatmullRomSpline spline;
	private CatmullRomMovement mov;
    private OpponentsHud hud;

    private SmoothTrail trail;
    public float hueStep;
    private Gradient gradient;
    private GradientColorKey[] gradientColor;
    private GradientAlphaKey[] gradientAlpha;

    void Start() {
        hud = (OpponentsHud) GameObject.Find("OpponentsHud").GetComponent<OpponentsHud>();
        trail = this.GetComponent<SmoothTrail>();

        gradient = new Gradient();
        gradientAlpha = new GradientAlphaKey[5];
        gradientColor = new GradientColorKey[5];
    }

	public void Create(Vector3 pos, float initSpeed, string name, Texture2D pic) {
		trail.Emit = false;
	    hud.SetHudNameAndPic(name, pic, this);
		mov = this.gameObject.GetComponent<CatmullRomMovement>();
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		mov.startOffset = spline.GetClosestPointAtSpline(pos);	
		mov.speed = initSpeed;
		mov.DelayedStart();
		trail.Emit = true;


		float hue = Random.Range(0.01f, 0.999f);
		for (int i = 0; i < 5; i++) {
			gradientColor[i].color = new HSBColor( (hue + (hueStep*i)) % 1f, 1, 1, 1 - (0.25f * i)).ToColor();
			gradientColor[i].time = i * 0.25f;

			gradientAlpha[i].alpha = 1 - (0.25f * i);
			gradientAlpha[i].time = i * 0.25f;
		}

		gradient.SetKeys(gradientColor,gradientAlpha);

		trail.TrailData.ColorOverLife = gradient;
	}
}
