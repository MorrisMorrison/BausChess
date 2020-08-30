using System.Collections.Generic;
using ChessEngine.Core;

namespace BausChess.Core
{

    public interface IBoardView{
        IList<IPieceView> Pieces{get;set;}
        Board Board {get;set;}
    }
    public class BoardView
    {
        public Board Board {get;set;}
        public IList<IPieceView> Pieces{get;set;}
        public IList<TileView> Tiles{get;set;}
        public BoardView(){
            Board = new Board();
            Pieces = new List<IPieceView>();
            Tiles = new List<TileView>();
        }

        public BoardView(Board board){
            Board = board;
        }
    }
}