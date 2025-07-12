using UnityEngine;
using DG.Tweening;

namespace Chess
{
    public class Juice_ObjectSpawning
    {
        private static readonly float SPAWN_PIECE_DUR = 1f;
        private static readonly float SPAWN_SQUARE_DUR = 0.75f;

        public static void SpawnPiece(ref GameObject piece, Vector3 scale, float delay = 0f)
        {
            piece.transform.DOScale(scale, SPAWN_PIECE_DUR).SetEase(Ease.OutElastic).
                SetId("spawn").
                SetDelay(delay);
        }

        public static void SpawnSquare(GameObject square, Vector3 scale, float delay = 0f)
        {
            square.transform.DOScale(scale, SPAWN_SQUARE_DUR).SetEase(Ease.OutElastic, 0.75f)
                .SetId("spawn").
                SetDelay(delay);
        }
    }
}
