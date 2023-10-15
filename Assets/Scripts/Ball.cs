using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private PCInputService _gameInput;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float force;
    [SerializeField] private float smoothX;
    // Start is called before the first frame update
    void Start()
    {
        _gameInput.InputDirection += MoveBall;
    }

    private void MoveBall(Vector2 inputDirection)
    {
        Vector3 ballDirection = new Vector3(inputDirection.x, transform.position.y, inputDirection.y);
        Vector3 velocity = force* ballDirection;
        if (velocity.z > 8)
            velocity.z = 8;
        velocity.x *= smoothX;
        _rigidbody.velocity = velocity;
    }
}
