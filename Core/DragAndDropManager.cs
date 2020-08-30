using System.Collections.Generic;
using BausChess.Utils;
using Microsoft.Xna.Framework.Input;

namespace BausChess.Core
{
 public class DragAndDropManager
    {

        IPieceView? SelectedPiece { get; set; }
        public DragAndDropManager()
        {
        }

        public void Update(MouseInfo mouseInfo, IList<IPieceView> pieces)
        {
            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && SelectedPiece == null)
            {
                foreach (IPieceView pieceView in pieces)
                {
                    if (mouseInfo.CurrentPosition.X.CheckRange(pieceView.Position.X, pieceView.Position.X + pieceView.Texture.Width)
                    && mouseInfo.CurrentPosition.Y.CheckRange(pieceView.Position.Y, pieceView.Position.Y + pieceView.Texture.Height))
                    {
                        SelectedPiece = pieceView;
                        break;
                    }
                }
            }

            if (mouseInfo.CurrentLeftButtonState == ButtonState.Pressed && SelectedPiece != null){
                SelectedPiece.Position = mouseInfo.CurrentPosition;
            }

            if (SelectedPiece != null && mouseInfo.CurrentLeftButtonState == ButtonState.Released)
            {
                SelectedPiece.Position = mouseInfo.CurrentPosition;
                SelectedPiece = null;
            }

        }

    }
}