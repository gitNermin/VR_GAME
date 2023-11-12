using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [SerializeField] private InputActionReference _movementLeft;
    [SerializeField] private InputActionReference _movementRight;
    [SerializeField] private InputActionReference _accelerateLeft;
    [SerializeField] private InputActionReference _accelerateRight;
    
    [SerializeField]float _movementSpeed = 5f;
    [SerializeField]float _acceleratedSpeed = 10f;      
    [SerializeField]float _rotationSpeed = 10f;

    private Rigidbody _characterController;
    private bool _isAccelerated;

    private Vector3 _movement = Vector3.zero;//Vector3.down;

    

    private void Start()
    {
        _characterController = GetComponent<Rigidbody>();

        _movementLeft.action.performed += OnMovement;
        _movementLeft.action.canceled += OnMovementCanceled;
        _movementRight.action.performed += OnMovement;
        _movementRight.action.canceled += OnMovementCanceled;

        _accelerateLeft.action.performed += OnAccelerate;
        _accelerateLeft.action.canceled += OnCancelAcceleration;

        _accelerateRight.action.performed += OnAccelerate;
        _accelerateRight.action.canceled += OnCancelAcceleration;
        
        RemoteControl.OnEnableCarControls += OnEnableCarControls;
        
        EnableLeftHand(false);
        EnableRightHand(false);
    }

    private void OnEnableCarControls(RemoteControl.Hand hand, bool isEnabled)
    {
        if (hand == RemoteControl.Hand.Left)
        {
            EnableLeftHand(isEnabled);
        }
        else
        {
            Debug.Log($"enable remote control {isEnabled}");
            EnableRightHand(isEnabled);
        }
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        _movement = Vector3.zero;
    }

    private void OnAccelerate(InputAction.CallbackContext ctx)
    {
        _isAccelerated = true;
    }
    private void OnCancelAcceleration(InputAction.CallbackContext ctx)
    {
        _isAccelerated = false;
    }

    private void EnableLeftHand(bool isEnabled)
    {
        if (isEnabled)
        {
            _movementLeft.action.Enable();
            _accelerateLeft.action.Enable();
        }
        else
        {
            _movementLeft.action.Disable();
            _accelerateLeft.action.Disable();
        }
    }

    private void EnableRightHand(bool isEnabled)
    {
        if (isEnabled)
        {
            _movementRight.action.Enable();
            _accelerateRight.action.Enable();
        }
        else
        {
            _movementRight.action.Disable();
            _accelerateRight.action.Disable();
        }
    }
    
    private void OnMovement(InputAction.CallbackContext ctx)
    {
        var value = ctx.action.ReadValue<Vector2>();
        
        _movement.x = value.x;
        _movement.z = value.y;
        
        _movement.Normalize();
    }

    private void Update()
    {
        float movementSpeed = _isAccelerated ? _movementSpeed : _acceleratedSpeed;

        Quaternion toRotation = _characterController.rotation;
        if (_movement != Vector3.zero)
        {
            toRotation = Quaternion.LookRotation(_movement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);
        }
        _characterController.Move(_characterController.position + _movement * movementSpeed * Time.deltaTime, toRotation);
    }

    private void OnDestroy()
    {
        RemoteControl.OnEnableCarControls -= OnEnableCarControls;
    }
}