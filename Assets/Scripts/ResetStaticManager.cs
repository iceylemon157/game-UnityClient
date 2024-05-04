using UnityEngine;

public class ResetStaticManager : MonoBehaviour {
    public void Awake() {
        BaseCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }
}