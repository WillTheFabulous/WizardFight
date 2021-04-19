using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassForceSphere : MonoBehaviour
{
    [SerializeField]
    private float radius;

    [SerializeField]
    private float height;

    public float Radius
    {
        get { return radius; }

        set
        {
            radius = value;
        }
    }

    public float Height
    {
        get { return height; }

        set
        {
            height = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("Character0", new Vector4(transform.position.x, transform.position.y + Height, transform.position.z, Radius));
    }

    private void OnDrawGizmos()
    {
        Vector3 center = new Vector3(transform.position.x, transform.position.y + Height, transform.position.z);
        Gizmos.DrawWireSphere(center, Radius);
    }
}
