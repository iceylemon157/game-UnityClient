using System;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateTaken;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float _spawnPlateTimer;
    private int _plateSpawnedCount;

    private const float SpawnPlateTime = 4f;
    private const int MaxPlateCount = 4;

    private void Update() {
        if (_plateSpawnedCount >= MaxPlateCount) return;
        _spawnPlateTimer += Time.deltaTime;
        if (_spawnPlateTimer >= SpawnPlateTime) {
            _spawnPlateTimer = 0;
            _plateSpawnedCount ++;
            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Interact(Player player) {
        if (player.HasKitchenObject()) return;
        if (_plateSpawnedCount <= 0) return;
        _plateSpawnedCount --;
        OnPlateTaken?.Invoke(this, EventArgs.Empty);
        KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
    }
}