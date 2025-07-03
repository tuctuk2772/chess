using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPieceModifier", menuName = "Chess/PieceModifier")]
public class PieceModifiers : ScriptableObject
{
    public enum Rarity
    {
        Common, Uncommon, Rare
    }

    public string personName;

    [TextArea(3, 10)]
    public string description;

    [Space(10)]
    public Rarity rarity;

    public Sprite spriteSheet;
}