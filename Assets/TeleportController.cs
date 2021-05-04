using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class TeleportController : MonoBehaviourPunCallbacks
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
            ///other.gameObject.GetComponent<WizardMovement>().SubmitPositionRequestServerRpc(exit.GetComponent<Transform>().position + 7.0f * exit.GetComponent<Transform>().forward);
            Vector3 newPosition = exit.GetComponent<Transform>().position + 7.0f * exit.GetComponent<Transform>().forward;
            if (other.tag == "Wizard") other.gameObject.GetComponent<WizardMovement>().photonView.RPC("setPosition", RpcTarget.All, newPosition);
            if (other.tag == "Bullet") other.gameObject.GetComponent<BulletControl>().photonView.RPC("setPosition", RpcTarget.All, newPosition);

            //PhotonNetwork.Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            ///Destroy(gameObject);
        }
    }
}
