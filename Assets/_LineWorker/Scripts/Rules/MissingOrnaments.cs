using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingOrnaments : QualifiedProductRule
{

    public override void ApplyToProduct(GameObject product)
    {
        base.ApplyToProduct(product);
        //Debug.Log("Applying to: " + product.GetInstanceID());
        Transform productTrans = product.transform;

        //Find all the ornaments of this product
        for (int i = 0; i < productTrans.childCount; i++)
        {
            var child = productTrans.GetChild(i).gameObject;
            if (!child.CompareTag("MainModel"))
            {
                ornaments.Add(child);
                appliedIndex.Add(i);
            }
        }

        //Get the random of missing ornaments
        int numberOfApply = Random.Range(1, ornaments.Count);
        for (int i = 0; i < numberOfApply;i++)
        {
            int randomIndex = Random.Range(0, appliedIndex.Count);
            ornaments[randomIndex].SetActive(false);
            appliedIndex.Remove(randomIndex);
        }

    }
}
