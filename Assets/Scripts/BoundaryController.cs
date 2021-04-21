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
            messageUI.transform.Find("KillMessage").gameObject.GetComponent<Text>().text = 
                other.GetComponent<WizardMovement>().hitPlayer + " killed " + other.name;
            Debug.Log(other.GetComponent<WizardMovement>().hitPlayer + " killed " + other.name);
            other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(new Vector3(973, -40, -400));
            //Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            Destroy(gameObject);
        }
    }
}
