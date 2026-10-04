using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Godot;

/// <summary>
/// Portable save envelope. Gameplay systems can persist their own versioned payloads
/// in GameplayObjectData without requiring save-layer references to gameplay assemblies.
/// </summary>
public sealed class JsonGameData
{
	public string SafeFileDateAndTime { get; set; } = DateTime.Now.ToString("O");
	public GameScenesGameplayEnum Scene { get; set; } = GameScenesGameplayEnum.Scene_System_Test;
	public MissionData MissionData { get; set; } = new();
	public PlayerBehaviourData PlayerBehaviour { get; set; } = new();
	public PlayerMovementData PlayerMovement { get; set; } = new();
	public PlayerCameraData PlayerCamera { get; set; } = new();
	public PlayerResourcesData PlayerResources { get; set; } = new();
	public List<string> PlayerKeys { get; set; } = new();
	public PlayerWeaponsData PlayerWeapons { get; set; } = new();
	public Dictionary<string, JsonObject> GameplayObjectData { get; set; } = new(StringComparer.Ordinal);
	[JsonExtensionData]
	public Dictionary<string, JsonElement> LegacyGameplayData { get; set; } = new(StringComparer.OrdinalIgnoreCase);

	public JsonGameData()
	{
		PlayerMovement.PlayerPosition = new Vector3(0f, 0f, -5f);
		PlayerMovement.PlayerRotation = Quaternion.Identity;
		PlayerCamera.PlayerCameraDistanceY = -1.75f;
		PlayerCamera.PlayerCameraDistanceZ = 3.25f;
		PlayerCamera.PlayerCameraRotation = Quaternion.Identity;
		PlayerResources.PlayerHealth = 80f;
		PlayerResources.PlayerMana = 80f;
	}
}

public sealed class MissionData
{
	public float Mission { get; set; }
	public int MissionStep { get; set; }
}

public sealed class PlayerBehaviourData
{
	public bool IsPlayerArmed { get; set; }
	public bool WasPlayerArmed { get; set; }
	public int TimesPlayerSpottedGameTotal { get; set; }
	public int PeopleKilledGameTotal { get; set; }
}

public sealed class PlayerMovementData
{
	public Vector3 PlayerPosition { get; set; }
	public Quaternion PlayerRotation { get; set; }
	public string PlayerMovementStateType { get; set; } = string.Empty;
}

public sealed class PlayerCameraData
{
	public float PLayerCameraDistanceY { get; set; }
	public float PlayerCameraDistanceZ { get; set; }
	public Quaternion PlayerCameraRotation { get; set; }
	public string PlayerCameraStateType { get; set; } = string.Empty;
	public bool IsPlayerCameraShoulderRight { get; set; } = true;
}

public sealed class PlayerResourcesData
{
	public float PlayerHealth { get; set; } = 80f;
	public int PlayerHealingItemsNumber { get; set; }
	public float PlayerMana { get; set; } = 80f;
	public int PlayerManaReplenishItemsNumber { get; set; }
	public int PlayerMoney { get; set; }
}

public sealed class PlayerWeaponsData
{
	public List<string> UnlockedPlayerWeapons { get; set; } = new();
	public string PlayerWeaponRightHand { get; set; }
	public string PlayerWeaponLeftHand { get; set; }
}