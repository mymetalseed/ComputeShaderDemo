using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class OrbitingStars : MonoBehaviour
{
    public int starCount = 17;
    public ComputeShader shader;
    public GameObject prefab;

    private ComputeBuffer resultBuffer;
    private int kernelHandle;
    private uint threadGroupSizeX;
    private int groupSizeX;
    private Vector3[] output;
    private Transform[] stars;
    
    
    // Start is called before the first frame update
    void Start()
    {
        kernelHandle = shader.FindKernel("OrbitingStars");
        shader.GetKernelThreadGroupSizes(kernelHandle,out threadGroupSizeX,out _,out _);
        groupSizeX = (int)((starCount + threadGroupSizeX - 1) / threadGroupSizeX);
        
        //buffer on the gpu in the ram
        resultBuffer = new ComputeBuffer(starCount, UnsafeUtility.SizeOf<Vector3>());
        shader.SetBuffer(kernelHandle,"Result",resultBuffer);
        output = new Vector3[starCount];
        
        //star we use for visual
        stars = new Transform[starCount];
        for (int i = 0; i < starCount; ++i)
        {
            stars[i] = Instantiate(prefab, transform).transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shader.SetFloat("time",Time.time);
        shader.Dispatch(kernelHandle,groupSizeX,1,1);
        resultBuffer.GetData(output);

        for (int i = 0; i < stars.Length; ++i)
        {
            stars[i].localPosition = output[i];
        }
    }

    private void OnDestroy()
    {
        resultBuffer.Dispose();
    }
}
