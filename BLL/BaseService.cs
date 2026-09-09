using DAL;
using Models;
using Helpers;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace BLL
{
    public abstract class BaseService<T>
    {
        public IDAL<T> _dal;

        protected readonly CalculationEngine _calculationEngine;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionHelper _session;
        public readonly DbHelper _db;

        protected BaseService()
        {
            _calculationEngine = new CalculationEngine();
        }

        protected BaseService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _calculationEngine = new CalculationEngine();
            _httpContextAccessor = httpContextAccessor;
            _session = new SessionHelper(_httpContextAccessor);
            _db = new DbHelper(config);
        }

        public int ClinicId
        {
            get => _session.ClinicId; 
            set => _session.ClinicId = value; 
        }
        
        public int RoleId
        {
            get => Convert.ToInt32(_session.RoleId);
            set => _session.RoleId = value;
        }

        public string RoleName
        {
            get => _session.RoleName;
            set => _session.RoleName = value;
        }

        public int UserId
        {
            get => Convert.ToInt32(_session.UserId);
            set => _session.UserId = value;
        }

        public string UserFirstName
        {
            get => _session.UserFirstName;
            set => _session.UserFirstName = value;
        }

        public string UserLastName
        {
            get => _session.UserLastName;
            set => _session.UserLastName = value;
        }

        public string UserDisplayName
        {
            get => _session.UserDisplayName;
            set => _session.UserDisplayName = value;
        }

        public int PatientId
        {
            get => Convert.ToInt32(_session.PatientId);
            set => _session.PatientId = value;
        }

        public string PatientFirstName
        {
            get => _session.PatientFirstName;
            set => _session.PatientFirstName = value;
        }

        public string PatientLastName
        {
            get => _session.PatientLastName;
            set => _session.PatientLastName = value;
        }

        public string PatientDisplayName
        {
            get => _session.PatientDisplayName;
            set => _session.PatientDisplayName = value;
        }

        public string PatientGender
        {
            get => _session.PatientGender;
            set => _session.PatientGender = value;
        }

        public int PatientAge
        {
            get => Convert.ToInt32(_session.PatientAge);
            set => _session.PatientAge = value;
        }

        public string PatientPhone
        {
            get => _session.PatientPhone;
            set => _session.PatientPhone = value;
        }

        public int LastVisitId
        {
            get => Convert.ToInt32(_session.LastVisitId);
            set => _session.LastVisitId = value;
        }

        public DateOnly? LastVisitDate
        {
            get => _session.LastVisitDate;
            set => _session.LastVisitDate = value;
        }

        public int? GetClinicId()
        {
            return RoleName == "SuperAdmin" ? null : ClinicId;
        }

        public virtual int Add(T model, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Add(model, conn, tran); 
        }

        public virtual bool Update(T model, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Update(model, conn, tran);
        }

        public virtual bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            return _dal.Delete(id, ClinicId, conn, tran);
        }

        public virtual T Get(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _dal.Get(id, ClinicId, conn);
            }
            catch
            {
                throw;
            }
            
        }

        public virtual List<T> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _dal.GetAll(ClinicId, conn);
            }
            catch
            {
                throw;
            }
            
        }

        public virtual T Get(int id, SqlConnection conn, SqlTransaction tran=null)
        {
            return _dal.Get(id, ClinicId, conn, tran);
        }

        public virtual List<T> List(SqlConnection conn, SqlTransaction tran=null)
        {
            return _dal.GetAll(ClinicId, conn, tran);
        }

        public virtual int Create(T model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int id = Add(model, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public virtual bool Modify(T model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Update(model, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
               tran.Rollback();
               throw; 
            }
        }


        public virtual bool Remove(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = Delete(id, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        protected string NormalizeDataType(string? dataType)
        {
            return (dataType ?? "")
                .Trim()
                .ToLowerInvariant();
        }

        protected DateOnly FormatDate(object value)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly;
            }
            else if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }
            else if (DateOnly.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid date.");
            }
        }

        protected DateTime FormatDateTime(object value)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            else if (value is DateTime dt)
            {
                return dt;
            }
            else if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid date time.");
            }
        }

        protected TimeSpan FormatTime(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }
            else if (value is DateTime dateTime)
            {
                return dateTime.TimeOfDay;
            }
            else if (TimeSpan.TryParse(value.ToString(), CultureInfo.InvariantCulture, out TimeSpan parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid time.");
            }
        }

        protected Decimal FormatDecimal(object value)
        {
            if (decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid decimal.");
            }
        }

        protected int FormatInteger(object value)
        {
            if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid integer.");
            }
        }

        protected long FormatLong(object value)
        {
            if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid long integer.");
            }
        }

        protected short FormatShort(object value)
        {
            if (short.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out short parsed))
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid long integer.");
            }
        }

        protected bool FormatBoolean(object value)
        {
            if (value is bool parsed)
            {
                return parsed;
            }
            else
            {
                throw new ArgumentException($"'{value.ToString()}' is not a valid boolean.");
            }
        }

        protected string FormatGeneralValue(object value, string? formatPattern)
        {
            if (value is IEnumerable<string> stringValues)
            {
                return string.Join(", ", stringValues);
            }

            if (!string.IsNullOrWhiteSpace(formatPattern) && value is IFormattable formattable)
            {
                return formattable.ToString(formatPattern, CultureInfo.InvariantCulture) ?? "";
            }

            return value.ToString() ?? "";
        }

        protected decimal? Calculate(string formula, IDictionary<string, decimal> values)
        {
            return _calculationEngine.Calculate(formula, values);
        }

    }


}