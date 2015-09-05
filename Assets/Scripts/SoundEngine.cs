using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEngine : MonoBehaviour {

	public Stack<SoundEngineChild> available;
	public float fadeInTime;
	public float fadeOutTime;
	public float crossfadeTime;
	public AudioClip[] clips;
	public AudioClip tapSound;
	public AudioClip fastPassSound;

	public AnimationCurve forwardPitchChangeCurve;
	public AnimationCurve previousPitchChangeCurve;
	public float pitchChangeTime;

	private static SoundEngine _instance;
	private AudioSource source;

	public static SoundEngine Instance {
		get { return _instance; }
	}

	private void Awake() {
		_instance = this;
		source = this.GetComponent<AudioSource>();
		source.volume = 0;
		available = new Stack<SoundEngineChild>();

		foreach(Transform t in transform) {
			available.Push(t.GetComponent<SoundEngineChild>());
		}

		Debug.Log("SoundEngine available executors count: "+available.Count);
		StartCoroutine("MusicFadeIn");
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

	public void MakeTapSound() {

	}

	public void NextRegionSound() {
		StartCoroutine("NextRegionCoroutine");
	}

	public void PrevRegionSound() {
		StartCoroutine("PrevRegionCoroutine");
	}

	private IEnumerator NextRegionCoroutine() {
		AudioLowPassFilter filter = this.GetComponent<AudioLowPassFilter>();
		filter.enabled = true;
		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / pitchChangeTime) {
			filter.cutoffFrequency = forwardPitchChangeCurve.Evaluate(f) * 5000;
			yield return new WaitForEndOfFrame();
		}
		filter.enabled = false;
	}

	private IEnumerator PrevRegionCoroutine() {
		AudioHighPassFilter filter = this.GetComponent<AudioHighPassFilter>();
		filter.enabled = true;
		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / pitchChangeTime) {
			filter.cutoffFrequency = previousPitchChangeCurve.Evaluate(f) * 100;
			yield return new WaitForEndOfFrame();
		}
		source.pitch = 1;
		filter.enabled = false;
	}


	public void ReturnToStack(SoundEngineChild child) {
		available.Push(child);
	}

	public void ChangeSoundtrack(AudioClip clip) {
		source.volume = 0;
		source.clip = clip;
		source.Play();
	}

	public void StartSoundtrack(int themeNumber) {
		StartCoroutine("SoundtrackCoroutine", themeNumber);
	}

	public void StopSoundtrack() {
		StopCoroutine("SoundtrackCoroutine");
		source.Stop();
	}

	public void ChangeSoundtrackPitch(float pitch) {
		source.pitch = pitch;
	}

	IEnumerator SoundtrackCoroutine(int themeNumber) {
		MusicFadeOut();
		yield return new WaitForSeconds(fadeOutTime);
		MusicFadeIn();
		for(int i = themeNumber; i < 12; i++) {
			source.clip = clips[i];
			source.Play();
			Debug.Log("Playing: "+clips[i].name);
			yield return new WaitForSeconds(clips[i].length);
		}
	}

	public void MusicFadeIn() {
		StopCoroutine("MusicFadeInCoroutine");
		StopCoroutine("MusicFadeOutCoroutine");
		StopCoroutine("CrossFadeCoroutine");
		
		StartCoroutine("MusicFadeInCoroutine");
	}

	public void MusicFadeOut() {
		StopCoroutine("MusicFadeInCoroutine");
		StopCoroutine("MusicFadeOutCoroutine");
		StopCoroutine("CrossFadeCoroutine");
		
		StartCoroutine("MusicFadeOutCoroutine");
	}

	public void MusicCrossfade() {
		StopCoroutine("MusicFadeInCoroutine");
		StopCoroutine("MusicFadeOutCoroutine");
		StopCoroutine("CrossFadeCoroutine");
		
		StartCoroutine("CrossFadeCoroutine");
	}

	private IEnumerator MusicFadeInCoroutine() {
		source.volume = 0;
		source.Play();
		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeInTime) {
			source.volume = f;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator MusicFadeOutCoroutine() {
		for(float f = 1.0f; f > 0.0f; f -= Time.deltaTime / fadeOutTime) {
			source.volume = f;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator CrossFadeCoroutine() {
		for(float f = 1.0f; f > 0.0f; f -= Time.deltaTime / crossfadeTime) {
			source.volume = f;
			yield return new WaitForEndOfFrame();
		}

		source.clip = ThemeManager.Instance.GetCurrentTheme().soundtrack;
		source.Play();

		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / crossfadeTime) {
			source.volume = f;
			yield return new WaitForEndOfFrame();
		}
	}
}
