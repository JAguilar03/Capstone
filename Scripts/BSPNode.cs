using UnityEngine;

/// <summary>
///  Represents a node in the Binary Space Partitioning tree.
/// </summary>
public class BSPNode
{
    public Rect Area;      // The space this node represents
    public BSPNode Left;   // Left child
    public BSPNode Right;  // Right child
    public Rect Room;      // The actual room inside this partition

    /// <summary>
    ///  Constructor for the BSPNode.
    /// </summary>
    /// <param name="area">The rectangular area that the node represents.</param>
    public BSPNode(Rect area)
    {
        Area = area;
        Left = null;
        Right = null;
    }

    /// <summary>
    ///  Checks if this node is a leaf node (has no children).
    /// </summary>
    /// <returns>True if the node is a leaf node, false otherwise.</returns>
    public bool IsLeaf()
    {
        return Left == null && Right == null;
    }
}
