using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Targets : MonoBehaviour
{
    private Transform _selfTrans;
    public Transform _centerTrans;
    private Vector3 _centerPos;

    [SerializeField] private float _minDistToCenter;
    [SerializeField] private float _maxDistToCenter;
    [SerializeField, Range(0.1f, 15.0f)] private float _speed;
    private Vector3 _RotAxis;
    [SerializeField] private float _distTotarget;

    [Serializable]
    public struct BoidData
    {
        public Vector3 position;
        public int ToRender;
    }

    public BoidData boidData;

    // bool hasBeenInit = false;

    public void Init()
    {
        _selfTrans = transform;
        _centerPos = _centerTrans.position;
        _RotAxis = new Vector3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2)).normalized;



        _distTotarget = (UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(_minDistToCenter, _maxDistToCenter)).magnitude;
        _selfTrans.position = _centerPos + UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(_minDistToCenter, _maxDistToCenter);

        boidData.position = _selfTrans.position;
        boidData.ToRender = UnityEngine.Random.Range(0, 2);

        if (boidData.ToRender == 0) gameObject.SetActive(false);
        // else
        //     hasBeenInit = true;
    }

    void Update()
    {
        // if (!hasBeenInit) return;
        _selfTrans.RotateAround(_centerPos, _RotAxis, 15 * _speed * Time.deltaTime);
        boidData.position = _selfTrans.position;
    }
}
