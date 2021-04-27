using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundaryController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wizard")
        {
            // Hit an enemy
            //other.gameObject.GetComponent<WizardMovement>().hitBack(bullet.forward);
            GameObject messageUI = GameObject.Find("MessageUI");
            /*messageUI.transform.Find("KillMessage").gameObject.GetComponent<Text>().text = 
                other.GetComponent<WizardMovement>().hitPlayer + " killed " + other.name;*/
            //Debug.Log(other.GetComponent<WizardMovement>().hitPlayer + " killed " + other.name);
            Vector3 spawnPos = new Vector3(973, -40, -400);
            Vector3 dir = spawnPos - this.gameObject.GetComponent<Transform>().position;
            float dotDir = Vector3.Dot(dir, this.gameObject.GetComponent<Transform>().forward);
            if (dotDir > 0.0f)
            {
                other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(other.gameObject.GetComponent<Transform>().position + 150.0f * this.gameObject.GetComponent<Transform>().forward);
            }
            else 
            {
                other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(other.gameObject.GetComponent<Transform>().position - 150.0f * this.gameObject.GetComponent<Transform>().forward);
            }
           
            //Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            Destroy(gameObject);
        }
    }
}
