using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _LineWorker;

public class FactoryProvider : MonoBehaviour
{
    public static System.Action<bool> OnClassifyProductCompleted = delegate { };

    public static FactoryProvider Instance = null;

    [Header("Value configurations")]
    [Range(1,6)]
    [SerializeField] private float producingSpeed = 1f;
    [SerializeField] private float offsetFromOutOfScreen = 2f;
    [SerializeField] private float increaseFactor = 2f;
    [SerializeField] private float increaseSpeed = 0.1f;
    [SerializeField] private float maxProducingSpeed = 2f;

    [SerializeField] private float unqualifiedProductForce = 3f;
  
    [Header("Object References")]
    public Camera gameCamera;
    public GameObject conveyor;
    [SerializeField] private GameObject classifiedArea;
    [SerializeField] private ProducingMachine producingMachine;

    #region Properties
    public float ProducingSpeed
    {
        get; private set;
    }
    public bool StopProducing
    {
        get
        {
            return isReadyToClassifiedProducts.Count == 0;
        }
    }
    #endregion

    //This is the queue of products when it has been produced
    private Queue<ProductController> isReadyToClassifiedProducts = new Queue<ProductController>();
    private List<Vector3> randomDirection = new List<Vector3>()
    {
        Vector3.left,
        Vector3.right,
        Vector3.forward,
        -Vector3.forward
    };


    private float startClassifiedAreaZ;
    private bool classifying;
    private float boostSpeed;
    #pragma warning disable 0414    
    private bool isSpeedUp = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            DestroyImmediate(gameObject);

        WorkerController.OnClassifiedProduct += WorkerController_OnClassifiedProduct;
    }

    private void OnDestroy()
    {
        WorkerController.OnClassifiedProduct -= WorkerController_OnClassifiedProduct;
    }

    private void Start()
    {
        startClassifiedAreaZ = classifiedArea.transform.position.z + classifiedArea.GetComponent<Collider>().bounds.extents.z;
        boostSpeed = producingSpeed * increaseFactor;
        ProducingSpeed = producingSpeed;
    }

    private void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
            return;

        //producingSpeed = Mathf.MoveTowards(producingSpeed, maxProducingSpeed, increaseSpeed * Time.deltaTime);
        //ProducingSpeed = isSpeedUp ? boostSpeed : producingSpeed;

        //Debug.Log("Producing speed:"  + ProducingSpeed);
    }

    public void RemoveFromReadyToClassfiedQueue()
    {
        #pragma warning disable 0219
        var product = isReadyToClassifiedProducts.Dequeue();
        //Debug.Log("Removing: " + product.gameObject.GetInstanceID());
    }

    public void ProductGoToTrash()
    {
        OnClassifyProductCompleted(false);
    }

    public void RecycleProduct(GameObject product)
    {
        producingMachine.RecycleProduct(product);
    }

    public void AddToReadyToClassifiedProductQueue(ProductController product)
    {
        isReadyToClassifiedProducts.Enqueue(product);
    }

    void WorkerController_OnClassifiedProduct(bool goLeft)
    {

        if (isReadyToClassifiedProducts.Count == 0)
            return;

        //Check if the factory is not classifying
        if (!classifying)
        {
            classifying = true;
            //get the product from queue
            var product = isReadyToClassifiedProducts.Dequeue();

            //Check if the product is in classified area
            //Debug.Log("Check and remove: " + product.gameObject.GetInstanceID());
            if (product.InClassifiedRange)
            {
                //Then snap to out of screen | classified side
                product.TranslateToOutOfScreen(goLeft, offsetFromOutOfScreen);
                if (goLeft)
                {
                    AddRandomForce(product);
                }
                classifying = false;
                ProducingSpeed = producingSpeed;
                CheckingProduct(goLeft,product);
            }
            else
            {
                //Increase the producing speed
                ProducingSpeed = boostSpeed;
                isSpeedUp = true;

                //Snap the product to the classfied side
                product.TranslateToClassifiedSide(goLeft, startClassifiedAreaZ, offsetFromOutOfScreen, () =>
                {
                    classifying = false;
                    ProducingSpeed = producingSpeed;
                    isSpeedUp = false;

                    //Add random force if product go left
                    if (goLeft)
                        AddRandomForce(product);
               
                    CheckingProduct(goLeft,product);
                });
            }

            //Incsease producing speed every time the player classified
            producingSpeed = Mathf.MoveTowards(producingSpeed, maxProducingSpeed, Time.deltaTime * increaseSpeed);
        }
    }

    private void AddRandomForce(ProductController product)
    {
        product.AddRandomForce(unqualifiedProductForce, randomDirection[Random.Range(0, randomDirection.Count)]);
    }

    private void CheckingProduct(bool goLeft, ProductController product)
    {
        bool isRightClassified = (goLeft && !product.IsQualified) || (!goLeft && product.IsQualified);
        OnClassifyProductCompleted(isRightClassified);
    }
    
    
}
