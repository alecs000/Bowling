

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTimeZone : MonoBehaviour
{
    [SerializeField] private float reduceBy;
    [SerializeField] private float seconds;

    private Ball _ball;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            _ball = ball;
            ball.MultiplyVelocity(-reduceBy);
            StartCoroutine(WaitTime());
        }
    }
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(seconds);
        _ball.MultiplyVelocity(reduceBy);
    }
}
