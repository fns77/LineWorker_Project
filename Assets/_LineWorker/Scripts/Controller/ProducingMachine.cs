using System.Collections;
using System.Collections.Generic;
using _LineWorker;
using UnityEngine;

public class ProducingMachine : MonoBehaviour {

    public enum QualifiedRuleEnum {
        MissingOrnament
    }

    //This class will using factory and decorator design pattern
    [Header ("Object references")]
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject[] productPrefab;
    [SerializeField] private float timeSpawnInterval = 2f; // The time between two product creation

    [Header ("Values configuration")]
    [Range (0, 1)]
    [SerializeField] private float qualifiedProductRatio = 0.5f;
    [SerializeField] private QualifiedRuleEnum qualifiedProductRule = QualifiedRuleEnum.MissingOrnament;
    [SerializeField] private int poolSize = 10;

    private Dictionary<int, QualifiedProductRule> productQualifiedRules = new Dictionary<int, QualifiedProductRule> ();
    private Queue<GameObject>[] recycleProductQueue;
    private Vector3 spawnPosition;
    private float curTime = 0;

    private void Awake () {
        productQualifiedRules.Add ((int) QualifiedRuleEnum.MissingOrnament, new MissingOrnaments ());
        recycleProductQueue = new Queue<GameObject>[productPrefab.Length];
        for (int i = 0; i < recycleProductQueue.Length; i++) {
            recycleProductQueue[i] = new Queue<GameObject> ();
        }

    }

    private void Start () {
        curTime = timeSpawnInterval / FactoryProvider.Instance.ProducingSpeed;
        spawnPosition = spawnTransform.position;
        spawnPosition.y = FactoryProvider.Instance.conveyor.transform.position.y + FactoryProvider.Instance.conveyor.transform.localScale.y / 2;
        InitProductPool (CharacterManager.Instance.CurrentCharacterIndex);

    }

    private void Update () {
        if (GameManager.Instance.GameState != GameState.Playing)
            return;
        SpawnProducts ();
    }

    private void SpawnProducts () {
        curTime += Time.deltaTime;
        if (curTime >= timeSpawnInterval / FactoryProvider.Instance.ProducingSpeed) {
            GameObject productIns = SpawnProductFromPool (spawnPosition, Quaternion.identity);
            //GameObject productIns = Instantiate(productPrefab, spawnPosition, Quaternion.identity);

            var productController = productIns.GetComponent<ProductController> ();
            //Check for qualified or unqualified product
            if (Random.Range (0, 101) * 1f / 100 < (1 - qualifiedProductRatio)) {
                //Apply the unqualified product rule
                productQualifiedRules[(int) qualifiedProductRule].ApplyToProduct (productIns);
                productController.IsQualified = false;

            } else {
                productController.IsQualified = true;
            }

            FactoryProvider.Instance.AddToReadyToClassifiedProductQueue (productController);
            curTime = 0;
        }
    }

    private GameObject SpawnProductFromPool (Vector3 position, Quaternion rotation, Transform parent = null) {
        GameObject productFromPool = recycleProductQueue[CharacterManager.Instance.CurrentCharacterIndex].Dequeue ();
        productFromPool.transform.position = position;
        productFromPool.transform.rotation = rotation;
        productFromPool.transform.SetParent (parent);
        productFromPool.SetActive (true);

        return productFromPool;

    }

    public void InitProductPool (int CharacterIndex) {
        for (int i = 0; i < poolSize; i++) {
            //Spawn the normal product
            GameObject productIns = Instantiate (productPrefab[CharacterIndex], spawnPosition, Quaternion.identity);
            productIns.SetActive (false);
            recycleProductQueue[CharacterIndex].Enqueue (productIns);
        }
    }

    public void RecycleProduct (GameObject product) {
        product.SetActive (false);
        product.GetComponent<Rigidbody> ().isKinematic = true;
        recycleProductQueue[CharacterManager.Instance.CurrentCharacterIndex].Enqueue (product);
    }
}