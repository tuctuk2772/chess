using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Chess;

namespace Chess
{
    public class Juice_Square
    {
        public static BoardManager boardManager => GameManager.instance.boardManager;

        private static readonly float SQUARE_ANIM_DEFAULT_DUR = 0.15f;
        private static readonly float SQUARE_ANIM_QUICK_MARKED_DUR = 0.1f;

        //these need to be public for piece juice
        public static readonly float SQUARE_MOVE_MULTIPLIER = 0.25f;
        public static readonly float SQUARE_MOVE_DEFAULT_SPEED = 0.75f;
        public static float squareTotalMoveSpeed;

        public static void ChangeSquareColor(GameObject square, Color selectedColor, bool notDefaultColor)
        {
            Material mat = square.GetComponent<SpriteRenderer>().material;

            int notDefault = notDefaultColor ? 1 : 0;

            mat.SetInt("_ChangeColor", notDefault);
            mat.DOColor(selectedColor, "_HighlightColor", SQUARE_ANIM_DEFAULT_DUR).SetEase(Ease.OutQuint);
        }

        public static void MarkSquare(List<SquareInfo> markedSquares)
        {
            foreach (var markedSquare in markedSquares)
            {
                if (boardManager.validCaptureSquares.Contains(markedSquare))
                {
                    markedSquare.circle.DOFade(1f, SQUARE_ANIM_QUICK_MARKED_DUR);
                    if (!boardManager.clickAgain)
                    {
                        Juice_Piece.ShakePiece(markedSquare.pieceGameObject, markedSquare.transform.position);
                    }
                    else
                    {
                        DOTween.Kill("shake");
                        Juice_Piece.UnShakePiece(markedSquare.pieceGameObject, markedSquare.transform.position);
                    }
                }

                if (boardManager.validEmptySquares.Contains(markedSquare))
                {
                    markedSquare.dot.DOFade(1f, SQUARE_ANIM_DEFAULT_DUR);
                }
            }
        }

        public static void UnMarkSquare(List<SquareInfo> markedSquares)
        {
            foreach (var markedSquare in markedSquares)
            {
                if (boardManager.validCaptureSquares.Contains(markedSquare))
                {
                    markedSquare.circle.DOFade(0f, SQUARE_ANIM_QUICK_MARKED_DUR);
                    continue;
                }

                if (boardManager.validEmptySquares.Contains(markedSquare))
                {
                    markedSquare.dot.DOFade(0f, SQUARE_ANIM_DEFAULT_DUR);
                }
            }
        }

        public static void SquareHover(GameObject square, Color hoverColor)
        {
            SpriteRenderer squareSprite = square.GetComponent<SpriteRenderer>();
            SquareInfo squareInfo = square.GetComponent<SquareInfo>();

            ChangeSquareColor(square, hoverColor, true);
            squareSprite.sortingOrder = 1;

            if (boardManager.allValidSquares == null || !boardManager.allValidSquares.Contains(squareInfo))
            {
                return;
            }

            if (boardManager.validEmptySquares != null && boardManager.validEmptySquares.Contains(squareInfo))
            {
                squareInfo.dot.DOFade(0, SQUARE_ANIM_QUICK_MARKED_DUR);
            }

            if (boardManager.validCaptureSquares != null && boardManager.validCaptureSquares.Contains(squareInfo))
            {
                squareInfo.circle.DOFade(0, SQUARE_ANIM_QUICK_MARKED_DUR);
            }
        }

        public static void SquareUnHover(GameObject square)
        {
            SpriteRenderer squareSprite = square.GetComponent<SpriteRenderer>();
            SquareInfo squareInfo = square.GetComponent<SquareInfo>();

            ChangeSquareColor(square, squareSprite.material.GetColor("_DefaultColor"), false);
            squareSprite.sortingOrder = 0;

            if (boardManager.allValidSquares == null || !boardManager.allValidSquares.Contains(squareInfo))
            {
                return;
            }

            if (boardManager.validEmptySquares != null && boardManager.validEmptySquares.Contains(squareInfo))
            {
                squareInfo.dot.DOFade(1, SQUARE_ANIM_QUICK_MARKED_DUR);
            }

            if (boardManager.validCaptureSquares != null && boardManager.validCaptureSquares.Contains(squareInfo))
            {
                Juice_Piece.PieceUnFade(ref squareInfo.pieceGameObject);
                squareInfo.circle.DOFade(1, SQUARE_ANIM_QUICK_MARKED_DUR);
            }
        }

        public static void SquareMoveDirection(List<SquareInfo> allSquares, System.Action onComplete,
            Vector2Int direction)
        {
            float slowdown = SQUARE_MOVE_MULTIPLIER * Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            int completedCount = 0;
            int totalSequences = allSquares.Count;

            squareTotalMoveSpeed = SQUARE_MOVE_DEFAULT_SPEED + slowdown;

            bool hasCompleted = false;

            foreach (SquareInfo square in allSquares)
            {
                Material squareMat = square.GetComponent<SpriteRenderer>().material;
                squareMat.SetVector("_Direction", (Vector2)direction);

                Color previousCol = squareMat.GetColor("_DefaultColor");
                Color newCol = squareMat.GetColor("_NewColor");

                var moveTween = DOTween.To(
                    () => squareMat.GetFloat("_ClampedTime"),
                    x => squareMat.SetFloat("_ClampedTime", x),
                    1f, //clamped value between 0 and 1
                    squareTotalMoveSpeed
                    ).SetEase(Ease.Linear);

                var sequence = DOTween.Sequence().SetId("moveSquares");

                sequence.Append(moveTween);

                sequence.AppendCallback(() =>
                {
                    squareMat.SetFloat("_ClampedTime", 0f);

                    if (SwitchSquareColor(direction))
                    {
                        squareMat.SetColor("_DefaultColor", newCol);
                        squareMat.SetColor("_NewColor", previousCol);
                    }
                });

                sequence.AppendInterval(slowdown);

                sequence.OnComplete(() =>
                {
                    completedCount++;
                    if (!hasCompleted && completedCount >= totalSequences)
                    {
                        hasCompleted = true;
                        onComplete.Invoke();
                    }
                });
            }
        }

        private static bool SwitchSquareColor(Vector2Int direction)
        {
            int x = direction.x;
            int y = direction.y;

            if ((x == 0 && Mathf.Abs(y) % 2 == 1) || (y == 0 && Mathf.Abs(x) % 2 == 1))
                return true;

            if ((Mathf.Abs(x) % 2 == 1 && Mathf.Abs(y) % 2 == 0 && y != 0) ||
                (Mathf.Abs(y) % 2 == 1 && Mathf.Abs(x) % 2 == 0 && x != 0))
                return true;

            return false;
        }
    }
}