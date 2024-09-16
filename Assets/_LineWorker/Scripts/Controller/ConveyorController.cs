using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _LineWorker;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private bool controlByFactory;

    private MeshRenderer conveyorMeshRenderer;
    private float curY = 0;

    // Start is called before the first frame update
    void Start()
    {
        conveyorMeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FactoryProvider.Instance.StopProducing)
            return;

        curY -= Time.fixedDeltaTime * moveSpeed * (controlByFactory ? FactoryProvider.Instance.ProducingSpeed : 1);
        conveyorMeshRenderer.material.SetTextureOffset("_MainTex", new Vector2(0, curY));
    }
}
