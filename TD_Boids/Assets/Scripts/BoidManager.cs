using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using System.Threading.Tasks;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;
    int threadGroups;

    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;
    ComputeBuffer boidBuffer;
    BoidData[] boidData;
    [SerializeField] private List<Transform> targets;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();

        foreach (Boid b in boids)
        {
            b.Initialize(settings, targets[Random.Range(0, targets.Count)]);
        }

        int numBoids = boids.Length;
        boidData = new BoidData[numBoids];
        boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);

        threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);

        compute.SetInt("numBoids", numBoids);
        compute.SetFloat("viewRadius", settings.perceptionRadius);
        compute.SetFloat("avoidRadius", settings.avoidanceRadius);
    }

    void Update() // TODO -> CREATE THE BOID DATA AND BUFFER IN START SO I CAN USE IT AGAIN AT NEXT FRAME
    {
        if (boids != null)
        {
            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < numBoids; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }


            boidBuffer.SetData(boidData);
            compute.SetBuffer(0, "boids", boidBuffer);

            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);


            Parallel.For(0, numBoids, i =>
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;
            }
            );

            for (int i = 0; i < numBoids; ++i)
            {
                boids[i].UpdateBoid();
            }

            // for (int i = 0; i < numBoids; ++i)
            // {
            //     boids[i].avgFlockHeading = boidData[i].flockHeading;
            //     boids[i].centreOfFlockmates = boidData[i].flockCentre;
            //     boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
            //     boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

            //     boids[i].UpdateBoid();
            // }
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

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}