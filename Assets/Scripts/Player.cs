using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    // config params
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    
    [SerializeField] float padding = 1f;
    [SerializeField] int health = 600;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 5f;
    [SerializeField] float projectileWait = 0.1f;
    [SerializeField] AudioClip projectileSound;
    [SerializeField] [Range(0, 1)] float projectileVolume = 0.4f;

    [Header("Explosion")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] float explosionDuration = 1f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathVolume = 0.8f;

    [Header("Boost")]
    [SerializeField] float boostSpeed = 13f;
    [SerializeField] float boostTime = 8f;
    [SerializeField] float projectileBoost = 15f;
    [SerializeField] float projectileBoostWait = 0.05f;
    [SerializeField] Image boostPowerupImage;

    [SerializeField] Image shieldPowerupImage;
    [SerializeField] Image onehitPowerupImage;
    [SerializeField] float onehitDuration = 5f;

    float xMin, xMax, yMin, yMax;
    Coroutine fireCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    void Update()
    {
        Move();
        Fire();
    }

    public void OneHit()
    {
        StartCoroutine(IncreaseDamage());
        
    }

    IEnumerator IncreaseDamage()
    {
        laserPrefab.GetComponent<DamageDealer>().SetDamage(500);
        onehitPowerupImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(onehitDuration);
        laserPrefab.GetComponent<DamageDealer>().SetDamage(100);
        onehitPowerupImage.gameObject.SetActive(false);
    }

    private void Fire()
    {
        
        if (Input.GetButtonDown("Fire1"))
        {
            fireCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(fireCoroutine);
        }
        
    }

    public int GetHealth()
    {
        return health;
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(projectileSound, Camera.main.transform.position, projectileVolume);
            yield return new WaitForSeconds(projectileWait);
        }
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0)).y - padding;
    }

    // Update is called once per frame
    

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
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
        if (health <= 0)
        {
            health = 0;
        }
        damageDealer.Hit();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Boost()
    {
        StartCoroutine(BoostPlayer());
    }

    public void ShieldUp()
    {
        shieldPowerupImage.gameObject.SetActive(true);
    }

    public void ShieldDown()
    {
        shieldPowerupImage.gameObject.SetActive(false);
    }

    IEnumerator BoostPlayer()
    {
        float oldSpeed = moveSpeed;
        float oldProjectileSpeed = projectileSpeed;
        float oldProjectileWait = projectileWait;
        moveSpeed = boostSpeed;
        projectileSpeed = projectileBoost;
        projectileWait = projectileBoostWait;
        boostPowerupImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(boostTime);
        moveSpeed = oldSpeed;
        projectileSpeed = oldProjectileSpeed;
        projectileWait = oldProjectileWait;
        boostPowerupImage.gameObject.SetActive(false);
    }

    private void Die()
    {
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(explosion, explosionDuration);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathVolume);
        FindObjectOfType<Level>().LoadGameOver();

    }
}
