using DAL;
using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace BLL
{
    public class TestDefinitionService : BaseService<TestPanel>
    {
        private readonly TestDefinitionDAL _testDefinitionDAL;

        public TestDefinitionService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
                                    : base(config, httpContextAccessor)
        {
            _testDefinitionDAL = new TestDefinitionDAL(config);

            _dal = _testDefinitionDAL;
        }

        // ============================================================
        // PANELS
        // ============================================================

        public List<TestPanel> ListPanels()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            int? clinicId = GetClinicId();

            if (clinicId.HasValue && clinicId.Value > 0)
                return _dal.GetAll(clinicId.Value, conn);
            else
                return _testDefinitionDAL.GetAllGlobal(conn);
        }

        public TestPanel GetPanel(int testPanelId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            int? clinicId = GetClinicId();

            if (clinicId.HasValue && clinicId.Value > 0)
                return _dal.Get(testPanelId, clinicId.Value, conn);
            else
                return _testDefinitionDAL.GetGlobal(testPanelId, conn);
        }

        public List<TestPanel> SearchPanel(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<TestPanel>();

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.SearchPanel(term.Trim(), clinicId, conn);
        }

        public int AddPanel(TestPanel panel)
        {
            ValidatePanel(panel);

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                panel.ClinicId = GetClinicId();
                bool result = _testDefinitionDAL.ShiftPanelsForInsert(panel.DisplayOrder, panel.ClinicId, conn, tran);
                int id = _dal.Add(panel, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool UpdatePanel(TestPanel panel)
        {
            ValidatePanel(panel);

            if (panel.TestPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                int? clinicId = GetClinicId();
                panel.ClinicId = clinicId;

                TestPanel existingPanel;
                
                if (clinicId.HasValue && clinicId.Value > 0)
                    existingPanel = _dal.Get(panel.TestPanelId, clinicId.Value, conn);
                else
                    existingPanel = _testDefinitionDAL.GetGlobal(panel.TestPanelId, conn);

                int oldOrder = existingPanel.DisplayOrder;

                bool result = _dal.Update(panel, conn, tran);
                _testDefinitionDAL.ShiftPanelsForUpdate(panel.TestPanelId, oldOrder, panel.DisplayOrder, 
                                                                    panel.ClinicId, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool RemovePanel(int testPanelId)
        {
            if (testPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                 * Remove panel-parameter mappings first because
                 * TestPanelParameters references TestPanels.
                 */
                bool result = false;

                int? clinicId = GetClinicId();

                TestPanel existingPanel;

                if (clinicId.HasValue && clinicId.Value > 0)
                    existingPanel = _dal.Get(testPanelId, clinicId.Value, conn, tran);
                else
                    existingPanel = _testDefinitionDAL.GetGlobal(testPanelId, conn, tran);

                int deletedOrder = existingPanel.DisplayOrder;

                _testDefinitionDAL.DissociateAllParametersOfPanel(testPanelId, clinicId, conn, tran);

                if (clinicId.HasValue && clinicId.Value > 0)
                    result = _dal.Delete(testPanelId, clinicId.Value, conn, tran);
                else
                    result = _testDefinitionDAL.DeleteGlobal(testPanelId, conn, tran);

                _testDefinitionDAL.ShiftPanelsOnDelete(deletedOrder, clinicId, conn, tran);

                tran.Commit();

                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void ReorderPanel(int panelId, int newDisplayOrder)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                TestPanel panel;

                int? clinicId = GetClinicId();
                
                if (clinicId.HasValue && clinicId.Value > 0)
                    panel = _dal.Get(panelId, clinicId.Value, conn, tran);
                else
                    panel = _testDefinitionDAL.GetGlobal(panelId, conn, tran);

                if (panel == null)
                {
                    return;
                }

                if (panel.DisplayOrder == newDisplayOrder)
                {
                    return;
                }

                _testDefinitionDAL.ShiftPanelsForUpdate(panelId, panel.DisplayOrder, newDisplayOrder, clinicId,
                                                        conn, tran);

                panel.DisplayOrder = newDisplayOrder;

                _dal.Update(panel, conn, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // ============================================================
        // PARAMETERS
        // ============================================================

        public List<TestParameter> ListParameters(int? panelId = null)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            int? clinicId = GetClinicId();

            return _testDefinitionDAL.GetAllParameter(panelId, clinicId, conn);
        }

        public TestParameter GetParameter(int testParameterId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            int? clinicId = GetClinicId();

            return _testDefinitionDAL.GetParameter(testParameterId, clinicId, conn);
        }

        public List<TestParameter> SearchParameter(string term, int? panelId = null)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<TestParameter>();

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.SearchParameter(term.Trim(), panelId, clinicId, conn);
        }

        public List<TestParameter> SearchUnassociatedParameter(string term, int panelId)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<TestParameter>();

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.SearchUnassociatedParameter(term.Trim(), panelId, clinicId, conn);
        }

        public bool AssociateParameterToPanel(TestPanelParameter association)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                int id;

                int? clinicId = GetClinicId();
                association.ClinicId = clinicId;

                _testDefinitionDAL.ShiftParametersForInsert(association.TestPanelId, association.DisplayOrder, 
                clinicId, conn, tran);

                _testDefinitionDAL.AddAssociation(association, conn, tran);

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
        
        public TestParameter AddParameterToPanel(int panelId, TestParameter parameter)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                int id;

                int? clinicId = GetClinicId();

                ValidateParameter(parameter);

                // Create Parameter, if new

                if (parameter.TestParameterId == 0)
                {
                    parameter.ClinicId = clinicId;
                    id = _testDefinitionDAL.AddParameter(parameter, conn, tran);
                    parameter.TestParameterId = id;
                }

                TestPanelParameter association = new TestPanelParameter
                {
                    ClinicId = clinicId,
                    TestPanelId = panelId,
                    TestParameterId = parameter.TestParameterId,
                    DisplayOrder = parameter.DisplayOrder
                };

                // Associate Parameter with Panel
                AssociateParameterToPanel(association);

                tran.Commit();
                return parameter;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public int GetNextPanelDisplayOrder()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _testDefinitionDAL.GetNextPanelDisplayOrder(GetClinicId(), conn);
            }
            catch
            {
                throw;
            }
            
        }

        public int GetNextParameterDisplayOrder(int panelId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (panelId <= 0)
                    return 1;

                return _testDefinitionDAL.GetNextParameterDisplayOrder(panelId, GetClinicId(), conn);
            }
            catch
            {
                throw;
            }
        }

        public void DissociateParameterFromPanel(int panelId, int parameterId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                if (parameterId == 0)
                    return;

                TestPanelParameter existingAssociation = 
                        _testDefinitionDAL.GetAssociationBYPanelAndParameter(panelId, parameterId, GetClinicId(), conn, tran);
                int oldOrder = existingAssociation.DisplayOrder;

                _testDefinitionDAL.DissociateParameterFromPanel(panelId, parameterId, GetClinicId(), conn, tran);

                _testDefinitionDAL.ShiftParametersOnDelete(panelId, oldOrder, GetClinicId(), conn, tran);

                tran.Commit();
                return;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool UpdateParameter(int panelId, TestParameter parameter)
        {
            ValidateParameter(parameter);

            if (parameter.TestParameterId <= 0)
                throw new ArgumentException("Invalid TestParameterId.");

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                parameter.ClinicId = GetClinicId();
                TestPanelParameter existingAssociation = 
                        _testDefinitionDAL.GetAssociationBYPanelAndParameter(panelId, parameter.TestParameterId, 
                                                                                    parameter.ClinicId, conn, tran);
                bool result = _testDefinitionDAL.UpdateParameter(parameter, conn, tran);
                _testDefinitionDAL.ShiftParametersForUpdate(panelId, parameter.TestParameterId, 
                                                                                existingAssociation.DisplayOrder, 
                                                                                parameter.DisplayOrder, GetClinicId(), 
                                                                                conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool RemoveParameter(int testParameterId)
        {
            if (testParameterId <= 0)
                throw new ArgumentException("Invalid TestParameterId.");

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                /*
                 * Remove mappings first because
                 * TestPanelParameters references TestParameters.
                 */
                bool result = false;

                List<TestPanel> panels = _testDefinitionDAL.GetPanelsByParameter(testParameterId, clinicId, conn, tran);

                foreach (var panel in panels)
                {
                    TestPanelParameter association = 
                        _testDefinitionDAL.GetAssociationBYPanelAndParameter(panel.TestPanelId, testParameterId, 
                                                                                        clinicId, conn, tran);
                    int oldOrder = association.DisplayOrder;

                    _testDefinitionDAL.DissociateParameterFromPanel(panel.TestPanelId, testParameterId, clinicId, conn, tran);

                    _testDefinitionDAL.ShiftParametersOnDelete(panel.TestPanelId, oldOrder, clinicId, conn, tran);
                }

                _testDefinitionDAL.DissociateAllPanelsOfParameter(testParameterId, clinicId, conn, tran);

                result = _testDefinitionDAL.DeleteParameter(testParameterId, clinicId, conn, tran);

                tran.Commit();

                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public void ReorderParameter(int panelId, int parameterId, int newDisplayOrder)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                TestPanelParameter mapping = _testDefinitionDAL.GetAssociationBYPanelAndParameter(panelId, parameterId,
                                                                                                GetClinicId(), conn, tran);

                if (mapping == null)
                {
                    return;
                }

                if (mapping.DisplayOrder == newDisplayOrder)
                {
                    return;
                }

                _testDefinitionDAL.ShiftParametersForUpdate(panelId, parameterId, mapping.DisplayOrder, newDisplayOrder,
                                                                    GetClinicId(), conn, tran);

                mapping.DisplayOrder = newDisplayOrder;

                _testDefinitionDAL.UpdateAssociation(mapping, conn, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }            
        }

        public List<TestParameter> ListUnassociatedParametersForPanel(int testPanelId)
        {
            if (testPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.GetUnassociatedParametersForPanel(testPanelId, clinicId, conn);
        }

        public List<TestParameter> ListExistingParametersOfPanel(int testPanelId, int? clinicId = null)
        {
            if (testPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.GetParametersByPanel(testPanelId, clinicId, conn);
        }

        public List<TestPanel> GetPanelsForParameter(int parameterId)
        {
            if (parameterId <= 0)
                return new List<TestPanel>();

            int? clinicId = GetClinicId();

            using var conn = _db.GetConnection();
            conn.Open();

            return _testDefinitionDAL.GetPanelsByParameter(parameterId, clinicId, conn);
        }

        public int CloneMasterDefinitions(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            if (clinicId <= 0)
            {
                throw new ArgumentException("Invalid clinic.");
            }

            int rowsAffected = _testDefinitionDAL.CloneMasterDefinitions(clinicId, conn, tran);

            return rowsAffected;
        }

        public int CloneMasterDefinitions(int clinicId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                int rowsAffected = CloneMasterDefinitions(clinicId, conn, tran);

                tran.Commit();

                return rowsAffected;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        // ============================================================
        // VALIDATION
        // ============================================================

        private void ValidatePanel(TestPanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            if (string.IsNullOrWhiteSpace(panel.Code))
                throw new ArgumentException("Panel code is required.");

            if (string.IsNullOrWhiteSpace(panel.Name))
                throw new ArgumentException("Panel name is required.");
        }

        private void ValidateParameter(TestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (string.IsNullOrWhiteSpace(parameter.Code))
                throw new ArgumentException("Parameter code is required.");

            if (string.IsNullOrWhiteSpace(parameter.Name))
                throw new ArgumentException("Parameter name is required.");

            if (string.IsNullOrWhiteSpace(parameter.DataType))
                throw new ArgumentException("Parameter data type is required.");
        }

        private void ValidateAssociation(TestPanelParameter panelParameter)
        {
            if (panelParameter == null)
                throw new ArgumentNullException(nameof(panelParameter));

            if (panelParameter.TestPanelId <= 0)
                throw new ArgumentException("Invalid TestPanelId.");

            if (panelParameter.TestParameterId <= 0)
                throw new ArgumentException("Invalid TestParameterId.");
        }

        private void ValidateAssociationScope(TestPanelParameter panelParameter, SqlConnection conn, SqlTransaction tran)
        {
            var panel = _dal.Get(panelParameter.TestPanelId, ClinicId, conn, tran);

            var parameter = _testDefinitionDAL.GetParameter(panelParameter.TestParameterId, ClinicId, conn, tran);

            if (panel.ClinicId != parameter.ClinicId)
            {
                throw new InvalidOperationException(
                    "Test panel and test parameter must belong " +
                    "to the same clinic/global definition scope.");
            }

            if (panel.ClinicId != panelParameter.ClinicId)
            {
                throw new InvalidOperationException(
                    "ClinicId does not match the panel and parameter scope.");
            }
        }

    }
}
