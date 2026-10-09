using System;
using System.Collections.Generic;
using Godot;

public partial class ObjectPoolWeaponController : Node
{
	private const int MaxInstances = 50;
	private const float DecalPixelSize = 0.001f;
	private const float SurfaceOffset = 0.01f;
	private const string SolidTexturePath = "res://Assets/Sprites/BulletHoles/Sprite_BulletHole_Solid.png";
	private const string BloodTexturePath = "res://Assets/Sprites/BulletHoles/Sprite_BulletHole_Blood.png";

	private readonly List<Sprite3D> _decals = new(MaxInstances);
	private readonly HashSet<Sprite3D> _activeDecals = new();
	private Node3D _decalParent;
	private Texture2D _decalTextureDefault;
	private Texture2D _decalTextureBlood;
	private int _currentIndex;
	private bool _isBloodVisible = true;
	private PauseSubMenuSettingsSectionGeneralController _settingsController;
	private GameScenesManager _gameSceneManager;

	public int MaxDecalCount => MaxInstances;

	public void Initialize(
		Bootstrap bootstrap,
		GameScenesManager gameSceneManager,
		PauseSubMenuSettingsSectionGeneralController settingsController)
	{
		ArgumentNullException.ThrowIfNull(bootstrap);
		_gameSceneManager = gameSceneManager ?? throw new ArgumentNullException(nameof(gameSceneManager));
		_settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));

		ObjectPoolWeaponList weaponPools = bootstrap.GameData?.GameObjectPoolsList?.ObjectPoolWeaponList;
		_decalTextureDefault = weaponPools?.BulletHoleSolid?.ObjectPoolTexture ?? GD.Load<Texture2D>(SolidTexturePath);
		_decalTextureBlood = weaponPools?.BulletHoleBlood?.ObjectPoolTexture ?? GD.Load<Texture2D>(BloodTexturePath);
		if (_decalTextureDefault == null || _decalTextureBlood == null)
		{
			GD.PushWarning("Bullet-hole decal texture(s) are not configured. Assign them in GameObjectPoolsList or add the default textures under Assets/Sprites/BulletHoles.");
		}

		_decalParent = new Node3D { Name = "Decals" };
		AddChild(_decalParent);
		RecreatePool();

		_gameSceneManager.OnBeginLoadingGameplayScene += RecreatePool;
		_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene += RecreatePool;
		_settingsController.OnShowBlood += ShowBloodDecals;
		_settingsController.OnHideBlood += HideBloodDecals;

		GD.Print("Weapon decal pool initialized.");
	}

	public override void _ExitTree()
	{
		if (_gameSceneManager != null)
		{
			_gameSceneManager.OnBeginLoadingGameplayScene -= RecreatePool;
			_gameSceneManager.OnBeginLoadingMainMenuOrEndGameTitlesScene -= RecreatePool;
		}
		if (_settingsController != null)
		{
			_settingsController.OnShowBlood -= ShowBloodDecals;
			_settingsController.OnHideBlood -= HideBloodDecals;
		}
	}

	public void RecreatePool()
	{
		foreach (Sprite3D decal in _decals)
		{
			if (GodotObject.IsInstanceValid(decal))
			{
				decal.Free();
			}
		}

		_decals.Clear();
		_activeDecals.Clear();
		_currentIndex = 0;
		if (_decalParent == null || !GodotObject.IsInstanceValid(_decalParent))
		{
			return;
		}

		for (int i = 0; i < MaxInstances; i++)
		{
			Sprite3D decal = new()
			{
				Name = $"Pooled_Decal_{i}",
				Texture = _decalTextureDefault,
				PixelSize = DecalPixelSize,
				Visible = false,
				Position = Vector3.Zero,
				Rotation = Vector3.Zero,
				Scale = Vector3.One
			};
			_decalParent.AddChild(decal);
			_decals.Add(decal);
		}
	}

	public void SpawnDecal(Vector3 position, Vector3 normal, bool isBloodTarget, Node3D parent = null)
	{
		if (_decals.Count == 0 || _decalTextureDefault == null || _decalTextureBlood == null)
		{
			return;
		}

		Sprite3D decal = _decals[_currentIndex];
		_currentIndex = (_currentIndex + 1) % _decals.Count;
		if (!GodotObject.IsInstanceValid(decal))
		{
			return;
		}

		Vector3 surfaceNormal = normal.LengthSquared() > 0.000001f ? normal.Normalized() : Vector3.Up;
		Vector3 tangent = surfaceNormal.Cross(Vector3.Up);
		if (tangent.LengthSquared() < 0.000001f)
		{
			tangent = surfaceNormal.Cross(Vector3.Right);
		}
		tangent = tangent.Normalized();
		Vector3 bitangent = surfaceNormal.Cross(tangent).Normalized();

		decal.Reparent(_decalParent, false);
		decal.GlobalPosition = position + surfaceNormal * SurfaceOffset;
		decal.GlobalBasis = new Basis(tangent, bitangent, surfaceNormal).Orthonormalized();
		decal.Texture = isBloodTarget ? _decalTextureBlood : _decalTextureDefault;
		decal.Visible = !isBloodTarget || _isBloodVisible;
		_activeDecals.Add(decal);

		if (parent != null && GodotObject.IsInstanceValid(parent) && parent != _decalParent)
		{
			decal.Reparent(parent, true);
		}
	}

	public void ReturnSpecificDecalsToPool(IEnumerable<Sprite3D> decalsToReturn)
	{
		if (decalsToReturn == null)
		{
			return;
		}

		foreach (Sprite3D decal in decalsToReturn)
		{
			if (decal == null || !GodotObject.IsInstanceValid(decal) || !_decals.Contains(decal))
			{
				continue;
			}

			decal.Reparent(_decalParent, false);
			decal.Position = Vector3.Zero;
			decal.Rotation = Vector3.Zero;
			decal.Scale = Vector3.One;
			decal.Visible = false;
			_activeDecals.Remove(decal);
		}
	}

	public void HideBloodDecals()
	{
		_isBloodVisible = false;
		SetBloodDecalVisibility(false);
	}

	public void ShowBloodDecals()
	{
		_isBloodVisible = true;
		SetBloodDecalVisibility(true);
	}

	private void SetBloodDecalVisibility(bool visible)
	{
		foreach (Sprite3D decal in _decals)
		{
			if (GodotObject.IsInstanceValid(decal) && _activeDecals.Contains(decal) && decal.Texture == _decalTextureBlood)
			{
				decal.Visible = visible;
			}
		}
	}
}