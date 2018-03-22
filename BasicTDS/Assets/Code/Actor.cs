using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IController))]
public class Actor : MonoBehaviour
{
    protected const float RUN_LERP = 10f;
    protected const float AIM_LERP = 30f;

    [Header("Defense")]
    public float MaxHealth = 100;
    public float HealthRegen = 0;
    public float Armor = 0;
    public float MoveSpeed = 4;

    [Space(4)]
    [Header("Attack")]
    public float Power = 10;
    public float AttackSpeed = 10;
    public float Crit = 2.5f;
    public float CooldownReduction = 0;

    protected float _health;

    protected IController _controller;
    protected Rigidbody2D _rb;
    protected CircleCollider2D _hitBox;

    private Vector2 vr, vcw, vs, vccw;

    private Vector2 _velocity = Vector2.zero;

    public void Start()
    {
        _controller = GetComponent<IController>();
        _rb = GetComponent<Rigidbody2D>();
        _hitBox = GetComponent<CircleCollider2D>();
        _health = MaxHealth;
    }

    public void Update()
    {
        // inherited by subclasses
    }

    public void LateUpdate()
    {
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            _health += HealthRegen * Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        // inherited by subclasses
    }

    public void Damage(float dmg)
    {
        _health -= dmg - dmg * Armor / (Armor + 100);
    }

    public void HealPercent(float percent)
    {
        _health = Mathf.Clamp(_health + MaxHealth * percent / 100, 0, MaxHealth);
    }

    public void Heal(float health)
    {
        _health = Mathf.Clamp(_health + health, 0, MaxHealth);
    }

    public void Attack(GameObject other)
    {
        int critMult = ((Random.value * 100) <= Crit) ? 1 : 2;
        other.SendMessage("Damage", Power * critMult);
    }
}
