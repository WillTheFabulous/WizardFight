using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

public class WizardMovement : MonoBehaviourPunCallbacks
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

    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        hitPlayer = this.name;
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
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        Vector3 newPosition = new Vector3(0, 0, 0);
        Vector3 newEulerAngles = new Vector3(0, 0, 0);
        GetNewPosition(Time.deltaTime, out newPosition, out newEulerAngles);
        transform.position = newPosition;
        transform.localEulerAngles = newEulerAngles;
    }

    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            //AudioSource.PlayClipAtPoint(fireSoundEffect, gameObject.transform.position);

            // spawn bullet
            //GameObject bulletObject = Instantiate(bullet, bulletSpawn.position + bulletSpawn.forward * 1.2f * transform.localScale.z, bulletSpawn.rotation);
            GameObject bulletObject = PhotonNetwork.Instantiate(bullet.name, transform.position + transform.forward * 1.2f * transform.localScale.z, transform.rotation, 0);
            bulletObject.GetComponent<BulletControl>().shooter = this.name;
            //bulletObject.transform.localScale = new Vector3(1f, 3f, 3f);
        }

        if (isDead)
        {
            GameManager.Instance.LeaveRoom();
        }
    }

    [PunRPC]
    public void hitBack(Vector3 hitDir)
    {
        isHitBack = true;
        hitbackDir = hitDir;
    }

    [PunRPC]
    public void setPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
