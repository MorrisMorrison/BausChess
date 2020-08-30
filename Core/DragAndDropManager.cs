using System.Collections.Generic;
using BausChess.Utils;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BausChess.Core
{
    public class DragAndDropManager
    {

        IPieceView? SelectedPiece { get; set; }
        Vector2 PreviousPosition { get; set; }

        public DragAndDropManager()
        {
        }

        public void Update(MouseInfo mouseInfo, IList<IPieceView> pieces, Vector2 startingPosition, int tileSize, BoardView boardView, int pieceSize)
        {
            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && SelectedPiece == null)
            {
                foreach (IPieceView pieceView in pieces)
                {
                    if (mouseInfo.CurrentPosition.X.CheckRange(pieceView.Position.X, pieceView.Position.X + pieceView.Texture.Width)
                    && mouseInfo.CurrentPosition.Y.CheckRange(pieceView.Position.Y, pieceView.Position.Y + pieceView.Texture.Height))
                    {

                        SelectedPiece = pieceView;
                        PreviousPosition = pieceView.Position;
                        return;
                    }
                }
            }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && SelectedPiece != null)
            {
                SelectedPiece.Position = mouseInfo.CurrentPosition;
            }

            if (SelectedPiece != null && mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {

                if (!ViewUtils.IsInBoardRange(startingPosition, mouseInfo.CurrentPosition, tileSize))
                {
                    SelectedPiece.Position = PreviousPosition;
                    SelectedPiece = null;
                    return;
                }

                if (ViewUtils.IsValidPosition(boardView, startingPosition, mouseInfo.CurrentPosition, tileSize, SelectedPiece, pieceSize))
                {
                    // snap into tile
                    SelectedPiece.Position = mouseInfo.CurrentPosition;
                    TileView tile = ViewUtils.FindTileForMove(mouseInfo.CurrentPosition, boardView.Tiles, startingPosition, tileSize, pieceSize);
                    SelectedPiece.Position = ViewUtils.GetTileCenter( tile, tileSize, pieceSize);
                    SelectedPiece = null;
                    return;
                }else{
                    SelectedPiece.Position = PreviousPosition;
                    SelectedPiece = null;
                    return;
                }
            }
        }

    }
}