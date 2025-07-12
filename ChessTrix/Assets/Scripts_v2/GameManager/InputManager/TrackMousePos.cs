using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackMousePos
{
    public LayerMask layerMask;
    public Vector2Int tilePosition = new Vector2Int(-1, -1);

    [HideInInspector] public Vector2 offset;
    [HideInInspector] public Vector2 tileSize;
    [HideInInspector] public Vector2Int boardSize;

    private Vector3 lastWorldMousePos;

    public Vector2Int UpdatePosition()
    {
        RaycastHit2D ray = Physics2D.Raycast(MousePosition(), Vector2.zero, Mathf.Infinity, layerMask);

        if (ray.collider != null)
        {
            Vector2 localPos = ray.point - offset;

            int col = Mathf.FloorToInt(localPos.x / tileSize.x);
            int row = Mathf.FloorToInt(localPos.y / tileSize.y);

            if (col >= 0 && col < boardSize.x && row >= 0 && row < boardSize.y)
            {
                return tilePosition = new Vector2Int(col, row);
            }
            else
            {
                return tilePosition = new Vector2Int(-1, -1);
            }
        }
        else
        {
            return tilePosition = new Vector2Int(-1, -1);
        }
    }

    public Vector3 DebugLinePosition()
    {
        Vector2 mousePos = InputManager.input.inputActions.Player.Move.ReadValue<Vector2>();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    public Vector3 MousePosition()
    {
        Vector2 mousePos = InputManager.input.inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        worldPoint.z = 0f;

        return worldPoint;
    }

    public Vector3 MouseVelocity()
    {
        Vector3 currentPos = MousePosition();
        Vector3 velocity = (currentPos - lastWorldMousePos) / Time.deltaTime;
        lastWorldMousePos = currentPos;

        Vector3 clampVelocity = Vector3.ClampMagnitude(velocity, 35f);
        if (tilePosition.x < 0 || tilePosition.x > 7)
        {
            return Vector3.zero;
        }

        return clampVelocity;
    }

    public Vector3 ClampToBoard(Vector3 position)
    {
        float minX = offset.x;
        float maxX = offset.x + tileSize.x * boardSize.x;

        float minY = offset.y;
        float maxY = offset.y + tileSize.y * boardSize.y;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        position.z = 0;

        return position;
    }
}
