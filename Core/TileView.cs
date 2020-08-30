using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BausChess.Core
{
    public class TileView
    {
        public Vector2 Position {get;set;}
        public Texture2D Texture{get;set;}
        public Color Color {get;set;}

        public TileView(Vector2 position, Texture2D texture, Color color)
        {
            Position = position;
            Texture = texture;
            Color= color;
        }
    }
}