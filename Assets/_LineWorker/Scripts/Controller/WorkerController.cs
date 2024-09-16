using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _LineWorker;

public class WorkerController : MonoBehaviour
{
    
    public static System.Action<bool> OnClassifiedProduct;
    #pragma warning disable 0414
    [SerializeField] private float swipeThreshold = 1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private Camera gameCamera;

    private void Start()
    {
        gameCamera = FactoryProvider.Instance.gameCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
            return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    startPos = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, gameCamera.nearClipPlane));
        //}
        //if (Input.GetMouseButtonUp(0))
        //{
        //    endPos = gameCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, gameCamera.nearClipPlane));
        //    if ((startPos - endPos).sqrMagnitude >= swipeThreshold * swipeThreshold)
        //    {
        //        bool goLeft = (endPos.x - startPos.x) < 0;
        //        //.Log(goLeft ? "go left" : "go right");
        //        if (OnClassifiedProduct != null)
        //        {
        //            OnClassifiedProduct(goLeft);
        //        }
        //    }
        //}

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 viewportPos = gameCamera.ScreenToViewportPoint(Input.mousePosition);
            bool goLeft = viewportPos.x < 0.5f;
            if (OnClassifiedProduct != null)
                OnClassifiedProduct(goLeft);
        }
    }

}
