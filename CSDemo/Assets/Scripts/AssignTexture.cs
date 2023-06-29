using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTexture : MonoBehaviour
{
    public ComputeShader shader;

    public int texResolution = 256;

    private Renderer rend;

    private RenderTexture outputTexture;

    private int kernelHandle;

    private int X, Y;

    // Start is called before the first frame update
    void Start()
    {
        outputTexture = new RenderTexture( texResolution, texResolution,0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
        X = 1; 
        Y = 1;
        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InitShader();
    }

    private void InitShader()
    {
        kernelHandle = shader.FindKernel("CSMain");
        shader.SetTexture(kernelHandle,"Result",outputTexture);
        shader.SetInt("kernelCount",(int)texResolution);
        rend.material.SetTexture("_MainTex",outputTexture);
        
        DispatchShader();
    }

    private void DispatchShader()
    {
        shader.SetInt("x",(int)X);
        shader.SetInt("y",(int)Y);
        shader.Dispatch(kernelHandle,X,Y,1);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            X++;
            DispatchShader();
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            Y++;
            DispatchShader();
        }
        
        if (Input.GetKeyUp(KeyCode.Z))
        {
            X--;
            if (X <= 0) X = 1;
            DispatchShader();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            Y--;
            if (Y <= 0) Y = 1;
            DispatchShader();
        }
    }
    
    
}
