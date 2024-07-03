using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider)), RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private DynamicJoystick _joystick;
    [SerializeField] private GameObject _playerModel;
    private Animator _animator;

    [SerializeField] private float _moveSpeed = 5f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = _playerModel.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = _joystick.Horizontal;
        float moveVertical = _joystick.Vertical;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        // Ensure the movement is relative to the player's rotation
        movement = transform.TransformDirection(movement);

        if (movement != Vector3.zero)
        {
            _rigidbody.MovePosition(transform.position + movement * _moveSpeed * Time.deltaTime);
            _playerModel.transform.rotation = Quaternion.LookRotation(movement);
        }

        // Update animation based on movement
        _animator.SetFloat("Speed_f", movement.magnitude);
    }
}