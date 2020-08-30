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


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _mouseInfo = new MouseInfo();
            _dragAndDropManager = new DragAndDropManager();
            _boardView = new BoardView();
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 690;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadBoard();
            LoadPieces();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _mouseInfo.Update(Mouse.GetState());
            _dragAndDropManager.Update(_mouseInfo, _boardView.Pieces);


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
                    if (i == 1 || i % 2 != 0)
                    {
                        if (j == 1 || j % 2 != 0)
                        {
                            _boardView.Tiles.Add(new TileView(new Vector2(i * 64, j * 64), Content.Load<Texture2D>("Tile"), XNAColor.LightGray));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(new Vector2(i * 64, j * 64), Content.Load<Texture2D>("Tile"), XNAColor.Brown));
                        }
                    }
                    else
                    {
                        if (j == 1 || j % 2 != 0)
                        {
                            _boardView.Tiles.Add(new TileView(new Vector2(i * 64, j * 64), Content.Load<Texture2D>("Tile"), XNAColor.Brown));
                        }
                        else
                        {
                            _boardView.Tiles.Add(new TileView(new Vector2(i * 64, j * 64), Content.Load<Texture2D>("Tile"), XNAColor.White));
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
                _spriteBatch.Draw(piece.Texture, piece.Position, piece.Color);
            }
        }

        public void DrawBoard()
        {
            foreach (TileView tileView in _boardView.Tiles)
            {
                _spriteBatch.Draw(tileView.Texture, tileView.Position, tileView.Color);
            }
        }

        #endregion
    }
}
