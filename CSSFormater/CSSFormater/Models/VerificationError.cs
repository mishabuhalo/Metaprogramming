using System;
using System.Collections.Generic;
using System.Text;

namespace CSSFormater.Models
{
    public class VerificationError
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorLineNumber { get; set; }
        public int ErrorPosition { get; set; }
    }
}
