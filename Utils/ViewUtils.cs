using System.Reflection;
using XNAColor = Microsoft.Xna.Framework.Color;
using ChessEngineColor = ChessEngine.Core.Color;
using System.Collections.Generic;
using BausChess.Core;
using Microsoft.Xna.Framework;
using ChessEngine.Core;
using System.Linq;

namespace BausChess.Utils
{
    public static class ViewUtils
    {
        public static XNAColor GetXNAColor(ChessEngineColor color)
        {
            return color == ChessEngineColor.WHITE ? XNAColor.White : XNAColor.Black;
        }

        public static Vector2 ParsePosition(IPieceView piece, IList<TileView> tiles, Vector2 startingPosition, int tileSize, int pieceSize)
        {
            Coordinates coordinates = piece.Piece.Coordinates;
            float x = startingPosition.X + (coordinates.Column * tileSize) + ((tileSize - pieceSize) / 2);
            float y = startingPosition.Y + (coordinates.Row * tileSize) + ((tileSize - pieceSize) / 2);

            return new Vector2(x, y);
        }

        public static Vector2 ParsePosition(Move move, IList<TileView> tiles, Vector2 startingPosition, int tileSize, int pieceSize)
        {
            Coordinates coordinates = move.Coordinates;
            float x = startingPosition.X + (coordinates.Column * tileSize) + ((tileSize - pieceSize) / 2);
            float y = startingPosition.Y + (coordinates.Row * tileSize) + ((tileSize - pieceSize) / 2);

            return new Vector2(x, y);
        }

        public static TileView FindTileForMove(Move move, IList<TileView> tiles, Vector2 startingPosition, int tileSize, int pieceSize)
        {
            float xFrom = startingPosition.X + (move.Coordinates.Column * tileSize) ;
            float xTo = startingPosition.X + (move.Coordinates.Column * tileSize) +  pieceSize;

            float yFrom = startingPosition.Y + (move.Coordinates.Row * tileSize) ;
            float yto = startingPosition.Y + (move.Coordinates.Row * tileSize) + pieceSize;

            return tiles.FirstOrDefault(p_tile => p_tile.Position.X >= xFrom && p_tile.Position.X <= xTo && p_tile.Position.Y >= yFrom && p_tile.Position.Y <= yto);
        }

        public static IList<TileView> FindTilesForMoves(IList<Move> moves, IList<TileView> tiles, Vector2 startingPosition, int tileSize, int pieceSize)
        {
            IList<TileView> relevantTiles = new List<TileView>();

            foreach (Move move in moves)
            {
                relevantTiles.Add(FindTileForMove(move, tiles, startingPosition, tileSize, pieceSize));
            }

            return relevantTiles;
        }

        public static bool IsPieceSelected(IPieceView piece, Vector2 mousePosition)
        {
            return mousePosition.X.CheckRange(piece.Position.X, piece.Position.X + piece.Texture.Width)
                             && mousePosition.Y.CheckRange(piece.Position.Y, piece.Position.Y + piece.Texture.Height);
        }

        public static IPieceView GetSelectedPiece(IList<IPieceView> pieces, Vector2 mousePosition)
        {
            return pieces.FirstOrDefault(piece => IsPieceSelected(piece, mousePosition));
        }
    }
}