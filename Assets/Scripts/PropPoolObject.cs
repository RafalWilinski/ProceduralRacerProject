﻿using UnityEngine;
using System.Collections;

public class PropPoolObject : MonoBehaviour {

    public float to_y;
    public float ySpeed;
    public Transform MyTransform;
    public Transform vehicle;
    public bool isUsed = false;
    public EventsManager manager;
    public Vector3 translationVector;

    private float checkingFrequency = 1f;
    private string cachedCoroutine;

    private void Awake() {
        MyTransform = transform;
        StartCoroutine("CheckIfUnused");
    }

    IEnumerator CheckIfUnused() {
        while (true) {
            if (isUsed && vehicle.transform.position.z > MyTransform.position.z) {
                manager.ReturnEventObject(gameObject);
                isUsed = false;
                StopCoroutine(cachedCoroutine);
            }
            yield return new WaitForSeconds(checkingFrequency);
        }
    }

    public void Create (Vector3 position, string extraCoroutine = "") {
        isUsed = true;
        MyTransform.position = position;
        if (extraCoroutine != "") {
            StartCoroutine(extraCoroutine);
            cachedCoroutine = extraCoroutine;
        }
    }

    private IEnumerator Rise() {
        while (to_y > MyTransform.position.y) {
            MyTransform.Translate(new Vector3(0, ySpeed, 0));
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator RandomizeYRotation() {
        MyTransform.eulerAngles = new Vector3(MyTransform.eulerAngles.x, Random.Range(0, 180), MyTransform.eulerAngles.z);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator RandomizeSize() {
        MyTransform.localScale = MyTransform.localScale * Random.Range(0.8f, 1.2f);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator AddVelocity() {
        while(true) {
            MyTransform.Translate(translationVector * Time.deltaTime, Space.World);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator GenerateCave() {
        this.gameObject.SendMessage("GetBoundaryVertices");
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator AnnounceRespawn() {
        this.gameObject.SendMessage("Replace");
        yield return new WaitForEndOfFrame();
    }
}
