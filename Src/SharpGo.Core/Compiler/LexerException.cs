﻿namespace SharpGo.Core.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LexerException : Exception
    {
        public LexerException(string message)
            : base(message)
        {
        }
    }
}
