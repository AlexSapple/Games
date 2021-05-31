using System;
using Game.BoardGame;

namespace Game.Event
{
    public class BoardChangeEventArgs : EventArgs
    {
        public Position SourcePosition { get; set; }

        public string PropertyName { get; set; }
    }
}
