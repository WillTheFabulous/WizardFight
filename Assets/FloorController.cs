using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    // Start is called before the first frame update
    public int timePeriod = 1000;
    private int timeCount = 0;
    private int totalFloors;
    private float fallProb;
    public int LoopCount = 3;
    private List<List<Transform>> LoopList;
    

    void Start()
    {
        totalFloors = this.GetComponent<Transform>().childCount;
        fallProb = 1.0f - 4.0f / (float)totalFloors;

        LoopList = new List<List<Transform>>();
        for(int i = 0; i < LoopCount; i++)
        {
            List<Transform> currList = new List<Transform>();
            LoopList.Add(currList);

        }
        totalFloors = this.GetComponent<Transform>().childCount;

        for(int i = 0; i < totalFloors; i++)
        {
            int Loop = this.GetComponent<Transform>().GetChild(i).GetComponent<SingleFloorController>().numLoop;

            LoopList[Loop].Add(this.GetComponent<Transform>().GetChild(i));
            
        }
        print(LoopList.Count);
       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        totalFloors = this.GetComponent<Transform>().childCount;
        timeCount++;

        
        if (timeCount >= timePeriod)
        {
            timeCount = 0;


            for (int i = LoopCount - 1; i >= 0; i--)
            {
                if (LoopList[i].Count == 0)
                {
                    continue;
                }
                int floorNum = (int)Mathf.Floor(Random.Range(0, LoopList[i].Count - 0.00001f));
                int minusone = LoopList.Count - 1;
                //print(minusone + " " + floorNum);
                Transform currFloor = LoopList[i][floorNum];
               

                if (Random.value > fallProb)
                {
                    if (currFloor.GetComponent<SingleFloorController>().isFallen != true)
                    {
                        currFloor.GetComponent<SingleFloorController>().isFallen = true;
                    }
                    LoopList[i].RemoveAt(floorNum);

                    //try
                    //{
                    //    LoopList[i].RemoveAt(floorNum);
                    //}
                    //catch (System.IndexOutOfRangeException e)
                    //{
                    //    print(minusone + " " + floorNum);
                    //}
                }
                break;
            }

        }
    }
}
