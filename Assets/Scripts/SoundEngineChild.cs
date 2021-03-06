﻿using UnityEngine;
using System;
using System.Collections;

public class SoundEngineChild : MonoBehaviour {

	private Transform myTransform;
	private AudioSource src;
	private float length;
	private bool looping;

	public String ClipName {
		get {
			return src.clip.name;
		}
	}

	public bool Looping {
		get { return looping; }
		set {
			looping = value;

			if(src == null) src = GetComponent<AudioSource>();
			src.loop = value;
		}
	}

	private void Awake() {
		src = GetComponent<AudioSource>();
		myTransform = transform;
	}

	public void Play(AudioClip clip, float pitch = 1, float volume = 1, float length = 0) {
		try {
			if(myTransform != null && src != null) {
				myTransform.localPosition = Vector3.zero;
				src.clip = clip;
				src.pitch = pitch;
				src.volume = volume;
				this.length = length == 0 ? clip.length : length;
				gameObject.name = "Playing "+clip.name;
				
				src.Play();

				if(!looping) StartCoroutine("Recycle");
			}
		}
		catch(Exception e) {
			Debug.Log("SoundEngineChild.Play():: "+e.Message);
			Debug.Log(e.StackTrace);
		}
	}

	IEnumerator Recycle() {
		yield return new WaitForSeconds(length);
		SoundEngine.Instance.ReturnToStack(this);
		src.Stop();
		gameObject.name = "Available";
	}
}
