using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFloorController : MonoBehaviour
{
    public bool isFallen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isFallen) 
        {
            this.GetComponent<Transform>().position -= new Vector3(0.0f, 1.0f, 0.0f);
            //if(this.GetComponent<Transform>().position.y <= -100.0f)
                //Destroy(gameObject);
        }
    }
}
