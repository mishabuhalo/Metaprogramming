namespace CSSFormater.FormaterConfigurationModels
{
    public class TabsAndIndents
    {
        public bool UseTabCharacter { get; set; }
        public bool SmartTabs { get; set; }
        public int TabSize { get; set; }
        public int Indent { get; set; }
        public int ContinuationIndent { get; set; }
        public bool KeepIndentsOnEmtyLines { get; set; }
    }
}
