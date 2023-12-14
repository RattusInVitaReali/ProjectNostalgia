using Godot;
using ProjectNostalgia.item;
using ProjectNostalgia.reward;
using ProjectNostalgia.utils;

namespace ProjectNostalgia.ground_object.ground_reward;

public partial class GroundReward : GroundObject
{
    private static PackedScene _groundRewardScene = GD.Load<PackedScene>("res://ground_object/ground_reward/ground_reward.tscn");
    private static QuadMesh _iconMesh = GD.Load<QuadMesh>("res://ground_object/ground_reward/ground_reward_icon.tres");

    private static StandardMaterial3D _particleMaterial = GD.Load<StandardMaterial3D>("res://ground_object/ground_reward/ground_reward_particle_material.tres");
    
    private Reward _reward;

    public static GroundReward FromReward(Reward reward)
    {
        GroundReward groundReward = _groundRewardScene.Instantiate<GroundReward>();
        groundReward._reward = reward;
        return groundReward;
    }

    public override void _Ready()
    {
        MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");

        MeshData rewardMesh = _reward.GetMesh();
        if (rewardMesh.Mesh != null)
        {
            mesh.Mesh = rewardMesh.Mesh;
            mesh.RotationDegrees = rewardMesh.RotationDegrees;
            mesh.Scale = rewardMesh.Scale;
        }
        else
        {
            QuadMesh iconMesh = (QuadMesh)_iconMesh.Duplicate(true);
            iconMesh.Material.Set("albedo_texture", _reward.GetIcon());
            mesh.Mesh = iconMesh;
        }

        Color particleColor = RarityUtils.GetColor(_reward.GetRarity());
        SetParticleColor(particleColor);
        SetLightColor(particleColor);
    }

    private void SetParticleColor(Color color)
    {
        GpuParticles3D particles = GetNode<GpuParticles3D>("GPUParticles3D");
        ParticleProcessMaterial material = (ParticleProcessMaterial)particles.ProcessMaterial;
        GradientTexture1D colorRamp = (GradientTexture1D)material.ColorRamp;
        Color edgeColor = RarityUtils.GetColor(_reward.GetRarity()) with { A = 0 };
        Color middleColor = RarityUtils.GetColor(_reward.GetRarity());
        Gradient gradient = new Gradient();
        gradient.SetColor(0, edgeColor);
        gradient.SetColor(1, edgeColor);
        gradient.AddPoint(0.5f, middleColor);
        gradient.AddPoint(0.2f, middleColor);
        colorRamp.Gradient = gradient;

        QuadMesh drawPassMesh = (QuadMesh)particles.DrawPass1;
        StandardMaterial3D drawPassMaterial = (StandardMaterial3D)_particleMaterial.Duplicate(true);
        drawPassMaterial.Emission = color;
        drawPassMesh.Material = drawPassMaterial;
    }

    private void SetLightColor(Color color)
    {
        OmniLight3D light = GetNode<OmniLight3D>("OmniLight3D");
        light.LightColor = color;
    }
    
    public override void PickUp()
    {
        _reward.Acquire();
        QueueFree();
    }
}