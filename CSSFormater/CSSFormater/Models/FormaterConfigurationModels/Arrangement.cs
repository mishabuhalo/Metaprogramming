using System;
using System.Collections.Generic;
using System.Text;

namespace CSSFormater.FormaterConfigurationModels
{
    public class Arrangement
    {
        public bool SortCssPropertiesByName { get; set; }
        public bool SortCssPropertiesByCustomOrder { get; set; }
        public List<string> CustomOrder { get; set; }
    }
}
