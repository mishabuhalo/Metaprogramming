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
                NumberHandling();
            }

            if (currentCharacter == ' ' || currentCharacter == '\t')
            {
                WhiteSpaceHandling();
            }

            if (currentCharacter == '/')
            {
                CommentHandling();
            }

            if (currentCharacter == '\"' || currentCharacter == '\'')
            {
                StringHandling();
            }

            if(IsLetter(currentCharacter))
            {
                IdentifierHandling();
            }

            if(IsOperator(currentCharacter))
            {
                OperatorHandling();
            }
            if(IsBracket(currentCharacter))
            {
                BracketHandling();
            }

            if (currentCharacter == '\n')
            {
                this.NextCharacter();
            }

            CreateAndAddToken(currentCharacter.ToString(), TokenTypes.ErrorToken);
        }

        private void BracketHandling()
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            var token = currentCharacter.ToString();
            CreateAndAddToken(token, TokenTypes.Bracket);
            lexer.NextCharacter();
        }

        private void OperatorHandling()
        {
            var lexer = this;
            var curentCharacter = lexer.CurrentCharacter;
            var token = curentCharacter.ToString();
            var nextCharacter = lexer.NextCharacter();

            if(nextCharacter=='=' && IsOperator(token[0], true))
            {
                token += nextCharacter;
                CreateAndAddToken(token, TokenTypes.MatchOperator);
                lexer.NextCharacter();
                return;
            }

            CreateAndAddToken(token, TokenTypes.Operator);
        }

        private void IdentifierHandling(char identifier = '\0')
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            string token = identifier != '\0' ? identifier.ToString() + currentCharacter.ToString() : currentCharacter.ToString();

            currentCharacter = lexer.NextCharacter();

            while(IsLetter(currentCharacter) || IsDigit(currentCharacter))
            {
                token += currentCharacter;
                currentCharacter = lexer.NextCharacter();
            }

            CreateAndAddToken(token, TokenTypes.Identifier);
        }

        private void StringHandling()
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            var token = currentCharacter.ToString();
            var quote = currentCharacter;
            char nextCharacter;

            currentCharacter = lexer.NextCharacter();

            while (currentCharacter!=quote)
            {
                if(currentCharacter == '\n')
                {
                    nextCharacter = lexer.NextCharacter();
                    if(nextCharacter == '\\')
                    {
                        token += currentCharacter + nextCharacter;
                    }
                    else
                    {
                        // end of line without \ escape
                        CreateAndAddToken(token, TokenTypes.ErrorToken);
                    }
                }
                else
                {
                    if(currentCharacter== '\\')
                    {
                        token += currentCharacter + lexer.NextCharacter();
                    }
                    else
                    {
                        token += currentCharacter;
                    }
                }

                currentCharacter = lexer.NextCharacter();
            }

            token += currentCharacter;
            lexer.NextCharacter();
            CreateAndAddToken(token, TokenTypes.String);
        }

        private void CommentHandling()
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            string token = currentCharacter.ToString();
            char nextCharacter =lexer.NextCharacter();

            if(nextCharacter!='*')
            {
                CreateAndAddToken(token, TokenTypes.Symbol);
            }

            while(currentCharacter!='*' && nextCharacter !='/')
            {
                token += nextCharacter.ToString();
                currentCharacter = nextCharacter;
                nextCharacter = lexer.NextCharacter();
            }
            token += nextCharacter.ToString();
            lexer.NextCharacter();

            CreateAndAddToken(token, TokenTypes.Comment);
        }

        private void WhiteSpaceHandling()
        {
            var lexer = this;
            var currentCharacter = lexer.CurrentCharacter;
            var token = currentCharacter.ToString();

            if(token == "\t")
            {
                CreateAndAddToken(token, TokenTypes.Tab);
            }
            else
            {
                currentCharacter = lexer.NextCharacter();

                while(currentCharacter == ' ')
                {
                    token += currentCharacter.ToString();
                    currentCharacter = lexer.NextCharacter();
                }

                CreateAndAddToken(token, TokenTypes.WhiteSpace);
            }
        }

        private void NumberHandling()
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
                 IdentifierHandling('-');
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


        private bool IsOperator(char currentCharacter, bool checkMatch= false)
        {
            var operatorSet = "+ * = . , ; : > ~ | \\ % $ # @ ^ !".Split(' ');
            var operatorMatchSet = "* ^ | $ ~".Split(' ');

            if (!checkMatch)
            {
                for (int i = 0; i < operatorSet.Count(); ++i)
                {
                    if (currentCharacter.ToString() == operatorSet[i])
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                for (int i = 0; i < operatorMatchSet.Count(); ++i)
                {
                    if (currentCharacter.ToString() == operatorMatchSet[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool IsBracket(char currentCharacter)
        {
            var bracketSet = "{ } [ ] ( )".Split(' ');

            for(int i = 0; i< bracketSet.Count(); ++i)
            {
                if(currentCharacter.ToString() == bracketSet[i])
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
