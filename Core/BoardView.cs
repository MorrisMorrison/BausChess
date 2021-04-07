using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BausChess.Utils;
using ChessEngine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Utilities.Collections;
using Color = Microsoft.Xna.Framework.Color;

namespace BausChess.Core
{

    public interface IBoardView
    {
        IList<IPieceView> Pieces { get; set; }
        IList<ITileView> Tiles { get; set; }
        Board Board { get; set; }
        void MakeMove(IPieceView piece, ITileView tile);
        public int TileSize { get; set; }
        public int PieceSize { get; set; }
        IList<ITileView> FindValidTiles(IPieceView piece);
    }
    public class BoardView:IBoardView
    {
        public Board Board { get; set; }
        public IList<IPieceView> Pieces { get; set; }
        public IList<ITileView> Tiles { get; set; }
        public Vector2 StartPosition { get; set; }
        public int TileSize { get; set; }
        public int PieceSize { get; set; }


        public BoardView(Vector2 startPosition, int tileSize, int pieceSize)
        {
            Board = new Board();
            Pieces = new List<IPieceView>();
            Tiles = new List<ITileView>();
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

        
        public void MakeMove(IPieceView piece, ITileView tile)
        {
            // make move in underlying board
            Board.Move(piece.Piece, tile.BoardCell.Coordinates);
            
            // update piece position
            piece.Position = ViewUtils.GetTileCenter(tile, TileSize, PieceSize);

            // remove piece from old tile
            ITileView oldTile = FindTileByPiece(piece);
            oldTile.Piece = null;

            // add piece to new tile
            tile.Piece = piece;
        }

        public IList<ITileView> FindValidTiles(IPieceView piece)
        {
            IList<Coordinates> validMovesCoordinates = Board.FindValidMoves(piece.Piece);
            return FindTilesByCoordinates(validMovesCoordinates);
        }
        private IList<ITileView> FindTilesByCoordinates(IList<Coordinates> moveCoordinates)
        {
            IList<ITileView> tiles = new List<ITileView>();
            
            moveCoordinates.Each(coordinates =>
            {
                ITileView? tile = FindTileByCoordinates(coordinates);
                if (tile != null) tiles.Add(tile);
            });

            return tiles;
        }
        private  ITileView? FindTileByCoordinates(Coordinates coordinates)
        {
            float xFrom = StartPosition.X + (coordinates.Column * TileSize) ;
            float xTo = StartPosition.X + (coordinates.Column * TileSize) +  PieceSize;

            float yFrom = StartPosition.Y + (coordinates.Row * TileSize) ;
            float yto = StartPosition.Y + (coordinates.Row * TileSize) + PieceSize;

            return Tiles.FirstOrDefault(tile => tile.Position.X >= xFrom && tile.Position.X <= xTo && tile.Position.Y >= yFrom && tile.Position.Y <= yto);
        }
        private ITileView FindTileByPiece(IPieceView piece) => Tiles.FirstOrDefault(tile => tile.Piece == piece);
    }
        
    }
