using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundEngine : MonoBehaviour {

	public Stack<SoundEngineChild> available;
	public List<SoundEngineChild> loopingSounds;
	public float fadeInTime;
	public float fadeOutTime;
	public float crossfadeTime;

	public AudioClip[] clips;
	public AudioClip menuTheme;
	public AudioClip tapSound;
	public AudioClip fastPassSound;
	public AudioClip hitSound;
	public AudioClip heartbeatSound;
	public AudioClip startGameSound;
	public AudioClip tickSound;

	public AnimationCurve forwardPitchChangeCurve;
	public AnimationCurve previousPitchChangeCurve;
	public float pitchChangeTime;

	private static SoundEngine _instance;
	private AudioSource source;

	public static SoundEngine Instance {
		get { 
			if(_instance == null) {
				Debug.Log("------ Instance unavailable! --------");
			} 
			return _instance;
		}
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
		StartCoroutine("MusicFadeIn", 2.0f);
	}

//default(Vector3)
	public void CreateSound(AudioClip clip, float pitch = 1f, float volume = 1f, float length = 0f) {
		SoundEngineChild executor;

		if(available.Count > 0) executor = available.Pop();
		else {
			GameObject g = new GameObject();
			g.name = "Playing "+(clip.name);
			g.transform.parent = this.transform;
			g.AddComponent<AudioSource>();
			g.AddComponent<SoundEngineChild>();
			executor = (SoundEngineChild) g.GetComponent<SoundEngineChild>() as SoundEngineChild;
			Debug.Log("There was insufficient amount of executors so new one was added.");
		}

		if(executor != null) {
			if(length == 0) length = clip.length;
			executor.Play(clip, pitch, volume, length);
		}
		else {
			Debug.Log("Executor is null somehow!");
		}
	}

	public void CreateLoopingSound(AudioClip clip, float pitch = 1f, float volume = 1f) {
		SoundEngineChild executor;

		if(available.Count > 0) executor = available.Pop();
		else {
			GameObject g = new GameObject();
			g.name = "Playing "+(clip.name);
			g.transform.parent = this.transform;
			g.AddComponent<AudioSource>();
			g.AddComponent<SoundEngineChild>();
			executor = (SoundEngineChild) g.GetComponent<SoundEngineChild>() as SoundEngineChild;
			Debug.Log("There was insufficient amount of executors so new one was added.");
		}

		if(executor != null) {
			executor.Looping = true;
			executor.Play(clip, pitch, volume);
			loopingSounds.Add(executor);
		}
		else {
			Debug.Log("Executor is null somehow!");
		}
	}

	public void RemoveLoopingSound(string tag) {
		for(int i = 0; i < loopingSounds.Count; i++) {
			SoundEngineChild s = loopingSounds[i];

			if(s.ClipName == tag) {
				Destroy(s.gameObject);
				loopingSounds.Remove(s);
			}
		}
	}

	public void MakeTapSound() {
		CreateSound(tapSound);
	}

	public void MakeHeartbeat() {
		CreateSound(heartbeatSound);
	}

	public void MakeHit() {
		CreateSound(hitSound, UnityEngine.Random.Range(0.92f, 1.12f), UnityEngine.Random.Range(0.92f, 1.12f), 1.2f);
	}

	public void MakeSwoosh(float distance, Vector3 offset) {
		CreateSound(fastPassSound, UnityEngine.Random.Range(0.9f, 1.1f), 1-(distance / 120));
	}

	public void MakeStartGameSound() {
		CreateSound(startGameSound);
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
		source.pitch = 1;
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
		MusicFadeIn(0f);
		for(int i = themeNumber; i < 12; i++) {
			source.clip = clips[i];
			source.Play();
			Debug.Log("Playing: "+clips[i].name);
			yield return new WaitForSeconds(clips[i].length);
		}
	}

	public void MusicFadeIn(float delay) {
		StopCoroutine("MusicFadeInCoroutine");
		StopCoroutine("MusicFadeOutCoroutine");
		StopCoroutine("CrossFadeCoroutine");
		
		StartCoroutine("MusicFadeInCoroutine", delay);
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

	private IEnumerator MusicFadeInCoroutine(float delay) {
		yield return new WaitForSeconds(delay);
		source.volume = 0;
		source.Play();
		for(float f = 0.0f; f < 1.0f; f += Time.deltaTime / fadeInTime) {
			source.volume = f;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator MusicFadeOutCoroutine() {
		for(float f = source.volume; f > 0.0f; f -= Time.deltaTime / fadeOutTime) {
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
