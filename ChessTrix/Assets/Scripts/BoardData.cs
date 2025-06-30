using UnityEngine;

[CreateAssetMenu(fileName = "Create New Board Colors", menuName = "Chess/BoardInfo")]
public class BoardData : ScriptableObject
{
    [SerializeField] public BoardColors black;
    [SerializeField] public BoardColors white;

    
    [System.Serializable]
    public struct BoardColors
    {
        public Color normal;
    }
    [Space(25)]
    public Color selected;
    public Color validMove;
}
