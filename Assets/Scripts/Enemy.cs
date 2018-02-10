using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject laser;
    public Transform laserSpawn;

    /// <summary>
    /// Instantiates a laser object and fires it at the direction of the user
    /// </summary>
    public void Fire()
    {
        GameObject laserInstance = Instantiate(laser, laserSpawn);
        Laser laserObject = laserInstance.GetComponent<Laser>();
        laserObject.Fire();
    }

    /// <summary>
    /// When we are hit, destroy the laser and the enemy game object.
    /// Notify the GameController that the score should be changed
    /// </summary>
    /// <param name="other">the collider that has come in contact with our collider</param>
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        GameController gameController = gameControllerObject.GetComponent<GameController>();
        if (gameController != null)
        {
            gameController.AddToScore();
        }
        Destroy(other.gameObject);

        ParticleSystem explosion = GetComponent<ParticleSystem>();
        explosion.Play();
        Destroy(gameObject, explosion.main.duration);
    }
}
