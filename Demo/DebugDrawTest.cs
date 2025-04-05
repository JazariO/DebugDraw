using UnityEngine;
using DebugDrawer;

public class DebugDrawTest : MonoBehaviour
{
    public bool drawBox = true;
    public bool drawSphere = true;
    public bool drawWireSphere = true;
    public bool drawArrow = true;
    public bool toggleDepthTest = false;

    public Vector3 boxPosition = Vector3.zero;
    public Vector3 boxSize = Vector3.one;
    public Color boxColor = Color.green;

    public Vector3 wireSpherePosition = new Vector3(0, 1, 0);
    public Vector3 spherePosition = new Vector3(0, 1, 0);
    public float sphereRadius = 1f;
    public Color sphereColor = Color.yellow;

    public Vector3 arrowStartPosition;
    public Vector3 arrowEndPosition;
    public float arrowLength;

    public float duration = 0f;
    public DebugLayers debugLayer = DebugLayers.Layer1;
    public DebugLayers globalDebugLayer = DebugLayers.Layer1;

    private void Awake()
    {
        // Ensure DebugDraw Singleton is initialized
        var instance = DebugDraw.Instance;
    }

    private void Start()
    {
        DebugDraw.SetDrawingDepthTestEnabled(toggleDepthTest);
    }

    private void Update()
    {
        spherePosition = gameObject.transform.position;
        // Toggle depth test with a key press
        if(Input.GetKeyDown(KeyCode.T))
        {
            toggleDepthTest = !toggleDepthTest;
            DebugDraw.SetDrawingDepthTestEnabled(toggleDepthTest);
        }

        // Draw a debug box
        if(drawBox)
        {
            DebugDraw.Box(boxPosition, gameObject.transform.rotation, boxSize, boxColor, duration, (uint)debugLayer);
        }

        // Draw a debug solid sphere
        if(drawSphere)
        {
            DebugDraw.Sphere(spherePosition, gameObject.transform.rotation, sphereRadius, sphereColor, duration, (uint)debugLayer);
        }

        // Draw a wire sphere
        if(drawWireSphere)
        {
            DebugDraw.WireSphere(wireSpherePosition, gameObject.transform.rotation, sphereRadius, sphereColor, duration, (uint)debugLayer);
        }

        // Draw a wire arrow
        if(drawArrow)
        {
            DebugDraw.WireArrow(arrowStartPosition, arrowEndPosition, Vector3.up, arrowLength, Color.red);
        }
    }

    private void OnValidate()
    {
        DebugDraw.SetEnabledLayers((uint)globalDebugLayer);
    }
}
