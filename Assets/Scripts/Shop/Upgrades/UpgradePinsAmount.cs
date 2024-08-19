using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePinsAmount : UpgradableItem
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private PinsGenerator pinsGenerator;

    public override void Upgrade(int level)
    {
        pinsGenerator.Clear();
        for (int i = 0; i < positions.Length; i++)
        {
            if(i <= level)
            {
                pinsGenerator.Generate(4, positions[i].position);
            }
        }
    }

}
