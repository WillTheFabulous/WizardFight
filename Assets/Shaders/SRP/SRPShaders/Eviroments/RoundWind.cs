using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoundWind : MonoBehaviour
{
    [SerializeField] public float MaxTime=5f;
    [SerializeField] public float Range=5f;
    public float time=0;
    [SerializeField] public bool loop;

    private void Awake()
    {
        time = MaxTime;
        Shader.SetGlobalVector(Shader.PropertyToID("_RoundWind"), new Vector4(transform.position.x, transform.position.y, transform.position.z, Range));
        Shader.SetGlobalFloat(Shader.PropertyToID("_RoundWindTime"),1- time/ MaxTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if(time<=0)
            {
                if (loop)
                {
                    time = MaxTime;
                }
                else
                {
                    time = 0;
                }
            }
            else
            {
                Shader.SetGlobalVector(Shader.PropertyToID("_RoundWind"), new Vector4(transform.position.x, transform.position.y, transform.position.z, Range));
                Shader.SetGlobalFloat(Shader.PropertyToID("_RoundWindTime"), time);
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
