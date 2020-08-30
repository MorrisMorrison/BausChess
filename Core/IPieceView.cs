using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{
    public interface IPieceView
    {
        IPiece Piece {get;set;}
        Vector2 Position {get;set;}
        Texture2D Texture{get;set;}
        Color Color {get;set;}
    }

    public class PieceView : IPieceView
    {
        public IPiece Piece {get;set;}
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }

        public Color Color {get;set;}

        public PieceView(IPiece piece, Vector2 position, Texture2D texture, Color color){
            Piece = piece;
            Position = position;
            Texture = texture;
            Color = color;
        }
    }
}