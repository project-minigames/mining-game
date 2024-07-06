using UnityEngine;

public struct ChunkPos
{
  public int x;
  public int z;

  public static ChunkPos ZERO = new ChunkPos { x = 0, z = 0 };
}

public class Chunk : MonoBehaviour
{
  private static int CHUNK_SIZE = 32;
  private static int CHUNK_HEIGHT = 64;
  private static float VOXEL_NOISE_THRESHOLD = 0.45F;

  public Environment environment;
  public ChunkPos pos;
  public Vector3 absolutePos;
  private Voxel?[,,] voxels;

  public void Init(Environment environment, ChunkPos pos) {
    this.environment = environment;
    this.pos = pos;
    this.absolutePos = new Vector3(pos.x * CHUNK_SIZE, 0, pos.z * CHUNK_SIZE);
    this.voxels = new Voxel?[32, 128, 32];
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

  public void CreateVoxels()
  {
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
            voxels[x, y, z] = new Voxel(absolutePos + position, Color.yellow);
            Instantiate(environment.stoneVoxel, position, Quaternion.identity);
          }
        }
      }
    }
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
