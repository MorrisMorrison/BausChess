using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAColor = Microsoft.Xna.Framework.Color;
using ChessEngineColor = ChessEngine.Core.Color;
using System.Linq;
using System;
using System.Diagnostics;
using BausChess.Utils;
using BausChess.Core;
using Utilities.Collections;

namespace BausChess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BoardView _boardView { get; set; }
        private MouseInfo _mouseInfo { get; set; }
        private DragAndDropManager _dragAndDropManager { get; set; }
        private int _screenHeight = 692;
        private int _screenWidth = 1024;
        private int _tileSize = 64;
        private int _pieceSize = 32;
        private Vector2 _startPosition;
        private GameLogic _gameLogic;
        bool isDrag = false;

        private int _mousePressCounter;
        private long _startMousePress;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _mouseInfo = new MouseInfo();
            _dragAndDropManager = new DragAndDropManager();
            _startPosition = new Vector2((_screenWidth - (8 * _tileSize)) / 2, (_screenHeight - (8 * _tileSize)) / 2);
            _boardView = new BoardView(_startPosition, 64, 32);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _boardView.Initialize(Content);
            _gameLogic = new GameLogic();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // LoadPieces();
            // LoadPiecesPositions();
        }

        /*private void LoadPiecesPositions()
        {
            foreach(IPieceView piece in _boardView.Pieces){
                piece.Position = ViewUtils.ParsePosition(piece, _boardView.Tiles, _startPosition, _tileSize, _pieceSize);
            }
        }*/

        private bool queenPositionPrinted = false;
        private IPieceView? _selectedPiece;
        
        protected override void Update(GameTime gameTime)
        {
            _mouseInfo.Update(Mouse.GetState());

            DebugHelper.DebugPrint($@"Mouse at X:{Mouse.GetState().X} Y:{Mouse.GetState().Y}");

            if (!queenPositionPrinted)
            {
                IPieceView queen = _boardView.Pieces.FirstOrDefault(piece => piece.Color == XNAColor.White && piece.Piece.Type == PieceType.QUEEN);
                DebugHelper.ConsolePrint($@"Queen at X:{queen.Position.X} Y:{queen.Position.Y}");
                queenPositionPrinted = true;
            }
            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            #region Detect click or drag
            bool isClick= false;
            
            if (_mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && _startMousePress == 0)
            {
                _startMousePress = gameTime.TotalGameTime.Seconds;
            }

            if (_mouseInfo.CurrentLeftButtonState == ButtonState.Released &&
                _mouseInfo.PreviousLeftButtonState == ButtonState.Pressed && gameTime.TotalGameTime.Seconds - _startMousePress < 1)
            {
                DebugHelper.ConsolePrint("Click");
                DebugHelper.ConsolePrint("Click at " + _mouseInfo.CurrentPosition.X + " " + _mouseInfo.CurrentPosition.Y);
                _startMousePress = 0;
                isClick = true;
            }
            
            if (_mouseInfo.PreviousLeftButtonState == ButtonState.Pressed &&
                _mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && gameTime.TotalGameTime.Seconds - 1 >= _startMousePress )
            {
                isDrag = true;
                DebugHelper.ConsolePrint("Dragging");
            }

        
            #endregion
            
            IPieceView? markedPiece = ViewUtils.GetSelectedPiece(_boardView.Pieces, _mouseInfo.CurrentPosition);
            bool isPieceMarked = markedPiece != null;
            if (isPieceMarked)
            {
                // DebugHelper.ConsolePrint("Marked Piece at " + markedPiece?.Position.X + " " + markedPiece?.Position.Y);
                // DebugHelper.ConsolePrint("Mouse at " + _mouseInfo.CurrentPosition.X + " " + _mouseInfo.CurrentPosition.Y);
            }
            
            if (isClick && isPieceMarked)
            {
                bool isValidPieceClicked =
                    ((_gameLogic.CurrentState == GameState.WHITEMOVE && markedPiece?.Color == XNAColor.White) ||
                     _gameLogic.CurrentState == GameState.BLACKMOVE && markedPiece?.Color == XNAColor.Black);
                IList<Coordinates> validMoves = _boardView.Board.FindValidMoves(markedPiece?.Piece);
                IList<TileView> tiles = ViewUtils.FindTilesForMoves(
                    validMoves.Select(coordinates => new Move(markedPiece.Piece, coordinates)).ToList(),
                    _boardView.Tiles, _startPosition, _tileSize, _pieceSize);
                foreach (TileView tile in tiles)
                {
                    tile.DisplayColor = XNAColor.Red;
                }
            }

            if (isDrag && isPieceMarked && _selectedPiece == null)
            {
                DebugHelper.ConsolePrint("Selected Piece at " + markedPiece?.Position.X + " " + markedPiece?.Position.Y);
                DebugHelper.ConsolePrint("Mouse at " + _mouseInfo.CurrentPosition.X + " " + _mouseInfo.CurrentPosition.Y);
                _selectedPiece = markedPiece;
            }
            
            if (isDrag && _selectedPiece != null)
            {
                bool isPieceDropped = _dragAndDropManager.Update(_mouseInfo, _selectedPiece, _startPosition,
                    _tileSize, _boardView, _pieceSize);
                if (isPieceDropped)
                {
                    // _startMousePress = 0;
                    // isDrag = false;
                    // _selectedPiece = null;
                    
                    _boardView.Tiles.Each(tile =>
                    {
                        if (tile.DisplayColor == XNAColor.Red)
                        {
                            tile.DisplayColor = tile.TileColor;
                        }
                    });
                }
            }
            
            if (isDrag && _mouseInfo.PreviousLeftButtonState == ButtonState.Pressed &&
                _mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {
                DebugHelper.ConsolePrint("Drag release");
                _startMousePress = 0;
                isDrag = false;
                _selectedPiece = null;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(XNAColor.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate);
            DrawBoard();
            DrawPieces();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Load

        /*private void LoadPieces()
        {
            foreach (BoardCell cell in _boardView.Board.Cells)
            {
                if (cell.Piece != null)
                {
                    switch (cell.Piece.Type)
                    {
                        case PieceType.BISHOP:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Bishop"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                        case PieceType.KING:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("King"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                        case PieceType.QUEEN:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Queen"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                        case PieceType.KNIGHT:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Knight"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                        case PieceType.PAWN:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Pawn"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                        case PieceType.ROOK:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Rook"), ViewUtils.GetXNAColor(cell.Piece.Color), _pieceSize));
                            break;
                    }
                }else
                {
                    Console.Write("");
                }
            }
            
            Console.Write("");

        }*/

        /*private void LoadBoard()
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Vector2 position = new Vector2(_startPosition.X + ( (i-1) * _tileSize), _startPosition.Y + ((j-1) * _tileSize));

                    if (i == 1 || i % 2 != 0)
                    {
                        if (j == 1 || j % 2 != 0)
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.LightGray, _tileSize));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.Brown, _tileSize));
                        }
                    }
                    else
                    {
                        if (j == 1 || j % 2 != 0)
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.Brown, _tileSize));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.LightGray, _tileSize));
                        }

                    }
                }
            }
        }*/

        #endregion

        #region Draw

        public void DrawPieces()
        {
            foreach (IPieceView piece in _boardView.Pieces)
            {
                // _spriteBatch.Draw(piece.Texture, piece.Position, piece.Color);
                _spriteBatch.Draw(piece.Texture, piece.Position, null, piece.Color, 0f, Vector2.Zero, 2.0f,
                    SpriteEffects.None, 0f);
            }
        }

        public void DrawBoard()
        {
            foreach (TileView tileView in _boardView.Tiles)
            {
                // _spriteBatch.Draw(tileView.Texture, tileView.Position, tileView.Color);
                _spriteBatch.Draw(tileView.Texture, tileView.Position, null, tileView.DisplayColor, 0f, Vector2.Zero,
                    2.0f, SpriteEffects.None, 0f);
            }
        }

        #endregion
    }

    class GameLogic
    {
        public GameState CurrentState { get; set; }

        public GameLogic()
        {
            CurrentState = GameState.WHITEMOVE;
        }

        public void SetNextState()
        {
            if (CurrentState == GameState.WHITEMOVE)
            {
                CurrentState = GameState.BLACKMOVE;
            }

            if (CurrentState == GameState.BLACKMOVE)
            {
                CurrentState = GameState.WHITEMOVE;
            }
        }
    }

    public enum GameState
    {
        WHITEMOVE,
        BLACKMOVE
    }
}