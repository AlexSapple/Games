using System;
using Game.BoardGame;

namespace Game.Event
{
    public class BoardChangeEventArgs : EventArgs
    {
        internal Position SourcePosition { get; set; }

        internal string PropertyName { get; set; }
    }
}
