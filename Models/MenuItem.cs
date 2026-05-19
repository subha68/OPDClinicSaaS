namespace Models
{
    public class MenuItem
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; } = "Index";
        public string Icon { get; set; }
        
    }
}