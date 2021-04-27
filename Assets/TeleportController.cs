using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject exit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wizard" || other.tag == "Bullet")
        {
            other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(exit.GetComponent<Transform>().position + 7.0f * exit.GetComponent<Transform>().forward);
            //Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            Destroy(gameObject);
        }
    }
}
