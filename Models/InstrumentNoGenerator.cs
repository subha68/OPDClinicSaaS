namespace Models
{
    public class InstrumentNoGenerator
    {
        public string InstrumentType { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public int FY { get; set; }
        public long Number { get; set; }
        public string InstrumentNo => $"{Prefix}/{FY}/{Number:D6}";
    }
}
