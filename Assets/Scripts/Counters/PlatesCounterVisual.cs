using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {
    [SerializeField] private Transform counterTop;
    [SerializeField] private Transform platePrefab;
    [SerializeField] private PlatesCounter platesCounter;
    
    private List<Transform> _plateVisualTransformList;
    
    private const float PlateHeight = .1f;
    
    private void Awake() {
        _plateVisualTransformList = new List<Transform>();
    }
    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateTaken += PlatesCounter_OnPlateTaken;
    }

    private void PlatesCounter_OnPlateTaken(object sender, EventArgs e) {
        var plateTransform = _plateVisualTransformList[^1];
        _plateVisualTransformList.RemoveAt(_plateVisualTransformList.Count - 1);
        Destroy(plateTransform.gameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e) {
        var plateTransform = Instantiate(platePrefab, counterTop);
        plateTransform.localPosition = new Vector3(0, PlateHeight * _plateVisualTransformList.Count, 0);
        Debug.Log(plateTransform.localPosition);
        Debug.Log(_plateVisualTransformList.Count);
        _plateVisualTransformList.Add(plateTransform);
    }
}