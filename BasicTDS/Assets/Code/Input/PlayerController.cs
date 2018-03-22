using UnityEngine;
using System.Collections;
using System;

public class PlayerController : IController
{
    private float _moveDeadZone = 0.2f;
    private float _aimDeadZone = 0.2f;
    private float _aimKeyReleaseWindow = 0.12f;
    private float _lookSmooth = 1;
    
    // inputs and cached values
    private bool _useMouse = false;
    private Vector2 _aimInput = Vector2.zero;
    private Vector2 _aimDiagonalLast = Vector2.zero;
    private Vector3 _prevMousePos = Vector3.zero;
    private Vector3 _cameraOffset = Vector3.zero;
    private Vector3 _camVelocity = Vector3.zero;

    // timers
    private float _fireTimer = 0;
    private float _aimKeyReleaseTimer = 0;

    private Rigidbody2D _rb;
    private Camera _cam;

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void Update()
    {
        // Movement
        Movement.x = Input.GetAxis("Horizontal");
        Movement.y = Input.GetAxis("Vertical");
        Movement = Movement.normalized * 1000;
        if (Movement.magnitude <= _moveDeadZone)
        {
            Movement = Vector2.zero;
        }

        // Aim
        _aimInput.x = Input.GetAxis("Horizontal2");
        _aimInput.y = Input.GetAxis("Vertical2");
        if (_aimInput != Vector2.zero && _aimInput.magnitude > _aimDeadZone)
        {
            _useMouse = false;
            if (Mathf.Abs(_aimInput.x) == 1 && Mathf.Abs(_aimInput.y) == 1)
            {
                _aimKeyReleaseTimer = _aimKeyReleaseWindow;
                _aimDiagonalLast = _aimInput;
            }
            else if (_aimKeyReleaseTimer > 0)
            {
                _aimInput = _aimDiagonalLast;
            }
            Rotation = MathExt.AngleBetweenTwoPoints(_aimInput.normalized, Vector3.zero);
        }
        else if (_prevMousePos.x != Input.mousePosition.x || _prevMousePos.y != Input.mousePosition.y)
        {
            _useMouse = true;
        }
        _prevMousePos = Input.mousePosition;

        if (_useMouse)
        {
            Vector3 mouseInWorld = _cam.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            mouseInWorld.z = 0;
            Rotation = MathExt.AngleBetweenTwoPoints(mouseInWorld, transform.position);
            _cameraOffset = transform.position + (mouseInWorld - transform.position) * 0.2f;
        }
        else
        {
            _cameraOffset = transform.position;
        }

        _cameraOffset.z = -10.0f;

        // Timers
        Timer.Increment(ref _aimKeyReleaseTimer);

        // Keep dynamic rigid body from falling asleep automatically without contact or movement
        _rb.AddForce(Vector2.zero);
    }

    public void LateUpdate()
    {
        _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, _cameraOffset, ref _camVelocity, _lookSmooth * Time.deltaTime);
    }
}
