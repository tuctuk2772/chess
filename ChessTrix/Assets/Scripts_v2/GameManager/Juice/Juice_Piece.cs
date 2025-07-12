using UnityEngine;
using DG.Tweening;

namespace Chess
{
    public class Juice_Piece
    {
        private static readonly float PIECE_DEFAULT_SCALE = 0.05f;
        private static readonly float PIECE_GROW_SCALE = 0.065f;

        private static readonly float PIECE_ANIM_DEFAULT_DUR = 0.15f;

        private static readonly float PIECE_ROTATE_AMOUNT = 75f;
        private static readonly float PIECE_ROTATE_DUR = 0.25f;

        private static readonly float PIECE_RETURN_DUR = 0.5f;

        private static readonly float PIECE_SHAKE_AMOUNT = 1.5f;
        private static readonly float PIECE_SHAKE_DUR = 0.05f;

        public static void PieceFade(ref GameObject piece)
        {
            piece.GetComponent<SpriteRenderer>().DOFade(0f, PIECE_ANIM_DEFAULT_DUR);
        }

        public static void PieceUnFade(ref GameObject piece)
        {
            piece.GetComponent<SpriteRenderer>().DOFade(1f, PIECE_ANIM_DEFAULT_DUR);
        }

        public static void PieceHover(ref GameObject piece, Vector3 mousePosition)
        {
            piece.transform.DOLocalRotate(
                new Vector3(mousePosition.y * PIECE_ROTATE_AMOUNT, mousePosition.x * PIECE_ROTATE_AMOUNT, 0),
                PIECE_ROTATE_DUR)
                .SetId("hover");
        }

        public static void PieceUnHover(ref GameObject piece)
        {
            DOTween.Kill("hover");

            piece.transform.DOScale(PIECE_DEFAULT_SCALE, PIECE_ANIM_DEFAULT_DUR)
                .SetId("hover");
            piece.transform.DORotate(Vector3.zero, PIECE_ANIM_DEFAULT_DUR)
                .SetId("hover");
        }

        public static void MovePieceAround(ref GameObject piece, ref Vector3 clampedPosition, Vector3 mouseVelocity)
        {
            piece.GetComponent<SpriteRenderer>().sortingOrder = 5;

            piece.transform.DOScale(PIECE_GROW_SCALE, PIECE_ANIM_DEFAULT_DUR)
                .SetId("move");
            piece.transform.DORotate(new Vector3(0, 0, mouseVelocity.x), PIECE_ANIM_DEFAULT_DUR)
                .SetId("move");

            piece.transform.DOMove(clampedPosition, 0f).SetId("move");
        }

        public static void ReturnPiece(GameObject piece)
        {
            DOTween.Kill("move");

            piece.transform.DOScale(PIECE_DEFAULT_SCALE, PIECE_RETURN_DUR);
            piece.transform.DORotate(Vector3.zero, PIECE_RETURN_DUR); //for some reason i need this
            piece.transform.DOLocalMove(new Vector3(0, 0, -1), PIECE_RETURN_DUR).SetEase(Ease.OutBounce).
                OnComplete(() =>
            {
                piece.GetComponent<SpriteRenderer>().sortingOrder = 3;
            });
        }

        public static void SnapPieceToSquare(ref GameObject piece, Vector2 position)
        {
            piece.transform.position = new Vector3(position.x, position.y, 0f);
            piece.transform.localScale = new Vector3(PIECE_DEFAULT_SCALE, PIECE_DEFAULT_SCALE, 1f);
            piece.transform.localRotation = Quaternion.identity;
        }

        public static void TapPiece(GameObject piece)
        {
            DOTween.Kill("move");

            piece.transform.DOScale(PIECE_GROW_SCALE, PIECE_ANIM_DEFAULT_DUR).SetEase(Ease.OutElastic).
                OnComplete(() =>
                {
                    piece.transform.DOScale(PIECE_DEFAULT_SCALE, PIECE_ANIM_DEFAULT_DUR);
                });
        }

        public static void ShakePiece(GameObject piece, Vector3 originalPosition)
        {
            piece.transform.position = originalPosition;

            DOTween.Kill(piece);

            piece.transform.DOShakePosition(PIECE_SHAKE_AMOUNT, PIECE_SHAKE_DUR)
                .SetId(piece)
                .SetId("shake")
                .OnComplete(() =>
                {
                    UnShakePiece(piece, originalPosition);
                });
        }

        public static void UnShakePiece(GameObject piece, Vector3 originalPosition)
        {
            piece.transform.DOMove(originalPosition, PIECE_ANIM_DEFAULT_DUR);
        }

        public static void PieceMoveDirection(GameObject piece,
            Vector2Int direction, Vector2Int pieceOffset,
            System.Action onComplete)
        {
            Vector2 duration = CalculateSpeed(ref direction, ref pieceOffset);
            SpriteRenderer pieceSr = piece.GetComponent<SpriteRenderer>();

            DOTween.Kill("hover");
            PieceUnHover(ref piece);

            Sequence movePiece = DOTween.Sequence();

            pieceSr.sortingOrder = 5;

            movePiece.Append(piece.transform.DOLocalMoveX(pieceOffset.x, duration.x).SetEase(Ease.Linear));
            movePiece.Join(piece.transform.DOLocalMoveY(pieceOffset.y, duration.y).SetEase(Ease.Linear));

            movePiece.OnComplete(() =>
            {
                pieceSr.sortingOrder = 3;
                onComplete?.Invoke();
            });
        }

        private static Vector2 CalculateSpeed(ref Vector2Int direction, ref Vector2Int offset)
        {
            float speedX = 0f;
            float speedY = 0f;

            if (direction.x != 0)
            {
                speedX = (Juice_Square.squareTotalMoveSpeed / direction.x) * offset.x;
            }

            if (direction.y != 0)
            {
                speedY = (Juice_Square.squareTotalMoveSpeed / direction.y) * offset.y;
            }

            return new Vector2(speedX, speedY);
        }
    }
}
