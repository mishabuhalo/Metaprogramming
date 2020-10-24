using System;
using System.Collections.Generic;
using System.Text;

namespace CSSFormater
{
    public class Token
    {
        public TokenTypes TokenType { get; set; }
        public string TokenValue { get; set; }
        public int LineNumber { get; set; }
        public int Position { get; set; }

        public override string ToString()
        {
            return $"\"{TokenType.ToString("g")}\": \"{TokenValue}\", line number: {LineNumber+1}, position: {Position+1}";
        }
    }

   public enum TokenTypes
    {
        Comment,
        String,
        WhiteSpace,
        Tab,
        Identifier,
        Number,
        Operator,
        Symbol,
        ErrorToken,
        MatchOperator,
        OpeningBracket,
        ClosingBracket,
        NewLine
    }
}
