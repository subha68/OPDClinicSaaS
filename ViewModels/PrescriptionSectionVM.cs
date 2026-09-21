using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionSectionVM
    {
        public int SectionId { get; set; }
        public int ConsultationId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsVisible { get; set; }
        public bool Multiline { get; set; }
        public bool IsPharmacologyAdvice { get; set; }
        public bool IsVital { get; set; }
        public bool IsTestResult { get; set; }

        public List<PrescriptionRowVM> Rows { get; set; }
            = new List<PrescriptionRowVM>();
    }
}