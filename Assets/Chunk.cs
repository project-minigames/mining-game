using System.Collections.Generic;
using UnityEngine;

public struct ChunkPos
{
  public int x;
  public int z;

  public static ChunkPos ZERO = new ChunkPos { x = 0, z = 0 };
}

public class Chunk : MonoBehaviour
{
  public static readonly int CHUNK_SIZE = 32;
  public static readonly int CHUNK_HEIGHT = 64;
  private static float VOXEL_NOISE_THRESHOLD = 0.45F;

  public Environment environment;
  public ChunkPos pos;
  public Vector3 absolutePos;
  private Voxel?[,,] voxels = new Voxel?[CHUNK_SIZE, CHUNK_HEIGHT, CHUNK_SIZE];

  /* voxel meshes */
  private MeshFilter meshFilter;
  private MeshRenderer meshRenderer;
  private MeshCollider meshCollider;

  private List<Vector3> vertices = new List<Vector3>();
  private List<int> triangles = new List<int>();
  private List<Vector2> uvs = new List<Vector2>();

  public Voxel getVoxelRelative(int x, int y, int z)
  {
    if (voxels[x & 31, y, z & 31] is Voxel voxel)
    {
      return voxel;
    }

    return Voxel.EMPTY;
  }

  public void Init(Environment environment, ChunkPos pos)
  {
    this.environment = environment;
    this.pos = pos;
    this.absolutePos = new Vector3(pos.x * CHUNK_SIZE, 0, pos.z * CHUNK_SIZE);

    meshFilter = gameObject.AddComponent<MeshFilter>();
    meshRenderer = gameObject.AddComponent<MeshRenderer>();
    meshCollider = gameObject.AddComponent<MeshCollider>();

    /* generate the voxels in this chunk and create a mesh */
    CreateVoxels();
    ConstructMesh();
  }

  /*
    void OnDrawGizmos()
    {
      if (voxels != null)
      {
        for (int x = 0; x < CHUNK_SIZE; x++)
        {
          for (int y = 0; y < CHUNK_HEIGHT; y++)
          {
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
              var value = voxels[x, y, z];
              if (value is Voxel voxel)
              {
                Gizmos.color = voxel.color;
                Gizmos.DrawCube(transform.position + new Vector3(x, y, z), Vector3.one);
              }
            }
          }
        }
      }
    }
    */

  void ConstructMesh()
  {
    for (int x = 0; x < CHUNK_SIZE; x++)
    {
      for (int y = 0; y < CHUNK_HEIGHT; y++)
      {
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
          ProcessVoxel(x, y, z);
        }
      }
    }

    Mesh mesh = new Mesh
    {
      vertices = vertices.ToArray(),
      triangles = triangles.ToArray(),
      uv = uvs.ToArray()
    };

    mesh.RecalculateNormals(); // Important for lighting

    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;

