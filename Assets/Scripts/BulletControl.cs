using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class BulletControl : MonoBehaviourPunCallbacks
{
    public float speed;
    private Transform bullet;
    public string shooter;
    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bullet.position += bullet.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (other.tag == "Wizard")
        {
            // Hit an enemy
            //other.gameObject.GetComponent<WizardMovement>().hitPlayer = shooter;
            //other.gameObject.GetComponent<WizardMovement>().hitBack(bullet.forward);

            other.gameObject.GetComponent<WizardMovement>().photonView.RPC("hitBack", RpcTarget.All, bullet.forward);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void setPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
