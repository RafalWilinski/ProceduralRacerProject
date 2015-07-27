using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEngine : MonoBehaviour {

	public Stack<SoundEngineChild> available;

	private static SoundEngine _instance;

	public static SoundEngine Instance {
		get { return _instance; }
	}

	private void Awake() {
		_instance = this;
		available = new Stack<SoundEngineChild>();

		foreach(Transform t in transform) {
			available.Push(t.GetComponent<SoundEngineChild>());
		}

		Debug.Log("SoundEngine available executors count: "+available.Count);
	}

	public void CreateSound(AudioClip clip, float pitch = 1, float volume = 1, Vector3 pos = default(Vector3), float length = 0) {

		SoundEngineChild executor;

		if(available.Count > 0) executor = available.Pop();
		else {
			GameObject g = new GameObject();
			g.name = "Playing";
			g.transform.parent = this.transform;
			g.AddComponent<AudioSource>();
			g.AddComponent<SoundEngineChild>();
			executor = g.GetComponent<SoundEngineChild>();
		}

		if(executor != null) {
			if(length == 0) length = clip.length;
			executor.Play(clip, pitch, volume, pos, length);
		}
	}

	public void ReturnToStack(SoundEngineChild child) {
		available.Push(child);
	}
}
