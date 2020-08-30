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



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _mouseInfo = new MouseInfo();
            _dragAndDropManager = new DragAndDropManager();
            _boardView = new BoardView();
            _startPosition = new Vector2((_screenWidth - (8 * _tileSize)) / 2, (_screenHeight - (8 * _tileSize)) / 2);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = _screenWidth;
            _graphics.PreferredBackBufferHeight = _screenHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadBoard();
            LoadPieces();
            LoadPiecesPositions();
        }

        private void LoadPiecesPositions()
        {
            foreach(IPieceView piece in _boardView.Pieces){
                piece.Position = ViewUtils.ParsePosition(piece, _boardView.Tiles, _startPosition, _tileSize, _pieceSize);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _mouseInfo.Update(Mouse.GetState());
            _dragAndDropManager.Update(_mouseInfo, _boardView.Pieces, _startPosition, _tileSize, _boardView, _pieceSize);

            // check if piece is clicked
            bool isClicked = _mouseInfo.PreviousLeftButtonState == ButtonState.Pressed && _mouseInfo.CurrentLeftButtonState == ButtonState.Released;
            IPieceView selectedPiece = ViewUtils.GetSelectedPiece(_boardView.Pieces, _mouseInfo.CurrentPosition);
            if (isClicked && selectedPiece != null){
                IList<Coordinates> validMoves = _boardView.Board.FindValidMoves(selectedPiece.Piece);
                IList<TileView> tiles = ViewUtils.FindTilesForMoves(validMoves.Select(coordinates => new Move(selectedPiece.Piece, coordinates)).ToList(), _boardView.Tiles, _startPosition, _tileSize, _pieceSize);
                foreach (TileView tile in tiles){
                    tile.Color = XNAColor.Red;
                }
            }
            // if true display valid moves

            base.Update(gameTime);
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

        private void LoadPieces()
        {
            foreach (BoardCell cell in _boardView.Board.Cells)
            {

                if (cell.Piece != null)
                {
                    switch (cell.Piece.Type)
                    {
                        case PieceType.BISHOP:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Bishop"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                        case PieceType.KING:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("King"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                        case PieceType.QUEEN:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Queen"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                        case PieceType.KNIGHT:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Knight"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                        case PieceType.PAWN:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Pawn"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                        case PieceType.ROOK:
                            _boardView.Pieces.Add(new PieceView(cell.Piece, new Vector2(0, 0), Content.Load<Texture2D>("Rook"), ViewUtils.GetXNAColor(cell.Piece.Color)));
                            break;
                    }
                }
            }

        }

        private void LoadBoard()
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
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.LightGray));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.Brown));
                        }
                    }
                    else
                    {
                        if (j == 1 || j % 2 != 0)
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.Brown));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(position, Content.Load<Texture2D>("Tile"), XNAColor.LightGray));
                        }

                    }
                }
            }
        }

        #endregion

        #region Draw


        public void DrawPieces()
        {
            foreach (IPieceView piece in _boardView.Pieces)
            {
                // _spriteBatch.Draw(piece.Texture, piece.Position, piece.Color);
                _spriteBatch.Draw(piece.Texture, piece.Position, null, piece.Color, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);

            }
        }

        public void DrawBoard()
        {
            foreach (TileView tileView in _boardView.Tiles)
            {
                // _spriteBatch.Draw(tileView.Texture, tileView.Position, tileView.Color);
                _spriteBatch.Draw(tileView.Texture, tileView.Position, null, tileView.Color, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
            }
        }

        #endregion
    }
}
