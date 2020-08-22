using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] int health = 500;
    Player player;

    [SerializeField] AudioClip shieldUp;
    [SerializeField] [Range(0, 1)] float shieldUpVolume = 0.8f;
    [SerializeField] AudioClip shieldDown;
    [SerializeField] [Range(0, 1)] float shieldDownVolume = 0.8f;
    


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        AudioSource.PlayClipAtPoint(shieldUp, Camera.main.transform.position, shieldUpVolume);
        player.ShieldUp();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(shieldDown, Camera.main.transform.position, shieldDownVolume);
        player.ShieldDown();
    }
}
