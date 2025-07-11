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

    public bool isBeingHeld;

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
            squares = GetComponent<BoardManager>().allSquares
        };

        hoverSquares = new HoverSquares()
        {
            boardManager = GameManager.instance.boardManager,
        };

        //setup controls
        inputActions.Player.Interact.started += ctx =>
        {
            isBeingHeld = true;
            selectSquares.SelectSquare(squarePosition);
        };
        inputActions.Player.Interact.canceled += ctx =>
        {
            isBeingHeld = false;
            if (selectSquares.movedOutsideBounds)
            {
                selectSquares.StopDrag(squarePosition);
            }
            else if (GameManager.instance.boardManager.currentSelectedSquare ==
                     GameManager.instance.boardManager.previousSelectedSquare)
            {
                selectSquares.DeSelectSquare(squarePosition);
            }
            else
            {
                selectSquares.dragged = false;
            }
        };
    }

    private void Start()
    {
        inputActions.Enable();
    }

    private void LateUpdate()
    {
        //always have hovering being tracked
        squarePosition = trackMousePos.UpdatePosition();
        Debug.DrawLine(Camera.main.transform.position, trackMousePos.DebugLinePosition(),
            selectSquares.dragged ? Color.green : Color.red);

        if (isBeingHeld && selectSquares.dragged && GameManager.instance.boardManager.currentSelectedSquare != null)
        {
            selectSquares.DragPiece(trackMousePos.MousePosition(),
                GameManager.instance.boardManager.currentSelectedSquare.transform);
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
