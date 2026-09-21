using System;
using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class PrescriptionRowVM
    {
        public int RowId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public int DisplayOrder { get; set; }

        public List<PrescriptionFieldVM> Fields { get; set; }
            = new List<PrescriptionFieldVM>();
        public List<PrescriptionVitalVM> Vitals { get; set; }
            = new List<PrescriptionVitalVM>();
        public List<PrescriptionTestResultVM> TestResults { get; set; }
            = new List<PrescriptionTestResultVM>();
    }
}