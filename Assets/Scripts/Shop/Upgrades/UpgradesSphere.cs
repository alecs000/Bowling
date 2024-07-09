using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesSphere : UpgradableItem
{
    [SerializeField] private float[] scales; 
    [SerializeField] private float[] masses; 
    [SerializeField] private Rigidbody rigidbodySphere; 

    public override void Upgrade(int level)
    {
        transform.localScale = new Vector3(scales[level], scales[level], scales[level]);
        rigidbodySphere.mass = masses[level];
    }

}
