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
            }
        }
    }
}
