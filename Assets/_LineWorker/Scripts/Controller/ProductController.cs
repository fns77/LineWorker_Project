using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _LineWorker;

[RequireComponent(typeof(Rigidbody))]
public class ProductController : MonoBehaviour
{

    [SerializeField] private float unitMoveSpeed = 4f;
    [SerializeField] private float translateSpeed = 0.3f;
    [SerializeField] private float epsilon = 0.0001f;

    public bool InClassifiedRange;
    public bool IsQualified = true;

    #region Cache values
    private Rigidbody productRb;
    private Camera gameplayCamera;

    private bool startTranslateSequence = false;

    #endregion

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        if (productRb != null)
            productRb.isKinematic = false;
        startTranslateSequence = false;
        InClassifiedRange = false;

    }

    private void OnDisable()
    {
        if (productRb != null)
            productRb.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        productRb = GetComponent<Rigidbody>();
        gameplayCamera = FactoryProvider.Instance.gameCamera;
    }

    private void FixedUpdate()
    {
        if (!startTranslateSequence)
        {
            Vector3 newPosition = productRb.position - Vector3.forward * unitMoveSpeed * FactoryProvider.Instance.ProducingSpeed * Time.deltaTime;
            productRb.MovePosition(newPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ClassifiedArea"))
        {
            InClassifiedRange = true;
        }
        else if (other.CompareTag("Recycling"))
        {
            //Destroy(gameObject, 0.5f);
            FactoryProvider.Instance.RecycleProduct(gameObject);
        }
        else if (other.CompareTag("Trash"))
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                FactoryProvider.Instance.ProductGoToTrash();
            }
            // Destroy(gameObject, 0.5f);
            FactoryProvider.Instance.RecycleProduct(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ClassifiedArea"))
        {
            if (!startTranslateSequence)
            {
                FactoryProvider.Instance.RemoveFromReadyToClassfiedQueue();
            }

        }
    }

    Coroutine classifiedSequenceCR;
    public void TranslateToClassifiedSide(bool goLeft,float startClassifiedAreaZ, float offsetFromOutOfScreen, System.Action completed = null)
    {
        startTranslateSequence = true;
        if (classifiedSequenceCR != null)
            StopCoroutine(classifiedSequenceCR);

        classifiedSequenceCR = StartCoroutine(CR_TranslateToClassifiedSide(goLeft, startClassifiedAreaZ,offsetFromOutOfScreen, completed));
    }

    public void AddRandomForce(float force, Vector3 direction)
    {
        productRb.AddTorque(direction * force, ForceMode.Impulse);
    }

    private IEnumerator CR_TranslateToClassifiedSide(bool goLeft, float startClassifiedAreaZ,float offsetFromOutOfScreen, System.Action completed = null)
    {
        //Snap to the start classified area position;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position;
        endPos.z = startClassifiedAreaZ;
        float relativeDistance = (endPos - startPos).sqrMagnitude;
        while ( relativeDistance > epsilon)
        {
            //Debug.Log(name + ": " + relativeDistance);
            Vector3 newPosition = transform.position - Vector3.forward * unitMoveSpeed * FactoryProvider.Instance.ProducingSpeed * Time.fixedDeltaTime;
            productRb.MovePosition(newPosition);
            relativeDistance = (endPos - transform.position).sqrMagnitude;
            yield return null;
        }
        if (completed != null)
            completed();

        //Debug.Log("completed 1");
        //Snap to the classified side => out of bound left or right of the screenend
        endPos.x += (goLeft ? -1 : 1) * (gameplayCamera.ViewportToWorldPoint(new Vector3(0, 0, gameplayCamera.nearClipPlane)).x + offsetFromOutOfScreen);
        startPos = transform.position;

        relativeDistance = (endPos - startPos).sqrMagnitude;
        
        float value = 0;
        while (value < 1)
        {
            value += Time.deltaTime * translateSpeed;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, value);
            productRb.MovePosition(newPos);
            yield return null;
        }
    }
   
    Coroutine translateToOutOfScreenCR;
    public void TranslateToOutOfScreen(bool goLeft, float offsetFromOutOfScreen)
    {
        if (translateToOutOfScreenCR != null)
            StopCoroutine(translateToOutOfScreenCR);

        translateToOutOfScreenCR = StartCoroutine(CR_TranslateToOutOfScreen(goLeft, offsetFromOutOfScreen));
    }
    private IEnumerator CR_TranslateToOutOfScreen(bool goLeft, float offsetFromOutOfScreen)
    {
        startTranslateSequence = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position;
        endPos.x += (goLeft ? -1 : 1) * (gameplayCamera.ViewportToWorldPoint(new Vector3(0, 0, gameplayCamera.nearClipPlane)).x + offsetFromOutOfScreen);
        float value = 0;
        while (value < 1)
        {
            value += Time.deltaTime * translateSpeed;
            Vector3 newPos = Vector3.Lerp(startPos, endPos, value);
            productRb.MovePosition(newPos);
            yield return null;
        }
    }

}
