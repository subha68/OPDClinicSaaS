using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DAL;
using Helpers;
using Models;
using ViewModels;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class VitalService : BaseService<Vital>
    {
        private readonly VitalDAL _vitalDAL;
        private readonly VitalDefinitionDAL _vitalDefDAL;
        private readonly VisitDAL _visitDAL;
        private readonly PatientDAL _patientDAL;
        private readonly VisitService _visit;

        public VitalService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                            : base(config, httpContextAccessor)
        {
            _vitalDAL = new VitalDAL(config);
            _vitalDefDAL = new VitalDefinitionDAL(config);
            _visitDAL = new VisitDAL(config);
            _patientDAL = new PatientDAL(config);
            _visit = new VisitService(config, httpContextAccessor);
            _dal = _vitalDAL;
        }

        public VitalResult GetByPatientAndDate(int patientId)
        {
            List<Vital> vitals = new List<Vital>();
            List<VitalDefinition> vitalDefs = new List<VitalDefinition>();

            DateTime visitDate = DateTime.Today;

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (patientId <= 0)
                {
                    return new VitalResult
                    {
                        Success = false,
                        Message = "No patient was specified."
                    };
                }

                Visit visit = _visit.GetByPatientAndDate(patientId, visitDate, conn);

                if (visit == null)
                {
                    return new VitalResult
                    {
                        Success = false,
                        Message = "Book an appointment first for the selected patient today."
                    };
                }

                LastVisitId = visit.VisitId;
                LastVisitDate = FormatDate(visit.VisitDate);

                vitals = _vitalDAL.GetAll(ClinicId, visit.VisitId, conn);

                // Populate empty vitals from definition.
                vitalDefs = _vitalDefDAL.GetAll(ClinicId, conn);

                foreach (var vitalDef in vitalDefs)
                {
                    if (!vitals.Any(v =>
                        v.VitalCode == vitalDef.VitalCode))
                    {
                        vitals.Add(new Vital(vitalDef, ClinicId, visit.VisitId));
                    }
                }

                return new VitalResult
                {
                    Success = true,
                    Vitals = vitals
                };
            }
            catch
            {
                throw;
            }
        }

        public bool Upsert(int patientId, List<Vital> models)
        {
            if (patientId <= 0)
            {
                throw new ArgumentException(
                    "A valid patient is required.",
                    nameof(patientId)
                );
            }

            if (models == null || models.Count == 0)
            {
                return false;
            }

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                * Find today's visit for the explicitly supplied
                * patient.
                *
                * Do not use PatientId or LastVisitId from BaseService.
                */
                var visit = _visit.GetByPatientAndDate(patientId, DateTime.Today, conn, tran);

                if (visit == null)
                {
                    throw new InvalidOperationException(
                        "Book an appointment first for the selected patient today."
                    );
                }


                var vitalDefs = _vitalDefDAL.GetAll(ClinicId, conn, tran);

                var definitions = vitalDefs.ToDictionary(
                        x => x.VitalDefId
                    );


                List<Vital> newVitals = new List<Vital>();
                List<Vital> modifiedVitals = new List<Vital>();
                List<Vital> deletedVitals = new List<Vital>();

                foreach (var model in models)
                {
                    /*
                    * will save Derived and Composition values also
                    */
                /*    if (model.IsDerived || model.VitalType == "Composition")
                    {
                        continue;
                    } */


                    /*
                    * Do not create an empty vital.
                    */
                    if (
                        model.VitalId == 0 &&
                        string.IsNullOrWhiteSpace(
                            model.VitalValue
                        )
                    )
                    {
                        continue;
                    }


                    /*
                    * Populate Vital metadata from
                    * the server-side VitalDefinition.
                    */
                    if (definitions.TryGetValue(model.VitalDefId, out var vitalDef))
                    {
                        model.ClinicId = ClinicId;
                        model.VitalLabel = vitalDef.VitalLabel;
                        model.VitalCode = vitalDef.VitalCode;
                        model.VitalType = vitalDef.VitalType;
                        model.DataType = vitalDef.DataType;
                        model.VitalUnit = vitalDef.VitalUnit;
                        model.DeriveFormula = vitalDef.DeriveFormula;
                        model.DisplayComposition = vitalDef.DisplayComposition;
                    }


                    /*
                    * Always associate the submitted vital
                    * with the visit belonging to the supplied patient.
                    */
                    model.VisitId = visit.VisitId;

                    if (model.VitalId == 0)
                    {
                        newVitals.Add(model);
                    }
                    else if (string.IsNullOrWhiteSpace(model.VitalValue))
                    {
                        deletedVitals.Add(model);
                    }
                    else
                    {
                        modifiedVitals.Add(model);
                    }
                }


                // ========================================================
                // INSERT
                // ========================================================

                int rowsAdded = 0;

                foreach (var model in newVitals)
                {
                    _vitalDAL.Add(model, conn, tran);

                    rowsAdded++;
                }


                // ========================================================
                // UPDATE
                // ========================================================

                int rowsModified = 0;

                foreach (var model in modifiedVitals)
                {
                    _vitalDAL.Update(model, conn, tran);

                    rowsModified++;
                }


                // ========================================================
                // DELETE
                // ========================================================

                int rowsDeleted = 0;

                foreach (var model in deletedVitals)
                {
                    _vitalDAL.Delete(model.VitalId, ClinicId, conn, tran);

                    rowsDeleted++;
                }


                tran.Commit();


                return
                    rowsAdded > 0 ||
                    rowsModified > 0 ||
                    rowsDeleted > 0;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<Vital> GetVitalsByVisit(int patientId, int visitId, int clinicId,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Vital> vitals = new List<Vital>();

            try
            {
                vitals = _vitalDAL.GetAll(clinicId, visitId, conn, tran);

                // Do not need empty vitals from definition.

                return vitals;
            }
            catch
            {
                throw;
            }
        }

        // Pass VitalCode, VitalValue pair for all Vitals in dictionary
        // Returns only the calculated values of derived vitals
        // need to be sorted by DisplayOrder buy calling method
        public List<Vital> CalculateDerivedVitals(Dictionary<string, string> values)
        {
            var result = new List<Vital>();

            using var conn = _db.GetConnection();
            conn.Open();

            // Get all active vital definitions for this clinic.
            var vitalDefs = _vitalDefDAL.GetAll(ClinicId, conn);

            if (vitalDefs == null || vitalDefs.Count == 0)
                return result;

            /*
            * Convert supplied values to decimals.
            *
            * Only numeric values can participate in calculations.
            */
            var numericValues = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in values)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                    continue;

                if (decimal.TryParse(item.Value,
                                        System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        out decimal value))
                {
                    numericValues[item.Key] = value;
                }
            }

            /*
            * Calculate every derived vital
            */
            foreach (var vitalDef in vitalDefs)
            {
                if (!vitalDef.IsActive)
                    continue;

                if (!vitalDef.IsDerived)
                    continue;

                if (string.IsNullOrWhiteSpace(vitalDef.DeriveFormula))
                    continue;

                decimal? calculatedValue = Calculate(vitalDef.DeriveFormula, numericValues);

                /*
                * If one or more source values are missing,
                * the derived vital cannot be calculated.
                */
                if (!calculatedValue.HasValue)
                    continue;

                string vitalValue = calculatedValue.Value.ToString("0.##",
                                                        System.Globalization.CultureInfo.InvariantCulture);

                if (string.IsNullOrWhiteSpace(vitalValue))
                    continue;

                var vital = new Vital(vitalDef, ClinicId, LastVisitId);
                vital.VitalValue = vitalValue;
                result.Add(vital);
            }

            return result;
        }
        
    }

}

