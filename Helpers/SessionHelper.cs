using Microsoft.AspNetCore.Http;

namespace Helpers
{
    public class SessionHelper
    {
        private readonly IHttpContextAccessor _context;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }

        public int ClinicId
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("ClinicId") ?? "0"); }
            set { _context.HttpContext.Session.SetString("ClinicId", value == null ? "" : value.ToString()); }
        }

        public int RoleId
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("RoleId") ?? "0"); }
            set { _context.HttpContext.Session.SetString("RoleId", value == null ? "" : value.ToString()); }
        }

        public string RoleName
        {
            get { return _context.HttpContext.Session.GetString("RoleName") ?? "0"; }
            set { _context.HttpContext.Session.SetString("RoleName", value == null ? "" : value.ToString()); }
        }

        public int UserId
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("UserId") ?? "0"); }
            set { _context.HttpContext.Session.SetString("UserId", value == null ? "" : value.ToString()); }
        }

        public string UserFirstName
        {
            get { return _context.HttpContext.Session.GetString("UserFirstName") ?? "0"; }
            set { _context.HttpContext.Session.SetString("UserFirstName", value == null ? "" : value.ToString()); }
        }

        public string UserLastName
        {
            get { return _context.HttpContext.Session.GetString("UserLastName") ?? "0"; }
            set { _context.HttpContext.Session.SetString("UserLastName", value == null ? "" : value.ToString()); }
        }

        public string UserDisplayName
        {
            get { return _context.HttpContext.Session.GetString("UserDisplayName") ?? "0"; }
            set { _context.HttpContext.Session.SetString("UserDisplayName", value == null ? "" : value.ToString()); }
        }

        public int PatientId
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("PatientId") ?? "0"); }
            set { _context.HttpContext.Session.SetString("PatientId", value == null ? "" : value.ToString()); }
        }
        
        public string PatientFirstName
        {
            get { return _context.HttpContext.Session.GetString("PatientFirstName") ?? ""; }
            set { _context.HttpContext.Session.SetString("PatientFirstName", value == null ? "" : value.ToString()); }
        }

        public string PatientLastName
        {
            get { return _context.HttpContext.Session.GetString("PatientLastName") ?? ""; }
            set { _context.HttpContext.Session.SetString("PatientLastName", value == null ? "" : value.ToString()); }
        }

        public string PatientDisplayName
        {
            get { return _context.HttpContext.Session.GetString("PatientDisplayName") ?? ""; }
            set { _context.HttpContext.Session.SetString("PatientDisplayName", value == null ? "" : value.ToString()); }
        }

        public string PatientGender
        {
            get { return _context.HttpContext.Session.GetString("PatientGender") ?? ""; }
            set { _context.HttpContext.Session.SetString("PatientGender", value == null ? "" : value.ToString()); }
        }

        public int PatientAge
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("PatientAge") ?? "0"); }
            set { _context.HttpContext.Session.SetString("PatientAge", value == null ? "" : value.ToString()); }
        }

        public string PatientPhone
        {
            get { return _context.HttpContext.Session.GetString("PatientPhone") ?? ""; }
            set { _context.HttpContext.Session.SetString("PatientPhone", value == null ? "" : value.ToString()); }
        }

        public int LastVisitId
        {
            get { return int.Parse(_context.HttpContext.Session.GetString("LastVisitId") ?? "0"); }
            set { _context.HttpContext.Session.SetString("LastVisitId", value == null ? "" : value.ToString()); }
        }

        public DateOnly? LastVisitDate
        {
            get { 
                var value = _context.HttpContext.Session.GetString("LastVisitDate");

                if (DateOnly.TryParse(value, out var date))
                    return date;

                return null;
                }
            set { _context.HttpContext.Session.SetString("LastVisitDate", value?.ToString("dd-MM-yyyy") ?? ""); }
        }
    }
}