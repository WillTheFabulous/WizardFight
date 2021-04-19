using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMovement : MonoBehaviour
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
    private float nextFire;
    private bool upgraded;
    private Vector3 hitbackDir;

    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHitBack)
        {
            float x;
            float z;
            //bool jumpPressed = false;

            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            //jumpPressed = Input.GetButtonDown("Jump");

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 screenAheadPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, 1f));

            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);

            Vector3 aheadVec = screenAheadPos - screenPos;
            Vector3 mouseObjectVec = mousePos - screenPos;

            float rotateAngle = Vector3.Angle(aheadVec, mouseObjectVec);


            if (Vector3.Dot(new Vector3(0f, 1f, screenPos.z), mouseObjectVec) >= 0f)
            {
                transform.localEulerAngles = new Vector3(0f, rotateAngle, 0f);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0f, -rotateAngle, 0f);
            }

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);
            //controller.Move(velocity * Time.deltaTime);
        }
        else 
        {
            if (hitBackCount >= 1)
                hitBackCount--;
            else 
            {
                isHitBack = false;
                hitBackCount = 25;
            }
                

            //Vector3 move = transform.forward;
            controller.Move(hitbackDir * backSpeed * Time.deltaTime);
        }
        
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            //AudioSource.PlayClipAtPoint(fireSoundEffect, gameObject.transform.position);
            Instantiate(bullet, bulletSpawn.position + bulletSpawn.forward * 1.2f, bulletSpawn.rotation);
        }
    }

    public void hitBack(Vector3 hitDir) 
    {
        isHitBack = true;
        hitbackDir = hitDir;
    }
}
