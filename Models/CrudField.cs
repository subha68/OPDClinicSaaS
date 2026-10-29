namespace Models
{
    public class CrudField
    {
        public string Name { get; set; } = "";
        public string Label { get; set; } = "";
        public string Type { get; set; } = "text";
        public string Col { get; set; } = "12";

        // Optional properties
        public string? Id { get; set; }
        public string? CssClass { get; set; }
        public bool? Disabled { get; set; }
        public string? HelpText { get; set; }

        public string? RelatedInputLabel { get; set; }
        public string? RelatedInputPlaceholder { get; set; }
        public string? RelatedInputTriggerValue { get; set; }
        public string? RelatedInputTriggerLabel { get; set; }
        public bool? RelatedInputRequired { get; set; }

        public Dictionary<string, string> DataAttributes { get; set; } = new();

        public CrudField()
        {
            // default constructor
        }

        public CrudField(string name, string label, string type, string col)
        {
            Name = name;
            Label = label;
            Type = type;
            Col = col;
        }

        public CrudField(string name, string label, string type, string col, 
                        string id, string cssClass, bool disabled, string helpText)
                        : this(name, label, type, col)
        {
            Id = id;
            CssClass = cssClass;
            Disabled = disabled;
            HelpText = helpText;
        }

        public CrudField(string name, string label, string type, string col, 
                        string relatedInputLabel, string relatedInputPlaceholder, string relatedInputTriggerValue,
                        string relatedInputTriggerLabel, bool relatedInputRequired)
                        : this(name, label, type, col)
        {
            RelatedInputLabel = relatedInputLabel;
            RelatedInputPlaceholder = relatedInputPlaceholder;
            RelatedInputTriggerValue = relatedInputTriggerValue;
            RelatedInputTriggerLabel = relatedInputTriggerLabel;
            RelatedInputRequired = relatedInputRequired;
        }


    }
}