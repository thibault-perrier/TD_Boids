using Unity.Entities;
using Unity.Mathematics;

public struct BoidManager
{
    public float cohesionBias;
    public float separationBias;
    public float alignmentBias;
    public float targetBias;
    public float perceptionRadius;
    public float step;
    public int cellSize;
    public float fieldOfView;
    public int maxPercived;
}

public struct BoidManagerBLOB
{
    public BlobArray<BoidManager> blobManagerArray;
}