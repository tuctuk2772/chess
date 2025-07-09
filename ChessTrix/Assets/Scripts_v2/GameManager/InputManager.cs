using Chess;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    public static InputManager input { get; private set; }
    public InputSystem_Actions inputActions { get; private set; }
    public TrackMousePos trackMousePos { get; private set; }
    public SelectSquares selectSquares { get; private set; }
    public HoverSquares hoverSquares { get; private set; }

    [Header("Mouse Tracking Settings")]
    public LayerMask boardLayer;
    public Vector2Int squarePosition;

    private void Awake()
    {
        if (input != null && input != this)
        {
            Destroy(gameObject);
            return;
        }

        input = this;
        inputActions = new InputSystem_Actions();

        //create helper scripts
        trackMousePos = new TrackMousePos()
        {
            layerMask = boardLayer,
        };

        selectSquares = new SelectSquares()
        {
            squares = GetComponent<BoardManager>().squares
        };

        hoverSquares = new HoverSquares();

        //setup controls
        inputActions.Player.Interact.started += ctx => selectSquares.SelectSquare(squarePosition);
        inputActions.Player.Interact.canceled += ctx => selectSquares.StopDrag(squarePosition);
    }

    private void Start()
    {
        inputActions.Enable();
    }

    private void LateUpdate()
    {
        squarePosition = trackMousePos.UpdatePosition();
        Debug.DrawLine(Camera.main.transform.position, trackMousePos.DebugLinePosition(),
            selectSquares.dragged ? Color.green : Color.red);

        GameManager.instance.boardManager.hoveredSquare = hoverSquares.HoveredSquare(squarePosition);

        if (selectSquares.dragged)
        {
            selectSquares.MovePiece(trackMousePos.MousePosition(),
                GameManager.instance.boardManager.selectedSquare.transform);
            hoverSquares.ResetHover();
            return;
        }

        hoverSquares.UpdateHover(squarePosition, trackMousePos.MousePosition());
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
