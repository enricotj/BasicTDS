using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float _speed = 22;
    public float _life = 10;

    public Vector3 velocity;

    public void Init(float angle)
    {
        float rads = angle * Mathf.PI / 180;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        velocity = (new Vector3(Mathf.Cos(rads), Mathf.Sin(rads), 0)) * _speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            if (other.tag == "Enemy")
            {
                other.SendMessage("Damage", 10);
            }
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        transform.position += velocity * Time.deltaTime;
        Timer.Increment(ref _life, delegate{ Destroy(gameObject); });
    }
}
