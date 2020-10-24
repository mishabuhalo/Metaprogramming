namespace CSSFormater.FormaterConfigurationModels
{
    public class Other
    {
        public string BracesPlacement { get; set; }
        public string AlignValues { get; set; }
        public string QuoteMarks { get; set; }
        public bool EnforceOnFormat { get; set; }
        public bool AlignClosingBraceWithProperties { get; set; }
        public bool KeepSingleLineBlocks { get; set; }
        public Spaces Spaces { get; set; }
        public HexColors HexColors { get; set; }
    }
}
