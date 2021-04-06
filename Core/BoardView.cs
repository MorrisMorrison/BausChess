using System.Collections.Generic;
using BausChess.Utils;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{

    public interface IBoardView
    {
        IList<IPieceView> Pieces { get; set; }
        Board Board { get; set; }
    }
    public class BoardView
    {
        public Board Board { get; set; }
        public IList<IPieceView> Pieces { get; set; }
        public IList<TileView> Tiles { get; set; }
        public Vector2 StartPosition { get; set; }
        public int TileSize { get; set; }
        public int PieceSize { get; set; }


        public BoardView(Vector2 startPosition, int tileSize, int pieceSize)
        {
            Board = new Board();
            Pieces = new List<IPieceView>();
            Tiles = new List<TileView>();
            StartPosition = startPosition;
            TileSize = tileSize;
            PieceSize = pieceSize;
        }

        public BoardView(Board board)
        {
            Board = board;
        }

        public void Initialize(ContentManager content)
        {
            Texture2D tileTexture = content.Load<Texture2D>("Tile");

            foreach (BoardCell cell in Board)
            {
                IPieceView? piece = null;

                if (cell.Piece != null)
                {
                    string pieceType= cell.Piece.Type.ToString().ToLower();
                    string textureName = char.ToUpper(pieceType[0]) + pieceType.Substring(1);
                    Texture2D pieceTexture = content.Load<Texture2D>(textureName);
                    piece = new PieceView(cell.Piece, new Vector2(0, 0), pieceTexture, ViewUtils.GetXNAColor(cell.Piece.Color), PieceSize);
                    Vector2 piecePosition =  ViewUtils.ParsePosition(piece, Tiles, StartPosition, TileSize, PieceSize);
                    piece.Position = piecePosition;
                    Pieces.Add(piece);
                }

                Vector2 tilePosition = new Vector2(StartPosition.X + ((cell.Coordinates.Column) * TileSize), StartPosition.Y + ((cell.Coordinates.Row) * TileSize));
                
                Color color = Color.White;
                
                int x = cell.Coordinates.Column;
                int y = cell.Coordinates.Row;

                if ((x + 1) == 1 || (x + 1) % 2 != 0)
                {
                    if ((y + 1) == 1 || (y + 1) % 2 != 0) color = Color.LightGray;
                    else color = Color.Brown;
                }
                else
                {
                    if ((y + 1) == 1 || (y + 1) % 2 != 0) color = Color.Brown;
                    else color = Color.LightGray;
                }

                Tiles.Add(new TileView(cell, tilePosition, tileTexture, color, piece));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }

    }

    public class BoardLogic
    {

    }
}
