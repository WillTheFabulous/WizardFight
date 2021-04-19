using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
	public Gradient TopBackColor;
	public Gradient TopFrontColor;
	public Gradient BottomBackColor;
	public Gradient BottomFrontColor;
    public AnimationCurve TopBottomRange;
	public Gradient HorizonColor;
    public AnimationCurve HorizonRange;
    public AnimationCurve HorizonIntensity;
    public Gradient SunColor;
    public AnimationCurve SunRange;
    public AnimationCurve SunIntensity;
    public AnimationCurve TotalColorLightness;
	public Material mat;
	[Range(0,1)]public float TimeOfDay;
    public bool On;
	

    void Start()
    {
        
    }

    void Update()
    {
        if(On){
        mat.SetColor("_Color0",TopBackColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color1",TopFrontColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color2",BottomBackColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color3",BottomFrontColor.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter1",TopBottomRange.Evaluate(TimeOfDay));
        mat.SetColor("_Color4",HorizonColor.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter2",HorizonRange.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter3",HorizonIntensity.Evaluate(TimeOfDay));
        mat.SetColor("_Color5",SunColor.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter4",SunRange.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter5",SunIntensity.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter6",TotalColorLightness.Evaluate(TimeOfDay));
    }
    }
}
