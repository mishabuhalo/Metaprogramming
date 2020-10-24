using System;

namespace CSSFormater
{
    class Program
    {
        static void Main(string[] args)
        {
            var testFilePath = "test.css";

            Lexer lexer = new Lexer(testFilePath);
        }
    }
}
