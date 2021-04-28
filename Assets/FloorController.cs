using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    // Start is called before the first frame update
    public int timePeriod = 1000;
    private int timeCount = 0;
    private int totalFloors;
    private float fallProb;

    void Start()
    {
        totalFloors = this.GetComponent<Transform>().childCount;
        fallProb = 1.0f - 4.0f / (float)totalFloors;
    }

    // Update is called once per frame
    void Update()
    {
        totalFloors = this.GetComponent<Transform>().childCount;
        timeCount++;
        if (timeCount >= timePeriod) 
        {
            timeCount = 0;
            print("Floor Falling!");
            for (int i = 0; i < totalFloors; i++) 
            {
                if (this.GetComponent<Transform>().GetChild(i).GetComponent<SingleFloorController>().isFallen)
                    continue;
                if (Random.value > fallProb)
                    this.GetComponent<Transform>().GetChild(i).GetComponent<SingleFloorController>().isFallen = true;
            }
            
        }
    }
}
