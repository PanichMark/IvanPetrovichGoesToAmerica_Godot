using System;
using System.Collections.Generic;
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly Dictionary<ServiceLocatorGameObjectsEnum, Godot.Node> _nodes = new();
    private static readonly Dictionary<ServiceLocatorAudioSourcesEnum, Godot.AudioStreamPlayer> _audioPlayers = new();

	public static void Register<T>(object instance)
	{
		var type = typeof(T);

		// ПРОВЕРКА: Не затираем ли мы старый сервис новым?
		if (_services.ContainsKey(type))
		{
			//Debug.LogError($"[SL] OVERWRITING existing service '{type.Name}'. Old Instance ID: {_services[type].GetHashCode()}, New Instance ID: {instance.GetHashCode()}");
		}

		//Debug.Log($"[SL] REGISTERED: '{type.Name}' | Hash: {instance.GetHashCode()}");
		_services[type] = instance;
	}

    public static void Register(ServiceLocatorGameObjectsEnum tag, Godot.Node node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (_nodes.ContainsKey(tag))
        {
            throw new InvalidOperationException($"Node for {tag} is already registered.");
        }
        _nodes[tag] = node;
    }

    public static void Register(ServiceLocatorAudioSourcesEnum key, Godot.AudioStreamPlayer source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (_audioPlayers.ContainsKey(key))
        {
            throw new InvalidOperationException($"Audio player for {key} is already registered.");
        }
        _audioPlayers[key] = source;
    }

    public static T Resolve<T>()
    {
        var type = typeof(T);
        if (!_services.TryGetValue(type, out var result))
        {
            throw new KeyNotFoundException($"Contract '{type.Name}' not found");
        }
        return (T)result;
    }

    public static Godot.Node Resolve(ServiceLocatorGameObjectsEnum tag)
    {
        if (!_nodes.TryGetValue(tag, out var node) || !Godot.GodotObject.IsInstanceValid(node))
        {
            throw new KeyNotFoundException($"Node for {tag} was not registered or is no longer valid.");
        }
        return node;
    }

    public static Godot.AudioStreamPlayer Resolve(ServiceLocatorAudioSourcesEnum key)
    {
        if (!_audioPlayers.TryGetValue(key, out var source) || !Godot.GodotObject.IsInstanceValid(source))
        {
            throw new KeyNotFoundException($"Audio player for {key} was not registered or is no longer valid.");
        }
        return source;
    }

    public static T GetNode<T>(ServiceLocatorGameObjectsEnum tag) where T : Godot.Node
    {
        var node = Resolve(tag);
        if (node is T typedNode)
        {
            return typedNode;
        }

        return FindDescendant<T>(node)
            ?? throw new InvalidCastException($"Registered node for {tag} is not or does not contain a {typeof(T).Name} node.");
    }

    private static T FindDescendant<T>(Godot.Node parent) where T : Godot.Node
    {
        foreach (Godot.Node child in parent.GetChildren())
        {
            if (child is T match)
            {
                return match;
            }

            T nestedMatch = FindDescendant<T>(child);
            if (nestedMatch is not null)
            {
                return nestedMatch;
            }
        }

        return null;
    }

    public static void ClearServices()
    {
        _services.Clear();
    }

    public static void ClearGameObjects()
    {
        _nodes.Clear();
    }

    public static void ClearAudioSources()
    {
        _audioPlayers.Clear();
    }

    public static void ClearAllServices()
    {
        ClearServices();
        ClearGameObjects();
        ClearAudioSources();
    }

    public static bool Remove<T>()
{
    var type = typeof(T);
    return _services.Remove(type);
}
}