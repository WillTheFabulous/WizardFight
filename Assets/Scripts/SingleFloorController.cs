using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFloorController : MonoBehaviour
{
    public bool isFallen = false;
    private Material redMat;
    private Material curMat;
    private int preCount = 50;
    private int cnt = 0;
    private Renderer rend;
    public int numLoop;
    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<Transform>().GetChild(1).GetComponent<Renderer>();
        curMat = rend.material;
        redMat = new Material(curMat);
        //redMat.CopyPropertiesFromMaterial(curMat);
        redMat.color = new Color(1.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isFallen) 
        {
            if (cnt < preCount)
            {
                float interp = (float)(cnt % 10) / 10.0f;
                rend.material.Lerp(curMat, redMat, interp);
                cnt++;
            }
            else 
            {
                this.GetComponent<Transform>().position -= new Vector3(0.0f, 1.0f, 0.0f);
            }
            
           
            //if(this.GetComponent<Transform>().position.y <= -100.0f)
                //Destroy(gameObject);
        }
    }
}
