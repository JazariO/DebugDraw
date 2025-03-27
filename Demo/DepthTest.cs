using UnityEngine;
using DebugDrawer;

public class DepthTest : MonoBehaviour
{
    public bool doDepthTest;
    public int renderQueueValue;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            doDepthTest = !doDepthTest;
            DebugDraw.SetDrawingDepthTestEnabled(doDepthTest);
        }
    }
}
