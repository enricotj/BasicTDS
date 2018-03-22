using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    ///################
    /// PUBLIC MEMBERS
    ///################
    
    private int fps;
    private int fpsMin = 120;
    private float deltaTime;

    [Space(10)]
    [Header("Firing")]
    public float _fireWindow = 0.2f;

    [Space(10)]
    [Header("Prefabs")]
    public GameObject _bulletPrefab;

    ///################
    /// PRIVATE MEMBERS
    ///################

    // timers
    private float _fireTimer = 0;
    private float _aimKeyReleaseTimer = 0;

    public new void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = (int)Mathf.Ceil(1.0f / deltaTime);
        fpsMin = (fps < fpsMin) ? fps : fpsMin;

        // Quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        Timer.Increment(ref _fireTimer);
        if (_fireTimer == 0 && Input.GetButton("Fire1"))
        {
            _fireTimer = _fireWindow;
            GameObject bullet = Instantiate(_bulletPrefab, this.transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().Init(_rb.rotation);
        }

        base.Update();
    }

    public new void LateUpdate()
    {
        base.LateUpdate();
    }

    public new void FixedUpdate()
    {
        _rb.velocity = Vector2.Lerp(
            _rb.velocity,
            Vector2.ClampMagnitude(_controller.Movement, MoveSpeed),
            RUN_LERP * Time.fixedDeltaTime);
        _rb.rotation = Mathf.LerpAngle(_rb.rotation, _controller.Rotation, AIM_LERP * Time.fixedDeltaTime) % 360;
    }

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(0, 0, 128, 64), fps.ToString());
        GUI.Label(new Rect(0, 64, 128, 64), fpsMin.ToString());
    }
}
