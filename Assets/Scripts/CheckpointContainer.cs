using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckpointContainer : MonoBehaviour {

	public Text regionName;
	public Text regionIndex;
	public RegionSelector selector;

	public string name;
	public int index;
	public float offset;
	public float delay;

	public void Create (string name, int index, float o, float d) {
		this.name = name;
		this.index = index;
		this.offset = o;
		this.delay = d;

		CatmullRomMovement m = GetComponent<CatmullRomMovement>();
		m.startDelay = d;
		m.startOffset = o;
		m.DelayedStart();
		regionName.text = name;
		regionIndex.text = index+"/15";

		selector.regions.Add(m);
	}
}