    // Apply a material or texture if needed
    meshRenderer.material = Resources.Load("Materials/stone", typeof(Material)) as Material;
  }

  private void CreateVoxels()
  {
    Debug.Log("(" + pos.x + ", " + pos.z + ")");

    for (int x = 0; x < CHUNK_SIZE; x++)
    {
      for (int y = 0; y < CHUNK_HEIGHT; y++)
      {
        for (int z = 0; z < CHUNK_SIZE; z++)
        {
          float xCoord = ((pos.x * CHUNK_SIZE) + x) / 12F;
          float yCord = y / 24F;
          float zCoord = ((pos.z * CHUNK_SIZE) + z) / 12F;
          float noiseValue = PerlinNoise3D(xCoord, yCord, zCoord);
          //           Debug.Log(noiseValue);

          if (noiseValue > VOXEL_NOISE_THRESHOLD)
          {
            var position = new Vector3(x, y, z);
            var newVoxel = new Voxel(absolutePos + position, VoxelType.STONE);

            voxels[x, y, z] = newVoxel;
            //             Instantiate(environment.stoneVoxel, position, Quaternion.identity);
          }
        }
      }
    }
  }

  private void ProcessVoxel(int x, int y, int z)
  {
    // Check if the voxels array is initialized and the indices are within bounds
    if (voxels == null || x < 0 || x >= voxels.GetLength(0) ||
        y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2))
    {
      return; // Skip processing if the array is not initialized or indices are out of bounds
    }

    if (voxels[x, y, z] != null)
    {
      // Check each face of the voxel for visibility
      bool[] facesVisible = new bool[6];

      // Check visibility for each face
      facesVisible[0] = IsFaceVisible(x, y + 1, z); // Top
      facesVisible[1] = IsFaceVisible(x, y - 1, z); // Bottom
      facesVisible[2] = IsFaceVisible(x - 1, y, z); // Left
      facesVisible[3] = IsFaceVisible(x + 1, y, z); // Right
      facesVisible[4] = IsFaceVisible(x, y, z + 1); // Front
      facesVisible[5] = IsFaceVisible(x, y, z - 1); // Back

      for (int i = 0; i < facesVisible.Length; i++)
      {
        if (facesVisible[i])
          AddFaceData(x, y, z, i); // Method to add mesh data for the visible face
      }
    }
  }

  private void AddFaceData(int x, int y, int z, int faceIndex)
  {
    // Based on faceIndex, determine vertices and triangles
    // Add vertices and triangles for the visible face
    // Calculate and add corresponding UVs

    if (faceIndex == 0) // Top Face
    {
      vertices.Add(new Vector3(x, y + 1, z));
      vertices.Add(new Vector3(x, y + 1, z + 1));
      vertices.Add(new Vector3(x + 1, y + 1, z + 1));
      vertices.Add(new Vector3(x + 1, y + 1, z));
      uvs.Add(new Vector2(0, 0));
      uvs.Add(new Vector2(1, 0));
      uvs.Add(new Vector2(1, 1));
      uvs.Add(new Vector2(0, 1));
    }

    if (faceIndex == 1) // Bottom Face
    {
      vertices.Add(new Vector3(x, y, z));
      vertices.Add(new Vector3(x + 1, y, z));
      vertices.Add(new Vector3(x + 1, y, z + 1));
      vertices.Add(new Vector3(x, y, z + 1));
      uvs.Add(new Vector2(0, 0));
      uvs.Add(new Vector2(0, 1));
      uvs.Add(new Vector2(1, 1));
      uvs.Add(new Vector2(1, 0));
    }

    if (faceIndex == 2) // Left Face
    {
      vertices.Add(new Vector3(x, y, z));
      vertices.Add(new Vector3(x, y, z + 1));
      vertices.Add(new Vector3(x, y + 1, z + 1));
      vertices.Add(new Vector3(x, y + 1, z));
      uvs.Add(new Vector2(0, 0));
      uvs.Add(new Vector2(0, 0));
      uvs.Add(new Vector2(0, 1));
      uvs.Add(new Vector2(0, 1));
    }

    if (faceIndex == 3) // Right Face
    {
      vertices.Add(new Vector3(x + 1, y, z + 1));
      vertices.Add(new Vector3(x + 1, y, z));
      vertices.Add(new Vector3(x + 1, y + 1, z));
      vertices.Add(new Vector3(x + 1, y + 1, z + 1));
      uvs.Add(new Vector2(1, 0));
      uvs.Add(new Vector2(1, 1));
      uvs.Add(new Vector2(1, 1));
      uvs.Add(new Vector2(1, 0));
    }

    if (faceIndex == 4) // Front Face
    {
      vertices.Add(new Vector3(x, y, z + 1));
      vertices.Add(new Vector3(x + 1, y, z + 1));
      vertices.Add(new Vector3(x + 1, y + 1, z + 1));
      vertices.Add(new Vector3(x, y + 1, z + 1));
      uvs.Add(new Vector2(0, 1));
      uvs.Add(new Vector2(0, 1));
      uvs.Add(new Vector2(1, 1));
      uvs.Add(new Vector2(1, 1));
    }

    if (faceIndex == 5) // Back Face
    {
      vertices.Add(new Vector3(x + 1, y, z));
      vertices.Add(new Vector3(x, y, z));
      vertices.Add(new Vector3(x, y + 1, z));
      vertices.Add(new Vector3(x + 1, y + 1, z));
      uvs.Add(new Vector2(0, 0));
      uvs.Add(new Vector2(1, 0));
      uvs.Add(new Vector2(1, 0));
      uvs.Add(new Vector2(0, 0));

    }

    AddTriangleIndices();
  }

  private void AddTriangleIndices()
  {
    int vertCount = vertices.Count;

    // First triangle
    triangles.Add(vertCount - 4);
    triangles.Add(vertCount - 3);
    triangles.Add(vertCount - 2);

    // Second triangle
    triangles.Add(vertCount - 4);
    triangles.Add(vertCount - 2);
    triangles.Add(vertCount - 1);
  }

  private bool IsFaceVisible(int x, int y, int z)
  {
    if (x < 0 || x >= CHUNK_SIZE || y < 0 || y >= CHUNK_HEIGHT || z < 0 || z >= CHUNK_SIZE)
    {
      /* this face is at the boundary of a chunk */
      return true;
    }

    return voxels[x, y, z] == null;
  }

  public static float PerlinNoise3D(float x, float y, float z)
  {
    float xy = Mathf.PerlinNoise(x, y);
    float xz = Mathf.PerlinNoise(x, z);
    float yz = Mathf.PerlinNoise(y, z);
    float yx = Mathf.PerlinNoise(y, x);
    float zx = Mathf.PerlinNoise(z, x);
    float zy = Mathf.PerlinNoise(z, y);

    return (xy + xz + yz + yx + zx + zy) / 6;
  }
}
