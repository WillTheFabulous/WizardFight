using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class Reflection : MonoBehaviour
{
    [SerializeField] GameObject ReflectionCamera;
    [SerializeField] Transform Plane;
    [SerializeField] RenderTexture ReflectionTex;
    //[SerializeField] Text text;
    int frameNum;
    float time;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Plane != null)
        {

            if (ReflectionTex == null)
            {
                ReflectionTex = RenderTexture.GetTemporary(Screen.width/4, Screen.height/4,8);
            }

            if (ReflectionCamera == null)
            {
                ReflectionCamera = new GameObject();
                ReflectionCamera.AddComponent<Camera>();
                ReflectionCamera.GetComponent<Camera>().targetTexture = ReflectionTex;
            }
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //time += Time.deltaTime;
        //frameNum++;
        //if (time > 1f)
        //{
         //   time -= 1f;
         //   text.text = frameNum.ToString();
        //    frameNum = 0;

        //}
        if (Plane != null)
        {
            ReflectionCamera.transform.position = new Vector3(transform.position.x, 2 * Plane.position.y - transform.position.y, transform.position.z);
            ReflectionCamera.transform.rotation = Quaternion.Euler( -Vector3.Reflect(transform.rotation.eulerAngles, Vector3.up));
            ReflectionCamera.GetComponent<Camera>().fieldOfView = GetComponent<Camera>().fieldOfView;
            Shader.SetGlobalTexture("_ReflectionTex", ReflectionTex);
        }
    }
}
