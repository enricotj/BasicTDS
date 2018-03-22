using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Wander,
    Chase,
    Attack,
}

public class EnemyController : IController
{
    public float _visionRange = 12;
    public float _visionAngle = 120;

    private float _wanderCircleDistance = 50;
    private float _wanderCircleRadius = 49;
    private float _wanderAngleChange = Mathf.PI * 2;
    private float _wanderAngle = 0;
    private float _wanderForceMax = 10;

    private bool _playerInVision = false;
    private bool _hasLineOfSight = false;
    private EnemyState _state = EnemyState.Wander;
    private GameObject _player = null;
    private Rigidbody2D _rb;

    public EnemyState State
    {
        get
        {
            return _state;
        }
    }

    public void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        _state = EnemyState.Wander;
        _wanderAngle = Random.value * 2 * Mathf.PI;

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player == null)
        {
            return;
        }
        
        float pdist = (_player.transform.position - transform.position).sqrMagnitude;
        if (_rb.IsAwake() && pdist >= 800)
        {
            _rb.Sleep();
            return;
        }
        else if (!_rb.IsAwake() && pdist < 800)
        {
            _rb.WakeUp();
        }

        if (_state != EnemyState.Chase)
        {
            if (PlayerInFieldOfVision() && HasLineOfSight())
            {
                _state = EnemyState.Chase;
            }
        }
        else
        {
            if (!PlayerInFieldOfVision())
            {
                _state = EnemyState.Wander;
            }
        }

        switch (_state)
        {
            case EnemyState.Wander:
                var circleCenter = _rb.velocity.normalized * _wanderCircleDistance;
                var displacement = (new Vector2(0, 1)) * _wanderCircleRadius;
                var len = displacement.magnitude;
                displacement.x = Mathf.Cos(_wanderAngle) * len;
                displacement.y = Mathf.Sin(_wanderAngle) * len;
                _wanderAngle = Random.value * _wanderAngleChange - _wanderAngleChange * 0.5f;
                var wanderForce = (circleCenter + displacement);
                if (wanderForce.magnitude > _wanderForceMax)
                {
                    wanderForce = wanderForce.normalized * _wanderForceMax;
                }
                Movement = _rb.velocity + wanderForce;
                Rotation = (MathExt.AngleBetweenTwoPoints(_rb.position + _rb.velocity, _rb.position) + 360) % 360;
                break;
            case EnemyState.Chase:
                if (_player != null)
                {
                    Movement = (_player.transform.position - transform.position) * 100;
                    Rotation = (MathExt.AngleBetweenTwoPoints(_player.transform.position, transform.position) + 360) % 360;
                }
                else
                {
                    _state = EnemyState.Wander;
                }
                break;
            case EnemyState.Attack:
                break;
        }
    }

    public void LateUpdate()
    {
    }

    public void OnDestroy()
    {
        
    }

    private bool PlayerInFieldOfVision()
    {
        if (_player != null)
        {
            Vector3 playerDir = _player.transform.position - transform.position;
            if (((Vector2)playerDir).magnitude <= _visionRange && 
                Mathf.Abs(Mathf.DeltaAngle(MathExt.AngleBetweenTwoPoints(playerDir, Vector3.zero), _rb.rotation)) <= (_visionAngle / 2))
            {
                return true;
            }
        }
        return false;
    }

    private bool HasLineOfSight()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        int x2 = (int)_player.transform.position.x;
        int y2 = (int)_player.transform.position.y;

        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            if (Cave.IsFilled(x, y))
            {
                return false;
            }

            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
        return true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state != EnemyState.Chase && collision.gameObject.tag == "Terrain")
        {
            // TODO: Reverse direction?
        }
    }
}
