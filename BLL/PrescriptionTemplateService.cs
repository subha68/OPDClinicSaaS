using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Helpers;

namespace BLL
{
    public class PrescriptionTemplateService : BaseService<PrescriptionTemplate>
    {
        private readonly PrescriptionTemplateDAL _templateDAL;

        public PrescriptionTemplateService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _templateDAL = new PrescriptionTemplateDAL(config);
            _dal = _templateDAL;
        }

        public PrescriptionTemplate GetByDepartment(int departmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetByDepartment(departmentId, conn);
            }
            catch
            {
                throw;
            }

        }

        public PrescriptionTemplate GetByDoctor(int doctorId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetByDoctor(doctorId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateSection> GetSections(int templateId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetSections(templateId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateField> GetFieldsForSection(int sectionId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetFieldsForSection(sectionId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateFieldOption> GetOptionsForField(int fieldId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetOptionsForField(fieldId, conn);
            }
            catch
            {
                throw;
            }

        }
    }

}