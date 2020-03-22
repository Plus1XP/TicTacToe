using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            this.Value = value;
        }

        public T Value { get; private set; }
    }
}
