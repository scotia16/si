using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public float speed;
    private float ttl = 6.0f; //time to live

    /// <summary>
    /// Assigns a velocity to the rigid body for the laser
    /// </summary>
    /// <param name="isUser">true if firing from th euser</param>
    public void Fire(bool isUser = false)
    {
        float y = isUser ? speed * 1.0f : speed * -1.0f; 
        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, y, 0.0f);
    }
    
    void Update()
    {
        ttl -= Time.deltaTime;
        if(ttl <= 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
