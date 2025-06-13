using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public float destoryTime;
    // Start is called before the first frame update
    void Start()
    {
        if(destoryTime != -1)
        {
            Destroy(gameObject, destoryTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
