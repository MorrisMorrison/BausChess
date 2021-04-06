using System;
using System.Collections.Generic;
using BausChess.Utils;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BausChess.Core
{
    public class DragAndDropManager
    {

        public Vector2 PreviousPosition { get; set; }
        private TileView? SourceTile { get; set; }

        public DragAndDropManager()
        {
        }

        public bool Update(MouseInfo mouseInfo, IPieceView selectedPiece, Vector2 startingPosition, int tileSize, BoardView boardView, int pieceSize)
        {
            // if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && SelectedPiece == null)
            // {
            //     foreach (IPieceView pieceView in pieces)
            //     {
            //         if (mouseInfo.CurrentPosition.X.CheckRange(pieceView.Position.X, pieceView.Position.X + pieceView.Texture.Width)
            //         && mouseInfo.CurrentPosition.Y.CheckRange(pieceView.Position.Y, pieceView.Position.Y + pieceView.Texture.Height))
            //         {
            //             SelectedPiece = pieceView;
            //             PreviousPosition = pieceView.Position;
            //             return false;
            //         }
            //     }
            // }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed )
            {
                if (PreviousPosition == default)
                {
                    PreviousPosition = selectedPiece.Position;
                }
                // -16 + 12
                Console.WriteLine("Mouse Pressed at " + mouseInfo.CurrentPosition.X + " " + mouseInfo.CurrentPosition.Y);
                Console.WriteLine("Piece at " + selectedPiece.Position.X + " " + selectedPiece.Position.Y);
                
                
                // selectedPiece.Position = ViewUtils.ParsePosition(selectedPiece, boardView.Tiles, mouseInfo.CurrentPosition, tileSize, pieceSize);
                // selectedPiece.Position = mouseInfo.CurrentPosition;
                selectedPiece.Position = new Vector2(mouseInfo.CurrentPosition.X - pieceSize/2, mouseInfo.CurrentPosition.Y -pieceSize/2);

                // remember origin tile on first drag
                if (SourceTile == null)
                {
                    SourceTile = ViewUtils.FindTileForMove(mouseInfo.CurrentPosition, boardView.Tiles, startingPosition, tileSize, pieceSize);
                }
            }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {
                if (!ViewUtils.IsInBoardRange(startingPosition, mouseInfo.CurrentPosition, tileSize))
                {
                    selectedPiece.Position = PreviousPosition;
                    PreviousPosition = default;

                    return false;
                }
            
                if (ViewUtils.IsValidPosition(boardView, startingPosition, mouseInfo.CurrentPosition, tileSize, selectedPiece, pieceSize))
                {
                    // snap into tile
                    selectedPiece.Position = mouseInfo.CurrentPosition;
                    TileView targetTile = ViewUtils.FindTileForMove(mouseInfo.CurrentPosition, boardView.Tiles, startingPosition, tileSize, pieceSize);
                    selectedPiece.Position = ViewUtils.GetTileCenter( targetTile, tileSize, pieceSize);
            
                    SourceTile.Piece = null;
                    SourceTile = null;
                    PreviousPosition = default;

                    
                    return true;
                }else{
                    if (PreviousPosition != default(Vector2))
                    {
                        selectedPiece.Position = PreviousPosition;
                        PreviousPosition = default;
                    }
                    return false;
                }
            }


            return false;
        }

    }
}