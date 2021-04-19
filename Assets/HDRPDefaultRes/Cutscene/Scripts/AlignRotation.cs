using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AlignRotation : MonoBehaviour
{
    public Transform directionalLight;

    // Update is called once per frame
    void Update()
    {
        if (directionalLight == null)
            return;

        transform.rotation = directionalLight.rotation;
    }
}
