using System;
using System.Collections.Generic;
using System.Text;

namespace CSSFormater
{
    public class Token
    {
        public TokenTypes TokenType { get; set; }
        public string TokenValue { get; set; }

        public override string ToString()
        {
            return $"\"{TokenType.ToString("g")}\": \"{TokenValue}\"";
        }
    }

   public enum TokenTypes
    {
        Comment,
        String,
        WhiteSpace,
        Tab,
        NewLine,
        Identifier,
        Number,
        Operator,
        Symbol
    }
}
