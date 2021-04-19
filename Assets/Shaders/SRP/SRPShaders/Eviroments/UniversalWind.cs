using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(WindZone))]
public class UniversalWind : MonoBehaviour
{
    WindZone wind;
    private void Awake()
    {
        Shader.SetGlobalVector("RoundForce0", new Vector4(0, 0, 0, 1));
        Shader.SetGlobalVector("RoundForce0Setting", new Vector4(0, 0, 1, 1));
        wind = GetComponent<WindZone>();
    }
    

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_WindVector", new Vector4(transform.forward.x, transform.forward.y, transform.forward.z, wind.windMain));
    }
}
