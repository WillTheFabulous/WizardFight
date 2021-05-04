using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class BoundaryController : MonoBehaviourPunCallbacks
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
            Vector3 newPosition = new Vector3(0, 0, 0);
            if (dotDir > 0.0f)
            {
                //other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(
                newPosition = other.gameObject.GetComponent<Transform>().position + 70.0f * this.gameObject.GetComponent<Transform>().forward;
                //other.gameObject.transform.position = newPosition;
                other.gameObject.GetComponent<WizardMovement>().photonView.RPC("setPosition", RpcTarget.All, newPosition);
            }
            else
            {
                //other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(
                newPosition = other.gameObject.GetComponent<Transform>().position - 70.0f * this.gameObject.GetComponent<Transform>().forward;
                other.gameObject.GetComponent<WizardMovement>().photonView.RPC("setPosition", RpcTarget.All, newPosition); 
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
