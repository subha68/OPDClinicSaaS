using DAL;
using Models;

namespace BLL
{
    public class MedicineService : BaseService<Medicine>
    {
        private readonly MedicineDAL _medicineDAL;

        public MedicineService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _medicineDAL = new MedicineDAL(config);
            _dal = _medicineDAL;
        }

        public override int Create(Medicine model)
        {
            // set default values
            if (!model.ReorderLevel.HasValue)
                model.ReorderLevel = 0;
            if (model.UnitOfMeasure == null)  
                model.UnitOfMeasure = "Tab";   
            if (model.DosageForm == null)
                model.DosageForm = "Tablet";
            if (model.GenericName == null)
                model.GenericName = model.MedicineName;
            if (model.ScheduleType == null)
                model.ScheduleType = "OTC";
            if (!model.IsActive.HasValue)
                model.IsActive = true;
            if (!model.CreatedFromPrescription.HasValue)
                model.CreatedFromPrescription = false;

            return base.Create(model);
        }

        public List<Medicine> Search(string term)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _medicineDAL.GetAll(term, ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }
    }
}