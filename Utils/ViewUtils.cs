using XNAColor = Microsoft.Xna.Framework.Color;
using ChessEngineColor = ChessEngine.Core.Color;

namespace BausChess.Utils
{
    public static class ViewUtils
    {
        public static XNAColor GetXNAColor(ChessEngineColor color)
        {
            return color == ChessEngineColor.WHITE ? XNAColor.White : XNAColor.Black;
        }
    }
}