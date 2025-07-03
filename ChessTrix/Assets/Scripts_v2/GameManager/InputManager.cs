using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager input { get; private set; }
    public InputSystem_Actions inputActions { get; private set; }
    public TrackMousePos trackMousePos { get; private set; }

    [Header("Mouse Tracking Settings")]
    public LayerMask boardLayer;

    private void Awake()
    {
        if (input != null && input != this)
        {
            Destroy(gameObject);
            return;
        }

        input = this;
        inputActions = new InputSystem_Actions();

        //track mouse
        trackMousePos = new TrackMousePos()
        {
            layerMask = boardLayer,
        };
    }

    private void Start()
    {
        inputActions.Enable();
    }

    private void Update()
    {
        trackMousePos.UpdatePosition();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
