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
        public int StartCharacterNumber { get; set; }

        public override string ToString()
        {
            return $"\"{TokenType.ToString("g")}\": \"{TokenValue}\", line number: {LineNumber}, start character number: {StartCharacterNumber}";
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
        ClosingBracket
    }
}
