using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    public event UnityAction BallMoved;
    [SerializeField] private PCInputService _gameInput;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float force;
    [SerializeField] private float smoothX;
    [SerializeField] private float maxVelocity = 8;

    private bool _isCanMove = true;
    void Start()
    {
        _gameInput.InputDirection += MoveBall;
    }
    public void SetForce(float force)
    {
        this.force = force;
    }

    private void MoveBall(Vector2 inputDirection)
    {
        if (!_isCanMove)
        {
            return;
        }
        BallMoved?.Invoke();
        _isCanMove = false;
        Vector3 ballDirection = new Vector3(inputDirection.x, transform.position.y, inputDirection.y);
        Vector3 velocity = force* ballDirection;
        if (velocity.z > maxVelocity)
            velocity.z = maxVelocity;
        velocity.x *= smoothX;
        _rigidbody.velocity = velocity;
    }
    public void MultiplyVelocity(float multiplier)
    {
        //if (_rigidbody.velocity.z+ multiplier <0)
        //{
        //    multiplier = -_rigidbody.velocity.z + 0.01f;
        //}
        //float xVelocity = (_rigidbody.velocity.z/multiplier)* _rigidbody.velocity.x;
        //_rigidbody.velocity += new Vector3(xVelocity, 0, multiplier);
    }
}
