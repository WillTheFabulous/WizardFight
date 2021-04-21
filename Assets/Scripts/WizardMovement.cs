using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class WizardMovement : NetworkBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -10f;
    public float jumpHeight = 2f;
    public float fireRate;
    public GameObject bullet;
    public Transform bulletSpawn;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isHitBack = false;
    private int hitBackCount = 25;
    private float backSpeed = 4f;
    public float nextFire;
    private bool upgraded;
    private Vector3 hitbackDir;

    public string hitPlayer;
    Vector3 velocity;
    bool isGrounded;

    public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    public NetworkVariableVector3 LocalEulerAngles = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        hitPlayer = this.name;
    }

    public override void NetworkStart()
    {
        transform.position = new Vector3(973, -40, -400);
        transform.localScale = new Vector3(5, 5, 5);

        Position.Value = transform.position;
    }

    [ServerRpc]
    public void SubmitPositionRequestServerRpc(Vector3 position)
    {
        Position.Value = position;
    }

    [ServerRpc]
    public void SubmitLocalEulerAnglesRequestServerRpc(Vector3 localEulerAngles)
    {
        LocalEulerAngles.Value = localEulerAngles;
    }

    [ServerRpc]
    public void SubmitShootBulletRequestServerRpc()
    {
        GameObject bulletObject = Instantiate(bullet, bulletSpawn.position + bulletSpawn.forward * 1.2f * transform.localScale.z, bulletSpawn.rotation);
        bulletObject.GetComponent<BulletControl>().shooter = this.name;
        bulletObject.transform.localScale = new Vector3(1f, 3f, 3f);
    }

    public void GetNewPosition(float deltaTime, out Vector3 newPosition, out Vector3 newEulerAngles)
    {
        newPosition = transform.position;
        newEulerAngles = transform.localEulerAngles;
        if (!isHitBack)
        {
            float x;
            float z;
            //bool jumpPressed = false;

            float scale = 10f;
            x = Input.GetAxis("Horizontal") * scale;
            z = Input.GetAxis("Vertical") * scale;
            //jumpPressed = Input.GetButtonDown("Jump");

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 screenAheadPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, 1f));

            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);

            Vector3 aheadVec = screenAheadPos - screenPos;
            Vector3 mouseObjectVec = mousePos - screenPos;

            float rotateAngle = Vector3.Angle(aheadVec, mouseObjectVec);


            if (Vector3.Dot(new Vector3(1f, 0f, screenPos.z), mouseObjectVec) >= 0f)
            {
                newEulerAngles = new Vector3(0f, rotateAngle, 0f);
            }
            else
            {
                newEulerAngles = new Vector3(0f, -rotateAngle, 0f);
            }

            Vector3 move = transform.right * x + transform.forward * z;

            //controller.Move(move * speed * Time.deltaTime);
            newPosition += move * speed * deltaTime;
            ///controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            if (hitBackCount >= 1)
                hitBackCount--;
            else
            {
                isHitBack = false;
                hitBackCount = 25;
                hitPlayer = this.name;
            }


            //Vector3 move = transform.forward;
            ///controller.Move(hitbackDir * backSpeed * transform.localScale.z * Time.deltaTime);
            newPosition += hitbackDir * backSpeed * transform.localScale.z * deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                    out var networkedClient))
        {
                Vector3 newPosition = new Vector3(0, 0, 0);
                Vector3 newEulerAngles = new Vector3(0, 0, 0);
                GetNewPosition(Time.deltaTime, out newPosition, out newEulerAngles);

                if (!NetworkManager.Singleton.IsServer)
                {
                    SubmitPositionRequestServerRpc(newPosition);
                    SubmitLocalEulerAnglesRequestServerRpc(newEulerAngles);
                }
                else
                {
                    Position.Value = newPosition;
                    LocalEulerAngles.Value = newEulerAngles;
                }*/


    }

    private void Update()
    {
        transform.position = Position.Value; 
        transform.localEulerAngles = LocalEulerAngles.Value;
    }

    public void hitBack(Vector3 hitDir) 
    {
        isHitBack = true;
        hitbackDir = hitDir;
    }
}
