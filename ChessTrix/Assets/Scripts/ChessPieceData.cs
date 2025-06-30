using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[System.Serializable]
public class ChessPieceData
{
    public enum PieceColor { White, Black }
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
    public enum MovementType { Horizontal, Vertical, Diagonal, LShape, SingleSpace, PawnSpecific }

    //actual variables
    public string PieceName => $"{pieceColor} {pieceType}";
    public Sprite pieceSprite;
    public PieceColor pieceColor;
    public PieceType pieceType;
    public float pieceValue; //for scoring, based on Turing values:
    /*
    - Pawn = 1
    - Knight = 3
    - Bishop = 3.5
    - Rook = 5
    - Queen = 10
    */
    public List<MovementType> movements = new List<MovementType>();
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ChessPieceData))]
public class ChessPieceDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Build name from pieceColor and pieceType
        var colorProp = property.FindPropertyRelative("pieceColor");
        var typeProp = property.FindPropertyRelative("pieceType");

        string title = $"{colorProp.enumDisplayNames[colorProp.enumValueIndex]} {typeProp.enumDisplayNames[typeProp.enumValueIndex]}";

        EditorGUI.PropertyField(position, property, new GUIContent(title), true);
    }
}
#endif

public class RuntimeChessPieceData
{
    public ChessPieceData.PieceColor pieceColor;
    public ChessPieceData.PieceType pieceType;
    public Sprite pieceSprite;
    public List<ChessPieceData.MovementType> movements;
    public float pieceValue;
    public int moveAmount; //mainly for pawn, but might need for castling
}