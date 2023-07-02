using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTextureCircle : MonoBehaviour
{
    public ComputeShader shader;

    public int texResolution = 1024;

    private Renderer rend;

    private RenderTexture outputTexture;

    private int circleHandle;
    private int clearHandle;
    
    public Color clearColor;
    public Color circleColor;
    
    // Start is called before the first frame update
    void Start()
    {
        outputTexture = new RenderTexture( texResolution, texResolution,0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
        
        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InitShader();
    }

    private void InitShader()
    {
        circleHandle = shader.FindKernel("Circles");
        clearHandle = shader.FindKernel("Clear");
        
        //Color is float4
        shader.SetTexture(circleHandle,"Result",outputTexture);
        shader.SetTexture(clearHandle,"Result",outputTexture);
        
        shader.SetInt("texResolution",texResolution);
        shader.SetVector("clearColor",clearColor);
        shader.SetVector("circleColor",circleColor);
        
        rend.material.SetTexture("_MainTex",outputTexture);
        
    }

    private void DispatchKernels(int count)
    {
        shader.Dispatch(clearHandle,texResolution/8,texResolution/8,1);
        shader.Dispatch(circleHandle,count,1,1);
    }
    
    // Update is called once per frame
    void Update()
    {
        DispatchKernels(10);
    }
    
    
}
