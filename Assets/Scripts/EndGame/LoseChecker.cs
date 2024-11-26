using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoseChecker : MonoBehaviour
{
    public event UnityAction EndGame;
    [SerializeField] private Rigidbody rigidbodyPlayer;
    [SerializeField] private float amountForLose;
    [SerializeField] private int timeCheck = 1;
    [SerializeField] private int waitAfterEndGame = 3;
    [SerializeField] private Ball ball;
    private void Start()
    {
        ball.BallMoved += OnGameStarted;
    }
    public void OnGameStarted()
    {
        StartCoroutine(CheskLose());
    }
    IEnumerator CheskLose()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeCheck);
            if(rigidbodyPlayer.velocity.z< amountForLose&&!ball.IsCanMove)
            {
                yield return new WaitForSeconds(waitAfterEndGame);
                EndGame?.Invoke();
            }
        }
    }
}
