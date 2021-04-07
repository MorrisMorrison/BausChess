using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{

    public interface ITileView
    {
        public Vector2 Position {get;set;}
        public Texture2D Texture{get;set;}
        public Color TileColor {get;set;}
        public Color DisplayColor {get;set;}
        public BoardCell BoardCell { get; set; }
        public IPieceView? Piece{get;set;}
        
    }
    
    public class TileView:ITileView
    {
        public Vector2 Position {get;set;}
        public Texture2D Texture{get;set;}
        public Color TileColor {get;set;}
        public Color DisplayColor {get;set;}
        public IPieceView? Piece{get;set;}
        public BoardCell BoardCell { get; set; }

        public TileView(BoardCell boardCell, Vector2 position, Texture2D texture, Color tileColor, IPieceView? piece = null)
        {
            BoardCell = boardCell;
            Position = position;
            Texture = texture;
            TileColor= tileColor;
            DisplayColor = tileColor;
            Piece = piece;
        }
    }
}