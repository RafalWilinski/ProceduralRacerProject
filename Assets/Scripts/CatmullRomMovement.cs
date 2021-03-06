﻿using System.Linq;
using UnityEngine;
using System.Collections;

public class CatmullRomMovement : MonoBehaviour {

    public Transform vehicle;
    public OpponentsPool pool;
    public float speed;
    public float startOffset;
    public float waitInterval;
    public CatmullRomSpline spline;
    public float _t;
    public float startDelay;
    public bool shouldntStartAuto;
    public Vector3 targetOffset;
    public Vector3 offsetLimits;
    public Vector3 offset;
    public float changesFrequency;
    public bool destroyOnReachingEnd;
    public bool isWorking;
    public bool useLerp = false;

    private float splineTimeLimit;
    private Transform myTransform;
    private bool isRunning;

    public enum LoopMode {
        ONCE, LOOP, PINGPONG
    }

    public LoopMode loopMode;

    void OnEnable() {
        CatmullRomSpline.OnSplineUpdated += OnLimitChanged;
    }

    void OnLimitChanged(float f) {
        splineTimeLimit = f;
    }

    void Start() {
        spline = (CatmullRomSpline)GameObject.Find("Root").GetComponent<CatmullRomSpline>();
        myTransform = transform;
        splineTimeLimit = spline.TimeLimit;
        _t = startOffset;
        if (!shouldntStartAuto) {
            StartCoroutine("ComputeOffset");
            StartCoroutine("LerpOffset");
            StartCoroutine("Movement");
        }
    }

    public void DelayedStart() {
        StopAllCoroutines();
        isWorking = true;
        _t = startOffset;
        StartCoroutine("Movement");
        StartCoroutine("ComputeOffset");
        StartCoroutine("LerpOffset");
    }

    void FixedUpdate() {
            if (spline.IsReady && isRunning) {
                if (loopMode == LoopMode.ONCE) {
                    _t += speed;
                }
                else if (loopMode == LoopMode.LOOP) {
                    if (_t >= splineTimeLimit) _t = 0f;
                    else _t += speed;
                }
                else if (loopMode == LoopMode.PINGPONG) {
                    if (_t >= splineTimeLimit || _t <= 0f) speed = -speed;
                    _t += speed;
                }

                if (_t > splineTimeLimit) {
                    _t = splineTimeLimit;
                    if (destroyOnReachingEnd && isWorking) {
                        //Debug.Log("Returning OP, reached end. Limit: " + splineTimeLimit);
                        pool.Return(GetComponent<Opponent>());
                        isWorking = false;
                    }
                }
                if (_t < 0) _t = 0f;

                if(useLerp) myTransform.position = Vector3.Lerp(myTransform.position, spline.GetPositionAtTime(_t) + offset, Time.deltaTime * 5);
                else myTransform.position = spline.GetPositionAtTime(_t) + offset;

                spline.GetRotAtTime(_t, this.gameObject);
            }
    }

    IEnumerator Movement() {

        yield return new WaitForSeconds(startDelay);
        isRunning = true;
        /* while (true) {
            if (spline.IsReady) {
                if (loopMode == LoopMode.ONCE) {
                    _t += speed;
                }
                else if (loopMode == LoopMode.LOOP) {
                    if (_t >= splineTimeLimit) _t = 0f;
                    else _t += speed;
                }
                else if (loopMode == LoopMode.PINGPONG) {
                    if (_t >= splineTimeLimit || _t <= 0f) speed = -speed;
                    _t += speed;
                }

                if (_t > splineTimeLimit) {
                    _t = splineTimeLimit;
                    if (destroyOnReachingEnd && isWorking) {
                        //Debug.Log("Returning OP, reached end. Limit: " + splineTimeLimit);
                        pool.Return(GetComponent<Opponent>());
                        trail.time = -1;
                        isWorking = false;
                    }
                }
                if (_t < 0) _t = 0f;

                myTransform.position = spline.GetPositionAtTime(_t) + offset;
                spline.GetRotAtTime(_t, this.gameObject);
            }
            yield return new WaitForSeconds(waitInterval);
        }
        * */
    }

    IEnumerator LerpOffset() {
        while (true) {
            offset = Vector3.Lerp(offset, targetOffset, Time.deltaTime * 0.5f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ComputeOffset() {
        if (changesFrequency > 0) {
            while (true) {
                targetOffset = new Vector3(Random.Range(-offsetLimits.x, offsetLimits.x),
                    Random.Range(-offsetLimits.y, offsetLimits.y), Random.Range(-offsetLimits.z, offsetLimits.z));
                if (destroyOnReachingEnd) {
                    if (myTransform.position.z + 1200 < vehicle.position.z && isWorking) {
                        //Debug.Log("Returning OP, left behind.");
                        pool.Return(GetComponent<Opponent>());
                        isWorking = false;
                    }
                }
                yield return new WaitForSeconds(changesFrequency);
            }
        }
    }
}





