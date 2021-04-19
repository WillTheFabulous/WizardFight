using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorClouds : MonoBehaviour
{
	public Gradient BackDarkColor;
	public Gradient FrontDarkColor;
	public Gradient BackLightColor;
	public Gradient FrontLightColor;
    public AnimationCurve FrontLighten;
    public AnimationCurve TexLightAreaLighten;
	public Gradient RimColor;
    public AnimationCurve RimRange;
    public AnimationCurve RimIntensity;
    public AnimationCurve NoiseIntensity;
	public Material mat;
	protected float TimeOfDay;
    private  ChangeColor changeColor;
    public bool On;
	

    void Update()
    {
        if(changeColor==null){
            changeColor=this.gameObject.GetComponent<ChangeColor>();
        }
        if(On){
            TimeOfDay = changeColor.TimeOfDay;
        mat.SetColor("_Color0",BackDarkColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color1",FrontDarkColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color2",BackLightColor.Evaluate(TimeOfDay));
        mat.SetColor("_Color3",FrontLightColor.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter7",FrontLighten.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter6",TexLightAreaLighten.Evaluate(TimeOfDay));
        mat.SetColor("_Color4",RimColor.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter1",RimRange.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter2",RimIntensity.Evaluate(TimeOfDay));
        mat.SetFloat("_parameter8",NoiseIntensity.Evaluate(TimeOfDay));
    }
    }
}
