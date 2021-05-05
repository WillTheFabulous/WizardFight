using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BottomController : MonoBehaviour
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
            //other.gameObject.GetComponent<WizardMovement>().hitPlayer = shooter;
            //other.gameObject.GetComponent<WizardMovement>().hitBack(bullet.forward);
            WizardMovement wizardScript = other.gameObject.GetComponent<WizardMovement>();

            wizardScript.lifeCount--;
            if (wizardScript.lifeCount == 0)
                PhotonNetwork.Destroy(other.gameObject);
            else 
            {
                Vector3 spawnPos = new Vector3(973, -40, -400);
                wizardScript.photonView.RPC("setPosition", RpcTarget.All, spawnPos);
                wizardScript.photonView.RPC("updateUI", RpcTarget.All, wizardScript.lifeCount);
                //wizardScript._uiGo.SendMessage("UpdateText", SendMessageOptions.RequireReceiver);

                //PlayerUI uiObject = wizardScript._uiGo.GetComponent<PlayerUI>();
                //uiObject.photonView.RPC("UpdateText", RpcTarget.All);
            }
                

            //PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
