using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
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
        if (other.tag == "Wizard")
        {
            // Hit an enemy
            other.gameObject.GetComponent<WizardMovement>().hitPlayer = shooter;
            other.gameObject.GetComponent<WizardMovement>().hitBack(bullet.forward);
            Destroy(gameObject);
        }
        else if (other.tag == "Base")
        {
            // Hit the base
            Destroy(gameObject);
        }
    }
}
