using System;
using System.Collections.Generic;
using System.Linq;
using BausChess.Utils;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Utilities.Collections;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{
    public class DragAndDropManager
    {
        private Vector2 _previousPosition;
        private ITileView? _sourceTile;
        private bool _isDrag;
        private long _startMousePress;
        private IPieceView? _selectedPiece;
        private readonly GameLogic _gameLogic;
        private readonly ViewManager _viewManager;
        
        public DragAndDropManager()
        {
        }

        public DragAndDropManager(GameLogic gameLogic, ViewManager viewManager)
        {
            _gameLogic = gameLogic;
            _viewManager = viewManager;
        }


        public void Update(MouseInfo mouseInfo, Vector2 startingPosition, int tileSize, BoardView boardView, int pieceSize, GameTime gameTime)
        {
            #region Detect click or drag
            bool isClick= false;
            
            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && _startMousePress == 0)
            {
                _startMousePress = gameTime.TotalGameTime.Seconds;
            }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Released &&
                mouseInfo.PreviousLeftButtonState == ButtonState.Pressed && gameTime.TotalGameTime.Seconds - _startMousePress < 1)
            {
                DebugHelper.ConsolePrint("Click");
                DebugHelper.ConsolePrint("Click at " + mouseInfo.CurrentPosition.X + " " + mouseInfo.CurrentPosition.Y);
                _startMousePress = 0;
                isClick = true;
            }
            
            if (mouseInfo.PreviousLeftButtonState == ButtonState.Pressed &&
                mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && gameTime.TotalGameTime.Seconds - 1 >= _startMousePress )
            {
                _isDrag = true;
                _selectedPiece = null;
                DebugHelper.ConsolePrint("Dragging");
            }

        
            #endregion
            
            IPieceView? markedPiece = _viewManager.FindPieceByMousePosition(mouseInfo.CurrentPosition);
            bool isPieceMarked = markedPiece != null;
            
            bool isValidPieceClicked = isPieceMarked &&
                ((_gameLogic.CurrentState == GameState.WHITEMOVE && markedPiece?.Color == XNAColor.White) ||
                 _gameLogic.CurrentState == GameState.BLACKMOVE && markedPiece?.Color == XNAColor.Black);
            
            if (isPieceMarked)
            {
                // DebugHelper.ConsolePrint("Marked Piece at " + markedPiece?.Position.X + " " + markedPiece?.Position.Y);
                // DebugHelper.ConsolePrint("Mouse at " + mouseInfo.CurrentPosition.X + " " + mouseInfo.CurrentPosition.Y);
            }
            
            #region Valid Move Hints
            if (isClick && isPieceMarked && isValidPieceClicked)
            {
                RemoveRedTiles(boardView);
                
                IList<ITileView> validTiles = boardView.FindValidTiles(markedPiece);
                foreach (ITileView tile in validTiles)
                {
                    tile.DisplayColor = XNAColor.Red;
                }
            }
            #endregion

            #region Valid tile clicked to move
            if (isClick && !isPieceMarked && _selectedPiece != null)
            {
                // check if target click is in valid tile
                IList<ITileView> validTiles = boardView.FindValidTiles(_selectedPiece);
                ITileView? tileForMove = _viewManager.FindTileByMousePosition(mouseInfo.CurrentPosition);
                
                // execute move
                if (tileForMove != null && validTiles.Contains(tileForMove))
                {
                    boardView.MakeMove(_selectedPiece, tileForMove);
                    _selectedPiece = null;
                    isClick = false;
                    _previousPosition = default;
                    RemoveRedTiles(boardView);
                }
            }
            #endregion
            
            #region Detect selected Piece
            if ((_isDrag || isClick) && _selectedPiece == null && isValidPieceClicked)
            {
                DebugHelper.ConsolePrint("Selected Piece at " + markedPiece?.Position.X + " " + markedPiece?.Position.Y);
                DebugHelper.ConsolePrint("Mouse at " + mouseInfo.CurrentPosition.X + " " + mouseInfo.CurrentPosition.Y);
                _selectedPiece = markedPiece;
            }
            #endregion
            
            #region Drag and Drop -> Piece Dropped
            if (_isDrag && _selectedPiece != null)
            {
                bool isPieceDropped = UpdateInner(boardView, mouseInfo, startingPosition, tileSize, pieceSize);
                    
                if (isPieceDropped)
                {
                    RemoveRedTiles(boardView);
                }
            }
            
            if (_isDrag && mouseInfo.PreviousLeftButtonState == ButtonState.Pressed &&
                mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {
                DebugHelper.ConsolePrint("Drag release");
                _startMousePress = 0;
                _isDrag = false;
                _selectedPiece = null;
                RemoveRedTiles(boardView);
            }
            #endregion
        }

        private bool UpdateInner(BoardView boardView, MouseInfo mouseInfo, Vector2 startingPosition, int tileSize, int pieceSize)
        {
            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed )
            {
                if (_previousPosition == default)
                {
                    _previousPosition = _selectedPiece.Position;
                }
                DebugHelper.ConsolePrint("Mouse Pressed at " + mouseInfo.CurrentPosition.X + " " + mouseInfo.CurrentPosition.Y);
                DebugHelper.ConsolePrint("Piece at " + _selectedPiece.Position.X + " " + _selectedPiece.Position.Y);
                
                _selectedPiece.Position = new Vector2(mouseInfo.CurrentPosition.X - pieceSize/2, mouseInfo.CurrentPosition.Y -pieceSize/2);

                if (_sourceTile == null)
                {
                    _sourceTile = _viewManager.FindTileByMousePosition(mouseInfo.CurrentPosition);
                }
            }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {
                if (!ViewUtils.IsInBoardRange(startingPosition, mouseInfo.CurrentPosition, tileSize))
                {
                    _selectedPiece.Position = _previousPosition;
                    _previousPosition = default;
                    return  false;
                }
            
                if (ViewUtils.IsValidPosition(boardView, startingPosition, mouseInfo.CurrentPosition, tileSize, _selectedPiece, pieceSize))
                {
                    _selectedPiece.Position = mouseInfo.CurrentPosition;
                    
                    ITileView targetTile = _viewManager.FindTileByMousePosition(mouseInfo.CurrentPosition);
                    boardView.MakeMove(_selectedPiece, targetTile);
            
                    _sourceTile.Piece = null;
                    _sourceTile = null;
                    _previousPosition = default;
                    return true;
                }else{
                    if (_previousPosition != default(Vector2))
                    {
                        _selectedPiece.Position = _previousPosition;
                        _previousPosition = default;
                    }
                    return false;
                }
            }

            return false;
        }

        private void RemoveRedTiles(BoardView boardView)
        {
            boardView.Tiles.Each(tile =>
            {
                if (tile.DisplayColor == XNAColor.Red)
                {
                    tile.DisplayColor = tile.TileColor;
                }
            });
        }

    }
    
}