using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{   
    Left,
    Right,
    Down,
}


public class InvaderController : MonoBehaviour {

    public Transform spawnOrigin; // the anchor points used to spawn enemies
    public GameObject enemyPrefab;
    public float moveDistance = 0.3f;

    public int movementCooldown;

    private float timeToNextMove = 3.0f;
    private Queue<MoveDirection> movementSequence;

    private List<Queue<GameObject>> enemies;

    private Vector3 cacheSpawnOrigin;

    public float minShotDelay = 1.0f;
    public float maxShotDelay = 2.0f;
    private float timeToNextShot;
    
    private const int NUM_ROWS = 5;
    private const int NUM_COLUMNS = 7;
    private const float ENEMY_PADDING = 1.7f;
    private bool inProgress = false;

    // Use this for initialization
    void Start() {
        cacheSpawnOrigin = spawnOrigin.transform.position; //Cache the origin for restarting the game
        InitMovementSequence();
    }

    public void StartAssult()
    {
        enemies = new List<Queue<GameObject>>();
        SpawnEnemies();
        inProgress = true;
    }

    public void StopAssult()
    {
        inProgress = false;
    }

    public bool EnemiesExist()
    {
        return enemies.Count > 0;
    }

    /// <summary>
    /// Clean up objects from previous game and reset the movement sequence
    /// </summary>
    public void Reset()
    {
        InitMovementSequence();
        spawnOrigin.transform.position = cacheSpawnOrigin;
        foreach(Queue<GameObject> enemyColumn in enemies)
        {
            if (enemyColumn == null)
                continue;

            while (enemyColumn.Count > 0)
            {
                GameObject enemy = enemyColumn.Dequeue();
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
        }
        enemies = null;
    }

    /// <summary>
    /// Handle movement of enemies and when they fire at the user
    /// </summary>
    void FixedUpdate()
    {
        if (!inProgress) return;

        timeToNextMove -= Time.fixedDeltaTime;
        if (timeToNextMove <= 0.0f)
        {
            MoveEnemies();
            timeToNextMove = movementCooldown;
        }

        timeToNextShot -= Time.fixedDeltaTime;
        if (timeToNextShot <= 0.0f)
        {
            EnemyFire();
            timeToNextShot = Random.Range(minShotDelay, maxShotDelay);
        }
    }

    /// <summary>
    /// When it is time to move the enemy horde, we just move the transform that 
    /// all enemies are parented to.
    /// </summary>
    private void MoveEnemies()
    {
        MoveDirection direction = movementSequence.Dequeue();
        Vector3 movementVector = Vector3.zero;
        switch(direction)
        {
            case MoveDirection.Right:
                {
                    movementVector += new Vector3(1 * moveDistance, 0.0f, 0.0f);
                }
                break;
            case MoveDirection.Left:
                {
                    movementVector += new Vector3(-1 * moveDistance, 0.0f, 0.0f);
                }
                break;
            case MoveDirection.Down:
                {
                    movementVector += new Vector3(0.0f, -1 * moveDistance, 0.0f);
                }
                break;
            default:
                {
                    Debug.LogError("Unhandled move direction");
                }
                break;
        }
        spawnOrigin.transform.position += movementVector;
        //Put this movement at the back of the queue again
        movementSequence.Enqueue(direction);
    }

    /// <summary>
    /// Setup the order that the enemies will move in, this sequence will
    /// keep repeating until the game is over
    /// </summary>
    private void InitMovementSequence()
    {
        movementSequence = new Queue<MoveDirection>();
        movementSequence.Enqueue(MoveDirection.Right);
        movementSequence.Enqueue(MoveDirection.Right);
        movementSequence.Enqueue(MoveDirection.Right);
        movementSequence.Enqueue(MoveDirection.Down);
        movementSequence.Enqueue(MoveDirection.Left);
        movementSequence.Enqueue(MoveDirection.Left);
        movementSequence.Enqueue(MoveDirection.Left);
        movementSequence.Enqueue(MoveDirection.Down);
    }

    /// <summary>
    /// Create and position all the enemy objects
    /// </summary>
    private void SpawnEnemies()
    {
        for(int i = 0; i < NUM_COLUMNS; i++)
        {
            Queue<GameObject> enemyColumn = new Queue<GameObject>();
            for (int j = 0; j < NUM_ROWS; j++)
            {
                GameObject enemyObject = Instantiate(enemyPrefab, spawnOrigin.position + new Vector3(i * ENEMY_PADDING, j * ENEMY_PADDING, 0.0f), spawnOrigin.rotation);
                enemyObject.transform.parent = spawnOrigin.transform;
                enemyColumn.Enqueue(enemyObject);
            }
            enemies.Add(enemyColumn);
        }
    }

    /// <summary>
    /// Determine which column is going to fire at the user, and get
    /// enemy that is the closet to the user (first one in the queue)
    /// Where enemy objects references are kept in a queue, but destroyed
    /// elsewhere, we need to double check that our references are still valid
    /// otherwise remove them from the queue as they have been blown up by the user
    /// </summary>
    private void EnemyFire()
    {
        //Get a random column based on how many columns are remaining
        int enemyColumnIndex = Random.Range(0, enemies.Count);
        Queue<GameObject> enemyColumn = enemies[enemyColumnIndex];
        if(enemyColumn == null)
        {
            Debug.LogError("EnemyColumn is null");
        }
        while(enemyColumn.Count > 0 && enemyColumn.Peek() == null)
        {
            enemyColumn.Dequeue();
            if(enemyColumn.Count == 0)
            {
                enemies.Remove(enemyColumn);
            }
        }

        //Grab the enemy that is in the front of the queue, and fire at the user
        if (enemyColumn.Count != 0)
        {
            Enemy enemy = enemyColumn.Peek().GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.Fire();
            }
            else
            {
                Debug.LogError("Trying to fire a laser from an Enemy that is null");
            }
        }
    }
}
