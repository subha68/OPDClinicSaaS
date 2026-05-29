using DAL;
using Models;
using ViewModels;

namespace BLL
{    
    public class PatientService : BaseService<Patient>
    {
        private readonly PatientDAL _patientDAL;

        public PatientService(IConfiguration config)
        {
            _patientDAL = new PatientDAL(config);
            _dal = _patientDAL;
        }

        public Patient GetByMobile(string mobile)
        {
            return _patientDAL.GetByMobile(mobile);
        }

        public List<Patient> Search(string? patientId,
                                    string? phone,
                                    string? name,
                                    int clinicId)
        {
            return _patientDAL.Search(clinicId, patientId, phone, name);
        }

        public List<Patient> SearchByText(string searchString, int clinicId)
        {
            return _patientDAL.SearchByText(clinicId, searchString);
        }

        public List<Patient> ListByClinic(int clinicId)
        {
            return _patientDAL.ListByClinic(clinicId);
        }

        public Patient GetFirstOrDefault(List<Patient> patients)
        {
            Patient p;
            
            if (patients == null || patients.Count == 0)
            {
                p = new Patient
                {
                    PatientId = 0,
                    FirstName = "No",
                    LastName = "Record"
                };
            }
            else
            {
                p = patients[0];
            }

            return p;
        }

    }
}

