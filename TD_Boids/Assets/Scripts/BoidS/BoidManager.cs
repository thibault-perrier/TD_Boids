using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using System.Threading.Tasks;
using System;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    const int threadGroupSize = 1024;
    int threadGroups;

    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;
    ComputeBuffer boidBuffer;
    BoidData[] boidData;
    public Dictionary<int, BoidData> AliveBoidData = new Dictionary<int, BoidData>();
    [SerializeField] private List<Transform> targets;

    [SerializeField] private int _waveBoidNum = 0;

    void Awake()
    {
        if (Instance != null) Instance = this;
    }

    void Start()
    {
        boids = FindObjectsOfType<Boid>();

        // Parallel.For(0, boids.Length, i =>
        // {
        //     boids[i].Initialize(settings, targets[i % targets.Count]);
        // });

        foreach (Boid b in boids)
        {
            b.Initialize(settings, targets[UnityEngine.Random.Range(0, targets.Count)]);
        }

        int numBoids = boids.Length;
        boidData = new BoidData[numBoids];
        boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);

        threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);

        compute.SetInt("numBoids", numBoids);
        compute.SetFloat("viewRadius", settings.perceptionRadius);
        compute.SetFloat("avoidRadius", settings.avoidanceRadius);
    }

    // void Update() // TODO -> CREATE THE BOID DATA AND BUFFER IN START SO I CAN USE IT AGAIN AT NEXT FRAME
    // {
    //     if (boids != null)
    //     {
    //         int numBoids = boids.Length;
    //         Array.Clear(boidData, 0, boidData.Length);

    //         for (int i = 0; i < numBoids; i++)
    //         {
    //             boidData[i].position = boids[i].position;
    //             boidData[i].direction = boids[i].forward;
    //             boidData[i].ToRender = 1;
    //             boids[i].gameObject.SetActive(i < _waveBoidNum);
    //         }

    //         boidBuffer.SetData(boidData);
    //         compute.SetBuffer(0, "boids", boidBuffer);

    //         compute.Dispatch(0, threadGroups, 1, 1);

    //         boidBuffer.GetData(boidData);

    //         for (int i = 0; i < numBoids; ++i)
    //         {
    //             boids[i].avgFlockHeading = boidData[i].flockHeading;
    //             boids[i].centreOfFlockmates = boidData[i].flockCentre;
    //             boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
    //             boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

    //             boids[i].UpdateBoid();
    //         }
    //     }
    // }

    void Update()
    {
        if (boids != null)
        {
            int numBoids = boids.Length;

            for (int i = 0; i < numBoids; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;
                boids[i].UpdateBoid();

                // if (i == 0) Array.Clear(boidData, 0, boidData.Length);

                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
                boidData[i].flockHeading = default;
                boidData[i].flockCentre = default;
                boidData[i].avoidanceHeading = default;
                boidData[i].numFlockmates = default;
                boidData[i].ToRender = 1;
                boids[i].gameObject.SetActive(i < _waveBoidNum);
            }

            boidBuffer.SetData(boidData);
            compute.SetBuffer(0, "boids", boidBuffer);

            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);
        }
    }

    void OnDestroy()
    {
        if (boidBuffer != null)
            boidBuffer.Release();
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;
        public int ToRender;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int) * 2;
            }
        }
    }
}