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
    public class VitalDefinitionService : BaseService<VitalDefinition>
    {
        private readonly VitalDefinitionDAL _vitalDefDAL;

        public VitalDefinitionService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                            : base(config, httpContextAccessor)
        {
            _vitalDefDAL = new VitalDefinitionDAL(config);
            _dal = _vitalDefDAL;
        }

        public override VitalDefinition Get(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (RoleName == "SuperAdmin")
                    return _vitalDefDAL.GetDefault(id, conn);
                else if (RoleName == "ClinicAdmin")
                    return _vitalDefDAL.Get(id, ClinicId, conn);
                else
                    return new VitalDefinition();                
            }
            catch
            {
                throw;
            }
            
        }

        public override List<VitalDefinition> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (RoleName == "SuperAdmin")
                    return _vitalDefDAL.GetAllDefault(conn);
                else if (RoleName == "ClinicAdmin")
                    return _vitalDefDAL.GetAll(ClinicId, conn);
                else
                    return new List<VitalDefinition>();
            }
            catch
            {
                throw;
            }            
        }

        public int CloneDefaults(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return _vitalDefDAL.CloneVitalDefinitions(clinicId, conn, tran);
        }
        
        public int CloneDefaults()
        {
            int rowAffected = 0;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                rowAffected = CloneDefaults(ClinicId, conn, tran);

                tran.Commit();
                return rowAffected;
            }
            catch
            {
                tran.Rollback();
                throw;
            }            
        }

    }

}