using UnityEngine;
using UnityEngine.InputSystem;

public class TrackMousePos
{
    public LayerMask layerMask;
    public Vector2Int tilePosition = Vector2Int.zero;

    [HideInInspector] public Vector2 offset;
    [HideInInspector] public Vector2 tileSize;
    [HideInInspector] public Vector2Int boardSize;

    public void UpdatePosition()
    {
        Vector2 mousePos = InputManager.input.inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        worldPoint.z = 0f;

        RaycastHit2D ray = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, layerMask);
        Debug.DrawLine(Camera.main.transform.position, worldPoint, Color.red);

        if (ray.collider != null)
        {
            Vector2 localPos = ray.point - offset;

            int col = Mathf.FloorToInt(localPos.x / tileSize.x);
            int row = Mathf.FloorToInt(localPos.y / tileSize.y);

            if (col >= 0 && col < boardSize.x && row >= 0 && row < boardSize.y)
            {
                tilePosition = new Vector2Int(col, row);
            }
            else
            {
                tilePosition = new Vector2Int(-1, -1);
            }
        }
        else
        {
            tilePosition = new Vector2Int(-1, -1);
        }
    }
}
