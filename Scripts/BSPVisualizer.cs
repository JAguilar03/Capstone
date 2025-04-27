using UnityEngine;

public class BSPVisualizer : MonoBehaviour
{
    private BSPGenerator bspGenerator;

    void Start()
    {
        bspGenerator = GetComponent<BSPGenerator>();
    }

    void OnDrawGizmos()
    {
        if (bspGenerator == null || bspGenerator.rooms.Count == 0) return;

        // Draw rooms
        Gizmos.color = Color.green;
        foreach (Rect room in bspGenerator.rooms)
        {
            Gizmos.DrawWireCube(new Vector3(room.center.x, room.center.y, 0), new Vector3(room.width, room.height, 1));
        }

        // Draw corridors
        Gizmos.color = Color.yellow;
        foreach (Vector2Int corridor in bspGenerator.corridors)
        {
            Gizmos.DrawCube(new Vector3(corridor.x, corridor.y, 0), Vector3.one);
        }
    }
}
