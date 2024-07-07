using UnityEngine;

public struct Voxel
{
  public static Voxel EMPTY = new Voxel(Vector3.zero, VoxelType.AIR); 

  public Vector3 position;
  public VoxelType type;

  public Voxel(Vector3 position, VoxelType type)
  {
    this.position = position;
    this.type = type;
  }
}

public enum VoxelType
{
  AIR,
  STONE,
  GRASS,
  IRON_ORE,
  SILVER_ORE,
  GOLD_ORE
}