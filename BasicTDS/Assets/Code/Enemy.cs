using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
	public new void Start ()
    {
        base.Start();
	}
	
	public new void Update ()
    {
        base.Update();
	}

    public void FixedUpdate()
    {
        if (!_rb.IsSleeping())
        {
            _rb.velocity = Vector2.Lerp(
                _rb.velocity,
                Vector2.ClampMagnitude(_controller.Movement, MoveSpeed),
                RUN_LERP * Time.fixedDeltaTime);
            _rb.rotation = Mathf.LerpAngle(_rb.rotation, _controller.Rotation, AIM_LERP * Time.fixedDeltaTime) % 360;
        }
    }

    public void OnGUI()
    {
    }

    public void OnDestroy()
    {
    }
}
