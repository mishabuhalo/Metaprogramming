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

        private List<Token> Tokens { get; set; }

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

                Tokens = new List<Token>();

                this.NextLine();
                this.NextCharacter();
            }
        }

        private string NextLine()
        {
            var lexer = this;

            lexer.CurrentLineNumber += 1;

            if (lexer.TotalLinesCount <= lexer.CurrentLineNumber)
            {
                lexer.CurrentLine = String.Empty;
            }
            else
            {
                lexer.CurrentLine = lexer.Lines[lexer.CurrentLineNumber];
            }
            if (lexer.CurrentCharacterNumber != -1)
            {
                lexer.CurrentCharacterNumber = 0;
            }
            return lexer.CurrentLine;
        }

        private char NextCharacter()
        {
            var lexer = this;

            lexer.CurrentCharacterNumber += 1;

            if (lexer.CurrentCharacterNumber == lexer.CurrentLine.Length)
            {
                if (String.IsNullOrEmpty(this.NextLine()))
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

        private void Tokenize()
        {
            var currentCharacter = this.CurrentCharacter;

            if (currentCharacter == '-' || currentCharacter == '.' || IsDigit(currentCharacter)) // for cases when -1px or 1.32 etc
            {
                NumberHandler();
            }

            if (currentCharacter == ' ' || currentCharacter == '\t')
            {

            }

            if (currentCharacter == '/')
            {

            }

            if (currentCharacter == '\"' || currentCharacter == '\'')
            {

            }

            if(IsLetter(currentCharacter))
            {

            }

            if(IsOperator(currentCharacter))
            {

            }

            if (currentCharacter == '\n')
            {
                this.NextCharacter();
            }

            else throw new Exception("Unrcognized character");
        }

        private void NumberHandler()
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            string token = currentCharacter.ToString();
            bool IsPoint = token == ".";
            bool IsNotDigit;

            currentCharacter = lexer.NextCharacter();

            IsNotDigit = !IsDigit(currentCharacter);

            //.2px or .class
            if(IsPoint && IsNotDigit)
            {
                CreateAndAddToken(token, TokenTypes.Symbol);
            }

            //-2px or -something-something
            if(token == "-" && IsNotDigit)
            {
                 HandleIdentifier('-');
            }

            while(currentCharacter!='\0' &&(IsDigit(currentCharacter)||(!IsPoint&& currentCharacter == '.')))
            {
                if(currentCharacter == '.')
                {
                    IsPoint = true;
                    token += currentCharacter;
                }
                currentCharacter = lexer.NextCharacter();
            }

            CreateAndAddToken(token, TokenTypes.Number);
        }

        private void CreateAndAddToken(string token, TokenTypes tokenType)
        {
            Tokens.Add(new Token { TokenType = tokenType, TokenValue = token });
        }

        private Token HandleIdentifier(char v)
        {
            throw new NotImplementedException();
        }


        private bool IsOperator(char currentCharacter)
        {
            var operatorSet = "{ } [ ] ( ) + * = . , ; : > ~ | \\ % $ # @ ^ !".Split(' ');

            for(int i = 0; i < operatorSet.Count(); ++i)
            {
                if(currentCharacter.ToString() == operatorSet[i])
                {
                    return true;
                }
            }
            return false;
        }

        public void PrintAllTokens()
        {
            foreach(var token in this.Tokens)
            {
                Console.WriteLine(token.ToString() + "\n");
            }
        }

        private bool IsLetter(char currentCharacter)
        {
            return (currentCharacter == '_' || currentCharacter == '-' || Char.IsLetter(currentCharacter));
        }

        private bool IsDigit(char currentCharacter)
        {
            return Char.IsDigit(currentCharacter);
        }
    }
}
