using UnityEngine;
using DebugDrawer;

public class DebugDrawLineTest : MonoBehaviour
{
    [SerializeField] private Transform startTF;
    [SerializeField] private Transform endTF;
    [SerializeField] private Color color;

    private void Update()
    {
        DebugDraw.Line(startTF.position, endTF.position, color);
    }
}
