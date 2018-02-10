using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject laser; //Refrence to the laser prefab
    public Transform laserSpawn; //Reference to the spawn transform to use for the laser

    public float speed;
    public float xMin, xMax; // bounds for where the player can be on the x-axis
    public float laserCooldown; // Time in between shots
    public int health = 100;
    public int damagePerHit = 50;

    private float timeToNextShot = 0.0f;
    private ParticleSystem explosion;
    public int Health { get { return health; } }

    void Start()
    {
        explosion = GetComponent<ParticleSystem>();
    }

    public void Reset()
    {
        health = 100;
    }

	void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 velocity = new Vector3(horizontalInput*speed, 0.0f, 0.0f);
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = velocity;

        //Make sure we do not go off the playing field
        rigidBody.position = new Vector3(Mathf.Clamp(rigidBody.position.x, xMin, xMax), rigidBody.position.y, rigidBody.position.z);
    }

    /// <summary>
    /// Monitor for input to fire the laser and police the cooldown between shots
    /// </summary>
    void Update()
    {
        timeToNextShot -= Time.deltaTime;
        if(Input.GetButton("Fire1") && timeToNextShot <= 0.0f)
        {
            Fire();
        }
    }

    /// <summary>
    /// Spawns a laser instance towards the enemies
    /// </summary>
    private void Fire()
    {
        GameObject laserInstance = Instantiate(laser, laserSpawn.position, laserSpawn.rotation);
        Laser laserObject = laserInstance.GetComponent<Laser>();
        laserObject.Fire(true);
        timeToNextShot = laserCooldown;
    }

    /// <summary>
    /// Handles getting hit by a laser, by deducting the players health and notifying the
    /// GameController to update health remaining
    /// </summary>
    /// <param name="other">the collider of the laser</param>
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        GameController gameController = gameControllerObject.GetComponent<GameController>();
        health -= damagePerHit;
        explosion.Play();
        //User has been destroyed
        if(health <= 0)
        {
            gameController.UserDestroyed();
        //    Destroy(gameObject, explosion.main.duration);
        }
        Destroy(other.gameObject);
    }
}
