using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BufferJoy : MonoBehaviour
{
    public ComputeShader shader;

    public int texResolution = 1024;

    private Renderer rend;

    private RenderTexture outputTexture;

    private int circleHandle;
    private int clearHandle;

    struct Circle
    {
        public Vector2 origin;
        public Vector2 velocity;
        public float radius;
    }

    private int count = 10;
    private Circle[] circleData;
    private ComputeBuffer buffer;
    
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

        InitData();
        
        InitShader();
    }

    private void InitData()
    {
        circleHandle = shader.FindKernel("Circles");

        uint threadGroupSizeX;
        //返回kernelSize([numthreads(32,1,1)] 对应 x,y,z)
        shader.GetKernelThreadGroupSizes(circleHandle,out threadGroupSizeX,
            out _,out _);

        int total = (int)threadGroupSizeX * count;
        circleData = new Circle[total];

        float speed = 100;
        float halfSpeed = speed * 0.5f;
        float minRadius = 10.0f;
        float maxRadius = 30.0f;
        float radiusRange = maxRadius - minRadius;

        for (int i = 0; i < total; ++i)
        {
            Circle circle = circleData[i];
            circle.origin.x = Random.value * texResolution;
            circle.origin.y = Random.value * texResolution;
            circle.velocity.x = (Random.value * speed) - halfSpeed;
            circle.velocity.y = (Random.value * speed) - halfSpeed;
            circle.radius = Random.value * radiusRange + minRadius;

            circleData[i] = circle;
        }
        
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

        int stride = UnsafeUtility.SizeOf<Circle>();
        buffer = new ComputeBuffer(circleData.Length, stride);
        buffer.SetData(circleData);
        shader.SetBuffer(circleHandle,"circlesBuffer",buffer);
        
    }

    private void DispatchKernels(int count)
    {
        shader.Dispatch(clearHandle,texResolution/8,texResolution/8,1);
        shader.SetFloat("time",Time.time);
        shader.Dispatch(circleHandle,count,1,1);
    }
    
    // Update is called once per frame
    void Update()
    {
        shader.SetVector("clearColor",clearColor);
        shader.SetVector("circleColor",circleColor);
        DispatchKernels(10);
    }
}
