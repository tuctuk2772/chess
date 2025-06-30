using UnityEngine;

public class TrackMousePos : MonoBehaviour
{
    public LayerMask layerMask;
    public Vector2Int tilePosition = Vector2Int.zero;
    [HideInInspector] public Vector2 offset;
    [HideInInspector] public Vector2 tileSize;

    private void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, layerMask);

        Debug.DrawLine(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);

        if(ray.collider != null)
        {
            Vector2 localPos = ray.point - offset;

            int col = Mathf.FloorToInt(localPos.x / tileSize.x + 0.5f);
            int row = Mathf.FloorToInt(localPos.y / tileSize.y + 0.5f);
            tilePosition = new Vector2Int(col, row);
        } else
        {
            tilePosition = new Vector2Int(-1, -1);
        }
    }
}
