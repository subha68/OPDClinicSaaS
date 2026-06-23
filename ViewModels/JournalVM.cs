using Models;

namespace ViewModels
{
    public class JournalVM
    {
        public DateTime JournalDate { get; set; }
        public string VoucherType { get; set; }
        public string ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public string Narration { get; set; }
        public List<JournalLineVM> Lines { get; set; }
    }
}