using System.Linq;
using BausChess.Utils;
using Microsoft.Xna.Framework;

namespace BausChess.Core
{

    public interface IViewManager
    {
        public IPieceView? FindPieceByMousePosition(Vector2 mousePosition);

        public ITileView? FindTileByMousePosition(Vector2 mousePosition);

    }
    
    public class ViewManager:IViewManager
    {
        private IBoardView _boardView;

        public ViewManager(IBoardView boardView)
        {
            _boardView = boardView;
        }

        public IPieceView? FindPieceByMousePosition(Vector2 mousePosition)
        {
            return _boardView.Pieces.FirstOrDefault(piece => IsPieceSelected(piece, mousePosition));
        }

        public ITileView? FindTileByMousePosition(Vector2 mousePosition)
        {
            return _boardView.Tiles.FirstOrDefault(tile => mousePosition.X.CheckRange(tile.Position.X, tile.Position.X + _boardView.TileSize) && mousePosition.Y.CheckRange(tile.Position.Y, tile.Position.Y + _boardView.TileSize));
        }

        private bool IsPieceSelected(IPieceView piece, Vector2 mousePosition)
        {
            return mousePosition.X.CheckRange(piece.Position.X - 10, piece.Position.X + piece.Texture.Width + 10)
                   && mousePosition.Y.CheckRange(piece.Position.Y -  10, piece.Position.Y + piece.Texture.Height +10 );
        }

    }
}