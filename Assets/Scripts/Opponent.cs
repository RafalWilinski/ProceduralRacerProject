using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Opponent : MonoBehaviour {

	public CatmullRomSpline spline;
	private CatmullRomMovement mov;
    public Text textName;
    public Image picImage;
    private OpponentsHud hud;

    void Start() {
       // textName = gameObject.GetComponent<Text>();
       // picImage = gameObject.GetComponent<Image>();
        hud = (OpponentsHud) GameObject.Find("OpponentsHud").GetComponent<OpponentsHud>();
    }

	public void Create(Vector3 pos, float initSpeed, string name, Texture2D pic) {
	    //textName.text = name;
	    //picImage. = pic;
	    hud.SetHudNameAndPic(name, pic, this);
		mov = this.gameObject.GetComponent<CatmullRomMovement>();
		spline = (CatmullRomSpline) GameObject.Find("Root").GetComponent<CatmullRomSpline>();
		mov.startOffset = spline.GetClosestPointAtSpline(pos);	
		mov.speed = initSpeed;
        //Debug.Log("Spawning at position: " + mov.startOffset + ", with init speed: " + mov.speed);
		mov.DelayedStart();

	}
}
