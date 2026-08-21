namespace Models
{
    public class MenuItem
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Icon { get; set; }
        public string? FeatureCode { get; set; }
        public string? ReportCode { get; set; }
        public int? ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; private set; }
        public bool HasActiveDescendant { get; private set; }
        public bool IsExpanded =>
                        IsCurrent || HasActiveDescendant;

        public List<MenuItem> Children { get; set; } = new();

        public void SetActiveState(string? currentController, string? currentAction, string? currentReportCode)
        {
            // Reset presentation state
            IsCurrent = false;
            HasActiveDescendant = false;

            // Determine whether this item is the current item
            if (!string.IsNullOrWhiteSpace(ReportCode))
            {
                IsCurrent = string.Equals(ReportCode, currentReportCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (!string.IsNullOrWhiteSpace(Controller))
            {
                IsCurrent = string.Equals(Controller, currentController, StringComparison.OrdinalIgnoreCase)
                    && (string.IsNullOrWhiteSpace(Action)
                        || string.Equals(Action, currentAction, StringComparison.OrdinalIgnoreCase));
            }

            // Recursively calculate the state of children
            foreach (var child in Children)
            {
                child.SetActiveState(currentController, currentAction, currentReportCode);

                if (child.IsCurrent || child.HasActiveDescendant)
                {
                    HasActiveDescendant = true;
                }
            }
        }     
    }
}