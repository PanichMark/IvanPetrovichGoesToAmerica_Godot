using System;
using System.Collections.Generic;
using Godot;

/// <summary>Rebinds Godot Skeleton3D-backed meshes to a shared player skeleton by bone name.</summary>
public partial class TransferSkinnedMeshRendererArmatureBones : Node
{
	[Export] private Node3D _baseArmatureRoot;
	[Export] private MeshInstance3D _meshTorso;
	[Export] private bool _baseMeshTorso;
	[Export] private Godot.Collections.Array<MeshInstance3D> _meshesLimbs = new();

	private Skeleton3D _baseSkeleton;
	private readonly Dictionary<string, int> _boneIndices = new(StringComparer.OrdinalIgnoreCase);

	public override void _Ready()
	{
		_baseSkeleton = _baseArmatureRoot?.FindDeepNodeOfType<Skeleton3D>();
		if (_baseSkeleton == null)
		{
			GD.PushWarning($"[{nameof(TransferSkinnedMeshRendererArmatureBones)}] Base Skeleton3D is not assigned.");
			return;
		}

		for (int index = 0; index < _baseSkeleton.GetBoneCount(); index++)
			_boneIndices[_baseSkeleton.GetBoneName(index).Trim()] = index;

		if (_meshTorso != null)
			RebindMesh(_meshTorso, allowMeshDuplication: _baseMeshTorso);

		foreach (MeshInstance3D mesh in _meshesLimbs)
		{
			if (mesh != null)
				RebindMesh(mesh, allowMeshDuplication: false);
		}
	}

	private void RebindMesh(MeshInstance3D mesh, bool allowMeshDuplication)
	{
		if (_baseSkeleton == null)
			return;

		if (allowMeshDuplication && mesh.Mesh != null)
			mesh.Mesh = mesh.Mesh.Duplicate() as Mesh;

		if (mesh.Skin != null)
		{
			Skin reboundSkin = mesh.Skin.Duplicate() as Skin;
			for (int bindIndex = 0; bindIndex < reboundSkin.GetBindCount(); bindIndex++)
			{
				string boneName = reboundSkin.GetBindName(bindIndex).Trim();
				if (_boneIndices.TryGetValue(boneName, out int boneIndex))
					reboundSkin.SetBindBone(bindIndex, boneIndex);
				else
					GD.PushWarning($"[{nameof(TransferSkinnedMeshRendererArmatureBones)}] Bone '{boneName}' not found on base skeleton.");
			}
			mesh.Skin = reboundSkin;
		}

		mesh.Skeleton = mesh.GetPathTo(_baseSkeleton);
	}

	public bool TryGetBaseBone(string boneName, out int boneIndex)
	{
		return _boneIndices.TryGetValue(boneName?.Trim() ?? string.Empty, out boneIndex);
	}

	public bool RebindWeaponMesh(MeshInstance3D weaponMesh)
	{
		if (weaponMesh == null || _baseSkeleton == null)
			return false;

		RebindMesh(weaponMesh, allowMeshDuplication: false);
		return true;
	}
}