using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStrength : UpgradableItem
{
    [SerializeField] private Ball ball;
    [SerializeField] private float[] strengths;
    public override void Upgrade(int level)
    {
        ball.SetForce(strengths[level]);
    }
}
