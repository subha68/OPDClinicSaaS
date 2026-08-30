namespace Models
{
    public class BillNoGenerator
    {
        public string Prefix { get; set; } = string.Empty;
        public int FY { get; set; }
        public long Number { get; set; }
        public string BillNo => $"{Prefix}/{FY}/{Number:D6}";
    }
}
