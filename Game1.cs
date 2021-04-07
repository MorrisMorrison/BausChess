using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAColor = Microsoft.Xna.Framework.Color;
using ChessEngineColor = ChessEngine.Core.Color;
using BausChess.Utils;
using BausChess.Core;

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
        private GameConfig _gameConfig;
        private bool _configPrinted;
        private ViewManager _viewManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _mouseInfo = new MouseInfo();
            _gameLogic = new GameLogic();
            _startPosition = new Vector2((_screenWidth - (8 * _tileSize)) / 2, (_screenHeight - (8 * _tileSize)) / 2);
            _boardView = new BoardView(_startPosition, 64, 32);
            _viewManager = new ViewManager(_boardView);
            _dragAndDropManager = new DragAndDropManager(_gameLogic, _viewManager);
            _gameConfig = new GameConfig("../../../appsettings.json");
            
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _boardView.Initialize(Content);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            _mouseInfo.Update(Mouse.GetState());
            DebugHelper.DebugPrint($@"Mouse at X:{Mouse.GetState().X} Y:{Mouse.GetState().Y}");
            
            string pieceMovementMode = (string) _gameConfig["GameSettings:PieceMovementMode"];
            if (pieceMovementMode == "drag")
            {
                if (!_configPrinted) DebugHelper.ConsolePrint("PieceMovementMode: " + pieceMovementMode);
                _configPrinted = true;
                _dragAndDropManager.Update(_mouseInfo, _startPosition, _tileSize, _boardView, _pieceSize, gameTime);           
            }

            if (pieceMovementMode == "click")
            {
                if (!_configPrinted) DebugHelper.ConsolePrint("PieceMovementMode: " + pieceMovementMode);
                _configPrinted = true;
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

    public class GameLogic
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