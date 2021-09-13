using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            PlayerStateManager.platformCollider = collision;
           // Debug.Log("1");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == PlayerStateManager.platformCollider)
        {
            PlayerStateManager.platformCollider = null;
           // Debug.Log("2");
        }
    }
}
