using System;
using Godot;

/// <summary>Scene-tree helpers used by bootstrap view models.</summary>
public static class GodotNodeExtensions
{
    public static T FindNodeOfType<T>(this Node node) where T : Node
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node is T typedNode)
        {
            return typedNode;
        }

        foreach (Node child in node.GetChildren())
        {
            T match = child.FindNodeOfType<T>();
            if (match is not null)
            {
                return match;
            }
        }

        throw new InvalidOperationException($"Node '{node.GetPath()}' does not contain a {typeof(T).Name} node.");
    }

    public static T FindDeepNodeOfType<T>(this Node node) where T : Node
    {
        if (node is T typedNode)
        {
            return typedNode;
        }

        foreach (Node child in node.GetChildren())
        {
            T match = child.FindDeepNodeOfType<T>();
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    public static Node FindDeepNode(this Node root, string targetName)
    {
        ArgumentNullException.ThrowIfNull(root);

        if (root.Name == targetName)
        {
            return root;
        }

        foreach (Node child in root.GetChildren())
        {
            Node match = child.FindDeepNode(targetName);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    public static Node InstantiateGodot(this Node parent, PackedScene scene)
    {
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(scene);

        Node instance = scene.Instantiate();
        parent.AddChild(instance);
        return instance;
    }

    public static T CreateChildNode<T>(this Node parent) where T : Node, new()
    {
        ArgumentNullException.ThrowIfNull(parent);
        T child = new();
        parent.AddChild(child);
        return child;
    }

    public static void SetVisible(this Node node, bool visible)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node is CanvasItem canvasItem)
        {
            canvasItem.Visible = visible;
        }
    }

    public static void SetActive(this Node node, bool active)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node is CanvasItem canvasItem)
        {
            canvasItem.Visible = active;
        }

        node.ProcessMode = active ? Node.ProcessModeEnum.Inherit : Node.ProcessModeEnum.Disabled;
    }
}