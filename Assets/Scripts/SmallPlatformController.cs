using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPlatformController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = player.transform.rotation;
    }
}
