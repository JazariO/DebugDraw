using UnityEngine;
using Proselyte.DebugDrawer;

public class DebugDrawTest : MonoBehaviour
{
    public bool drawBox = true;
    public bool drawSphere = true;
    public bool drawWireSphere = true;
    public bool drawArrow = true;
    public bool drawCapsule = true;
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

    public Vector3 capsuleStartPosition;
    public Vector3 capsuleEndPosition;
    public float capsuleRadius;

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
            DebugDraw.WireArrow(arrowStartPosition, arrowEndPosition, Vector3.up, color: Color.red);
        }

        // Draw a wire capsule
        if(drawArrow)
        {
            DebugDraw.WireCapsule(point1: capsuleStartPosition, point2: capsuleEndPosition, radius: capsuleRadius, color: Color.red);
        }
    }

    private void FixedUpdate()
    {
        // Draw a wire capsule
        if(drawCapsule)
        {
            DebugDraw.WireCapsule(point1: capsuleStartPosition + Vector3.right, point2: capsuleEndPosition + Vector3.right, radius: capsuleRadius, color: Color.blue, fromFixedUpdate: true);
        }
    }

    private void OnValidate()
    {
        DebugDraw.SetEnabledLayers((uint)globalDebugLayer);
    }
}
