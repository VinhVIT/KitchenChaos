using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter plateCounter;
    [SerializeField] private Transform counterSpawnPoint;
    [SerializeField] private Transform platePrefab;
    private List<GameObject> platePrefabList;
    private void Awake()
    {
        platePrefabList = new List<GameObject>();
    }
    private void Start()
    {
        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateRemoved(object sender, EventArgs e)
    {
        GameObject plateGameObject = platePrefabList[platePrefabList.Count -1];
        platePrefabList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }   

    private void PlateCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        Transform plateTransform = Instantiate(platePrefab, counterSpawnPoint);
        float plateOffsetY = .1f;
        plateTransform.localPosition = new Vector3(0,plateOffsetY * platePrefabList.Count,0);
        platePrefabList.Add(plateTransform.gameObject);
    }
}
