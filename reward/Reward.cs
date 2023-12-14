using Godot;
using ProjectNostalgia.interfaces;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.reward;

public struct MeshData
{
    public readonly Mesh Mesh;
    public readonly Vector3 Scale;
    public readonly Vector3 RotationDegrees;

    public MeshData(Mesh mesh, Vector3 rotationDegrees, float scale = 1.0f)
    {
        Mesh = mesh;
        Scale = new Vector3(scale, scale, scale);
        RotationDegrees = rotationDegrees;
    }
}

public abstract class Reward : ISlottable
{

    public abstract void Acquire();

    public abstract Rarity GetRarity();

    public abstract MeshData GetMesh();

    public abstract Texture2D GetIcon();

    public abstract int GetAmount();
}