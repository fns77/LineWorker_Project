using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualifiedProductRule
{

    protected int numberOfAppliedOrnament;
    protected List<GameObject> ornaments = new List<GameObject>();
    protected List<int> appliedIndex = new List<int>();
    public virtual void ApplyToProduct(GameObject product)
    {
        ornaments.Clear();
        appliedIndex.Clear();
    }

}
