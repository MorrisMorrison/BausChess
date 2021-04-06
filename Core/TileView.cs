using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{
    public class TileView
    {
        public Vector2 Position {get;set;}
        public Texture2D Texture{get;set;}
        public Color TileColor {get;set;}
        public Color DisplayColor {get;set;}

        public IPieceView? Piece{get;set;}
        private BoardCell _boardCell{get;set;}

        public TileView(BoardCell boardCell, Vector2 position, Texture2D texture, Color tileColor, IPieceView? piece = null)
        {
            _boardCell = boardCell;
            Position = position;
            Texture = texture;
            TileColor= tileColor;
            DisplayColor = tileColor;
            Piece = piece;
        }
    }
}