using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BausChess.Core
{
  public class MouseInfo
    {
        public ButtonState CurrentLeftButtonState { get; set; }
        public ButtonState PreviousLeftButtonState { get; set; }
        public ButtonState CurrentRightButtonState { get; set; }
        public ButtonState PreviousRightButtonState { get; set; }
        public Vector2 CurrentPosition { get; set; }
        public Vector2 PreviousPosition { get; set; }
        public void Update(MouseState state)
        {
            PreviousPosition = CurrentPosition;
            PreviousLeftButtonState = CurrentLeftButtonState;
            CurrentPosition = state.Position.ToVector2();
            CurrentLeftButtonState = state.LeftButton;
        }
    }

}