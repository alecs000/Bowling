using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinsGenerator : MonoBehaviour
{
    public event UnityAction PinsGenerated;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float offset;
    [SerializeField] private Pin prefab;
    [SerializeField] private int pinsCount;
    private PoolMono<Pin> poolMono;
    private List<Pin> upgradedPins;
    public List<Pin> Pins => poolMono.pool;
    private void Start()
    {
        upgradedPins = new List<Pin>();
        poolMono = new PoolMono<Pin>(prefab, pinsCount);
        Generate(4, startPosition, true);
    }
    public void Generate(int rowsAmount, Vector3 position, bool isFirst = false)
    {

        for (int i = 0; i < rowsAmount; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                float currentOffset = offset;
                int xMultiply = 1;
                bool isCenterPin = j == (i + 1) / 2 || j == i / 2;
                if (i % 2 == 1)
                {
                    if (isCenterPin)
                    {
                        xMultiply = 1;
                        currentOffset = currentOffset / 2;
                    }
                    else
                    {
                        currentOffset = currentOffset+ currentOffset / 2;
                    }
                }
                else
                {
                    if (isCenterPin)
                    {
                        xMultiply = 0;
                    }
                }
                if (j < (i + 1) / 2)
                    currentOffset = -currentOffset;
                Pin pin = poolMono.GetElement();
                if (!isFirst)
                {
                    upgradedPins.Add(pin);
                }
                Vector3 pinPositioin = new Vector3(position.x + currentOffset * xMultiply, position.y, position.z + offset * i);
                pin.transform.position = pinPositioin;
            }
        }
        PinsGenerated?.Invoke();
    }
    public void Clear()
    {
        foreach (var pin in upgradedPins)
        {
            pin.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
