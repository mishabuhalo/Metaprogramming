using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSFormater
{
    public class Lexer
    {
        public List<string> Lines { get; set; }
        public int TotalLinesCount { get; set; }
        public string CurrentLine { get; set; }
        public int CurrentLineNumber { get; set; }
        public char CurrentCharacter { get; set; }
        public int CurrentCharacterNumber { get; set; }

        public Lexer(string filePath)
        {
            Init(filePath);
        }

        private void Init(string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                byte[] bytes = new byte[fileStream.Length];

                fileStream.Read(bytes, 0, bytes.Length);

                string textFromFile = Encoding.Default.GetString(bytes);

                this.Lines = textFromFile.Replace(@"/\r\n/g", "\n").Replace(@"/\r\g", "\n").Split('\n').ToList();

                this.TotalLinesCount = Lines.Count;

                this.CurrentCharacterNumber = -1;
                this.CurrentLineNumber = -1;
                this.CurrentLine = "";
                this.CurrentCharacter = '\0';

                this.NextLine();
                this.NextCharacter();
            }
        }

        private string NextLine()
        {
            var lexer = this;

            lexer.CurrentLineNumber += 1;

            if(lexer.TotalLinesCount <= lexer.CurrentLineNumber)
            {
                lexer.CurrentLine = String.Empty;
            }
            else
            {
                lexer.CurrentLine = lexer.Lines[lexer.CurrentLineNumber];
            }
            if(lexer.CurrentCharacterNumber!=-1)
            {
                lexer.CurrentCharacterNumber = 0;
            }
            return lexer.CurrentLine;
        }

        private char NextCharacter()
        {
            var lexer = this;

            lexer.CurrentCharacterNumber += 1;

            if(lexer.CurrentCharacterNumber == lexer.CurrentLine.Length)
            {
                if(String.IsNullOrEmpty(this.NextLine()))
                {
                    this.CurrentCharacter = '\0';
                    return '\0';
                }
                this.CurrentCharacterNumber = -1;
                this.CurrentCharacter = '\n';
                return '\n';
            }

            this.CurrentCharacter = lexer.CurrentLine[lexer.CurrentCharacterNumber];

            return this.CurrentCharacter;
        }

        private List<Token> Tokenize()
        {
            throw new NotImplementedException();
        }
    }
}
