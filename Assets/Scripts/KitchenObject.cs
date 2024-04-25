using System;
using UnityEngine;

public class KitchenObject : MonoBehaviour {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent _kitchenObjectParent;
    
    // For dropping the object, not used in real game
    private bool _isHeld;

    private void Start() {
        _isHeld = true;
    }

    private void Update() {
        if (_isHeld) return;
        const float moveSpeed = 1f;
        transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        if (transform.position.y >= 0) return;
        var transform1 = transform;
        var position = transform1.position;
        position = new Vector3(position.x, 0, position.z);
        transform1.position = position;
    }

    public string GetObjectName() {
        return kitchenObjectSO.objectName;
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void FallWithConstantSpeed() {
        transform.SetParent(null, true);
        _isHeld = false;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        _kitchenObjectParent?.ClearKitchenObject();

        _kitchenObjectParent = kitchenObjectParent;

        if (_kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("Error: IKitchenObjectParent is already occupied by " +
                           _kitchenObjectParent.GetKitchenObject().GetObjectName());
        }

        _kitchenObjectParent.SetKitchenObject(this);

        transform.parent = _kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return _kitchenObjectParent;
    }

    public void DestroySelf() {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent) {
        var kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        return kitchenObject;
    }

    public bool TryGetPlate(out PlateKitchenObject plate) {
        plate = null;
        if (this is not PlateKitchenObject plateKitchenObject) return false;
        plate = plateKitchenObject;
        return true;
    }
}