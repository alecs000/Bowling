using UnityEngine;

public class PinsGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float offset;
    [SerializeField] private Pin prefab;
    [SerializeField] private int pinsCount;
    private PoolMono<Pin> poolMono;
    private void Start()
    {
        poolMono = new PoolMono<Pin>(prefab, pinsCount);
        Generate(4);
    }
    public void Generate(int rowsAmount)
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
                Vector3 pinPositioin = new Vector3(startPosition.x + currentOffset * xMultiply, startPosition.y, startPosition.z + offset * i);
                pin.transform.position = pinPositioin;
            }
        }
    }
}
