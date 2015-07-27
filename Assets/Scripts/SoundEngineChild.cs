using UnityEngine;
using System.Collections;

public class SoundEngineChild : MonoBehaviour {

	private Transform myTransform;
	private AudioSource src;
	private float length;

	private void Awake() {
		src = GetComponent<AudioSource>();
		myTransform = transform;
	}

	public void Play(AudioClip clip, float pitch = 1, float volume = 1, Vector3 position = default(Vector3), float length = 0) {
		myTransform.localPosition = position;
		src.clip = clip;
		src.pitch = pitch;
		src.volume = volume;
		this.length = length == 0 ? clip.length : length;
		gameObject.name = "Playing";
		
		src.Play();

		StartCoroutine("Recycle");
	}

	IEnumerator Recycle() {
		yield return new WaitForSeconds(length);
		SoundEngine.Instance.ReturnToStack(this);
		src.Stop();
		gameObject.name = "Available";
	}
}
