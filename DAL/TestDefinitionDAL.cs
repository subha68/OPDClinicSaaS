using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class TestDefinitionDAL : BaseDAL, IDAL<TestPanel>
    {
        private readonly DbHelper _db;

        public TestDefinitionDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        // Test Panel related data manipulation methods
        public int Add(TestPanel model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO TestPanels
                (ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder)
                VALUES
                (@ClinicId, @Code, @Name, @Description, @DepartmentName, @IsActive, @DisplayOrder);
                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", (object?)model.ClinicId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentName", (object?)model.DepartmentName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(TestPanel model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanels
                SET
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    DepartmentName = @DepartmentName,
                    IsActive = @IsActive,
                    DisplayOrder = @DisplayOrder
                WHERE TestPanelId = @TestPanelId";

            if (model.ClinicId == null || model.ClinicId == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", model.TestPanelId);

            if (model.ClinicId.HasValue && model.ClinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId.Value);
            
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentName", (object?)model.DepartmentName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanels
                WHERE TestPanelId = @TestPanelId";

            if (clinicId == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", id);
            if (clinicId > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteGlobal(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanels
                WHERE TestPanelId = @TestPanelId
                AND ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public TestPanel Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE TestPanelId = @TestPanelId
                  AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestPanel(reader);

            return null;
        }

        public TestPanel GetGlobal(int id, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE TestPanelId = @TestPanelId
                  AND ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelId", id);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestPanel(reader);

            return null;
        }

        public List<TestPanel> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestPanel>();

            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE ClinicId = @ClinicId
                ORDER BY DisplayOrder, Name";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestPanel(reader));
            }

            return list;
        }

        public List<TestPanel> GetAllGlobal(SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestPanel>();

            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE ClinicId IS NULL
                ORDER BY DisplayOrder, Name";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestPanel(reader));
            }

            return list;
        }
        
        public TestPanel GetPanelByCode(string code, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE Code = @Code";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId;";
            else
                query += " AND @ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@Code", code);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestPanel(reader);

            return null;
        }

        public List<TestPanel> SearchPanel(string term, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<TestPanel> panels = new List<TestPanel>();

            string query = @"
                SELECT TestPanelId, ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder, CreatedDate
                FROM TestPanels
                WHERE ((Code LIKE '%' + @term + '%')
                OR (Name LIKE '%' + @term + '%')
                OR (Description LIKE '%' + @term + '%'))";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId;";
            else
                query += " AND @ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@term", term);
            
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                panels.Add(MapTestPanel(reader));

            return panels;
        }

        // Test Panel related data manipulation methods
        public int AddParameter(TestParameter model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO TestParameters
                (ClinicId, Code, Name, Description, DataType, Unit, NormalRange, NormalMin, NormalMax, TestMethod, IsActive)
                VALUES
                (@ClinicId, @Code, @Name, @Description,
                    @DataType,
                    @Unit,
                    @NormalRange,
                    @NormalMin,
                    @NormalMax,
                    @TestMethod,
                    @IsActive
                );

                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", (object?)model.ClinicId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataType", model.DataType);
            cmd.Parameters.AddWithValue("@Unit", (object?)model.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalRange", (object?)model.NormalRange ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMin", (object?)model.NormalMin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMax", (object?)model.NormalMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestMethod", (object?)model.TestMethod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateParameter(TestParameter model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestParameters
                SET
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    DataType = @DataType,
                    Unit = @Unit,
                    NormalRange = @NormalRange,
                    NormalMin = @NormalMin,
                    NormalMax = @NormalMax,
                    TestMethod = @TestMethod,
                    IsActive = @IsActive
                WHERE TestParameterId = @TestParameterId";

            if (model.ClinicId == null || model.ClinicId == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestParameterId", model.TestParameterId);

            if (model.ClinicId.HasValue && model.ClinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", (object?)model.ClinicId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DataType", model.DataType);
            cmd.Parameters.AddWithValue("@Unit", (object?)model.Unit ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalRange", (object?)model.NormalRange ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMin", (object?)model.NormalMin ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NormalMax", (object?)model.NormalMax ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestMethod", (object?)model.TestMethod ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive ? 1 : 0);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteParameter(int id, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestParameters
                WHERE TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestParameterId", id);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public TestParameter GetParameter(int id, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestParameterId, ClinicId, Code, Name, Description, DataType, Unit, NormalRange,
                    NormalMin, NormalMax, TestMethod, IsActive, CreatedDate
                FROM TestParameters
                WHERE TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestParameterId", id);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestParameter(reader);

            return null;
        }

        public List<TestParameter> GetAllParameter(int? panelId, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestParameter>();

            string query;
            bool clinicIdGiven = clinicId.HasValue && clinicId.Value > 0;
            bool panelIdGiven = panelId.HasValue && panelId.Value > 0;

            if (panelIdGiven && clinicIdGiven)  // Case 1 - Both Panel Id and Clinic Id given
            {
                query = @"
                    SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit,
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate,
                        tpp.DisplayOrder
                    FROM TestParameters tp
                    INNER JOIN TestPanelParameters tpp
                        ON tpp.TestParameterId = tp.TestParameterId
                        AND tpp.ClinicId = tp.ClinicId
                    WHERE tpp.TestPanelId = @PanelId
                    AND tp.ClinicId = @ClinicId
                    ORDER BY tpp.DisplayOrder, tp.Name;";
            }
            else if (panelIdGiven && !clinicIdGiven)  // Case 2 - Panel Id given but No Clinic Id
            {
                query = @"
                    SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit,
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate,
                        tpp.DisplayOrder
                    FROM TestParameters tp
                    INNER JOIN TestPanelParameters tpp
                        ON tpp.TestParameterId = tp.TestParameterId
                        AND tpp.ClinicId IS NULL
                        AND tp.ClinicId IS NULL
                    WHERE tpp.TestPanelId = @PanelId
                    AND tp.ClinicId IS NULL
                    ORDER BY tpp.DisplayOrder, tp.Name;";                
            }
            else if (!panelIdGiven && clinicIdGiven)  // Case 3 - No Panel Id but Clinic Id given
            {
               query = @"
                    SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit,
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate
                    FROM TestParameters tp
                    WHERE tp.ClinicId = @ClinicId
                    ORDER BY tp.Name;"; 
            }
            else if (!panelIdGiven && !clinicIdGiven)  // Case 4 - Neither Panel Id Nor Clinic Id given
            {
               query = @"
                    SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit,
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate
                    FROM TestParameters tp
                    WHERE tp.ClinicId IS NULL
                    ORDER BY tp.Name;";                 
            }
            else
                return list;

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            if (panelIdGiven)
            {
                cmd.Parameters.AddWithValue("@PanelId", panelId.Value);
            }

            if (clinicIdGiven)
            {
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);
            }

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                TestParameter parameter = MapTestParameter(reader);

                if (panelId.HasValue && panelId.Value > 0)
                {
                    parameter.DisplayOrder = reader.GetInt32(reader.GetOrdinal("DisplayOrder"));
                }

                list.Add(parameter);
            }

            return list;
        }

        public TestParameter GetParameterByCode(string code, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestParameterId, ClinicId, Code, Name, Description, DataType, Unit, NormalRange,
                    NormalMin, NormalMax, TestMethod, IsActive, CreatedDate
                FROM TestParameters
                WHERE Code = @Code";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId;";
            else
                query += " AND ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@Code", code);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestParameter(reader);

            return null;
        }

        public bool ShiftPanelsForInsert(int newDisplayOrder, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanels
                SET DisplayOrder = DisplayOrder + 1
                WHERE DisplayOrder >= @NewDisplayOrder";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@NewDisplayOrder", newDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public bool ShiftPanelsForUpdate(int panelId, int oldDisplayOrder, int newDisplayOrder, int? clinicId,
                                                    SqlConnection conn, SqlTransaction tran)
        {
            if (oldDisplayOrder == newDisplayOrder)
                return true;

            string query;

            if (newDisplayOrder > oldDisplayOrder)
            {
                // Moving down: 2 -> 5
                query = @"
                    UPDATE TestPanels
                    SET DisplayOrder = DisplayOrder - 1
                    WHERE TestPanelId <> @TestPanelId
                    AND DisplayOrder > @OldDisplayOrder
                    AND DisplayOrder <= @NewDisplayOrder";
            }
            else
            {
                // Moving up: 5 -> 2
                query = @"
                    UPDATE TestPanels
                    SET DisplayOrder = DisplayOrder + 1
                    WHERE TestPanelId <> @TestPanelId
                    AND DisplayOrder >= @NewDisplayOrder
                    AND DisplayOrder < @OldDisplayOrder";
            }

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@OldDisplayOrder", oldDisplayOrder);
            cmd.Parameters.AddWithValue("@NewDisplayOrder", newDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public bool ShiftPanelsOnDelete(int deletedDisplayOrder, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanels
                SET DisplayOrder = DisplayOrder - 1
                WHERE DisplayOrder > @DeletedDisplayOrder";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@DeletedDisplayOrder", deletedDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public bool ShiftParametersForInsert(int panelId, int newDisplayOrder, int? clinicId, 
                                                SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanelParameters
                SET DisplayOrder = DisplayOrder + 1
                WHERE TestPanelId = @TestPanelId
                AND DisplayOrder >= @NewDisplayOrder";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@NewDisplayOrder", newDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public bool ShiftParametersForUpdate(int panelId, int parameterId, int oldDisplayOrder, int newDisplayOrder, 
                                                        int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            if (oldDisplayOrder == newDisplayOrder)
                return true;

            string query;

            if (newDisplayOrder > oldDisplayOrder)
            {
                // Moving down: 2 -> 5
                query = @"
                    UPDATE TestPanelParameters
                    SET DisplayOrder = DisplayOrder - 1
                    WHERE TestPanelId = @TestPanelId
                    AND TestParameterId <> @TestParameterId
                    AND DisplayOrder > @OldDisplayOrder
                    AND DisplayOrder <= @NewDisplayOrder";
            }
            else
            {
                // Moving up: 5 -> 2
                query = @"
                    UPDATE TestPanelParameters
                    SET DisplayOrder = DisplayOrder + 1
                    WHERE TestPanelId = @TestPanelId
                    AND TestParameterId <> @TestParameterId
                    AND DisplayOrder >= @NewDisplayOrder
                    AND DisplayOrder < @OldDisplayOrder";
            }

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@TestParameterId", parameterId);
            cmd.Parameters.AddWithValue("@OldDisplayOrder", oldDisplayOrder);
            cmd.Parameters.AddWithValue("@NewDisplayOrder", newDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public bool ShiftParametersOnDelete(int panelId, int deletedDisplayOrder, int? clinicId,
                                                SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanelParameters
                SET DisplayOrder = DisplayOrder - 1
                WHERE TestPanelId = @TestPanelId
                AND DisplayOrder > @DeletedDisplayOrder";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId";
            else
                query += " AND ClinicId IS NULL";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@DeletedDisplayOrder", deletedDisplayOrder);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            cmd.ExecuteNonQuery();

            return true;
        }

        public List<TestParameter> SearchParameter(string term, int? panelId, int? clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<TestParameter> parameters = new List<TestParameter>();

            string query = @"
                SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit, tp.NormalRange,
                    tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate
                FROM TestParameters tp
                WHERE ((tp.Code LIKE '%' + @term + '%')
                OR (tp.Name LIKE '%' + @term + '%')
                OR (tp.Description LIKE '%' + @term + '%'))";

            if (panelId.HasValue && panelId.Value > 0)
                query += @" AND EXISTS (SELECT 1 FROM TestPanelParameters tpp
                                            WHERE tpp.TestParameterId = tp.TestParameterId
                                            AND tpp.TestPanelId = @PanelId)";
            else
                query += "";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND tp.ClinicId = @ClinicId;";
            else
                query += " AND tp.ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@term", term);
            if (panelId.HasValue && panelId.Value > 0)
                cmd.Parameters.AddWithValue("@PanelId", panelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                parameters.Add(MapTestParameter(reader));

            return parameters;
        }

        public List<TestParameter> SearchUnassociatedParameter(string term, int panelId, int? clinicId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<TestParameter> parameters = new List<TestParameter>();

            if (panelId <= 0)
                return parameters;

            string query = @"
                SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit, tp.NormalRange,
                    tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate
                FROM TestParameters tp
                WHERE ((Code LIKE '%' + @term + '%')
                OR (Name LIKE '%' + @term + '%')
                OR (Description LIKE '%' + @term + '%'))
                AND NOT EXISTS (SELECT 1 FROM TestPanelParameters tpp
                                            WHERE tpp.TestParameterId = tp.TestParameterId
                                            AND tpp.TestPanelId = @PanelId)";

            if (clinicId.HasValue && clinicId.Value > 0)
                query += " AND ClinicId = @ClinicId;";
            else
                query += " AND ClinicId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@term", term);
            cmd.Parameters.AddWithValue("@PanelId", panelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
                parameters.Add(MapTestParameter(reader));

            return parameters;
        }
        
        public int AddAssociation(TestPanelParameter model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO TestPanelParameters
                (ClinicId, TestPanelId, TestParameterId, DisplayOrder, IsActive)
                VALUES
                (@ClinicId, @TestPanelId, @TestParameterId, @DisplayOrder, @IsActive);

                SELECT SCOPE_IDENTITY();";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", (object?)model.ClinicId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestPanelId", model.TestPanelId);
            cmd.Parameters.AddWithValue("@TestParameterId", model.TestParameterId);
            cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool UpdateAssociation(TestPanelParameter model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE TestPanelParameters
                SET
                    DisplayOrder = @DisplayOrder,
                    IsActive = @IsActive
                WHERE TestPanelParameterId = @TestPanelParameterId";

            if (!model.ClinicId.HasValue || model.ClinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelParameterId", model.TestPanelParameterId);
            if (model.ClinicId.HasValue && model.ClinicId > 0)
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteAssociation(int id, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanelParameters
                WHERE TestPanelParameterId = @TestPanelParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelParameterId", id);

            if (clinicId.HasValue && clinicId > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DissociateAllParametersOfPanel(int panelId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanelParameters
                WHERE TestPanelId = @TestPanelId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DissociateAllPanelsOfParameter(int parameterId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanelParameters
                WHERE TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestParameterId", parameterId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DissociateParameterFromPanel(int panelId, int parameterId, int? clinicId, 
                                                                SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM TestPanelParameters
                WHERE TestPanelId = @TestPanelId
                AND TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@TestParameterId", parameterId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            return cmd.ExecuteNonQuery() > 0;
        }

        public TestPanelParameter GetAssociation(int id, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestPanelParameterId, ClinicId, TestPanelId, TestParameterId, DisplayOrder, IsActive
                FROM TestPanelParameters
                WHERE TestPanelParameterId = @TestPanelParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelParameterId", id);

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestPanelParameter(reader);

            return null;
        }

        public TestPanelParameter GetAssociationBYPanelAndParameter(int panelId, int parameterId, int? clinicId, 
        SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT TestPanelParameterId, ClinicId, TestPanelId, TestParameterId, DisplayOrder, IsActive
                FROM TestPanelParameters
                WHERE TestPanelId = @TestPanelId AND TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelId", panelId);
            cmd.Parameters.AddWithValue("@TestParameterId", parameterId );

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                return MapTestPanelParameter(reader);

            return null;
        }

        public List<TestPanelParameter> GetAllAssociations(int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestPanelParameter>();

            string query = @"
                SELECT TestPanelParameterId, ClinicId, TestPanelId, TestParameterId, DisplayOrder, IsActive
                FROM TestPanelParameters";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL;";
            else
                query += " AND ClinicId = @ClinicId";

            query += " ORDER BY TestPanelId, DisplayOrder";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            if (clinicId.HasValue && clinicId.Value > 0)
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(MapTestPanelParameter(reader));
            }

            return list;
        }

        public List<TestParameter> GetUnassociatedParametersForPanel(int testPanelId, int? clinicId, 
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestParameter>();

            string query = @"
                SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit, 
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate
                FROM TestParameters tp
                WHERE NOT EXISTS (SELECT 1
                                    FROM TestPanelParameters tpp
                                    WHERE tpp.TestParameterId = tp.TestParameterId
                                    AND tpp.TestPanelId = @TestPanelId)";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += @" AND tp.ClinicId IS NULL
                            AND tpp.ClinicId IS NULL";
            else
                query += @" AND tp.ClinicId = @ClinicId
                            AND tpp.ClinicId = @ClinicId";

            query += " ORDER BY tp.Name";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelId", testPanelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var parameter = MapTestParameter(reader);
                list.Add(parameter);
            }

            return list;
        }

        public List<TestParameter> GetParametersByPanel(int testPanelId, int? clinicId, 
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestParameter>();

            string query = @"
                SELECT tp.TestParameterId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit, 
                        tp.NormalRange, tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive, tp.CreatedDate, tpp.DisplayOrder
                FROM TestPanelParameters tpp
                INNER JOIN TestParameters tp
                    ON tpp.TestParameterId = tp.TestParameterId
                WHERE tpp.TestPanelId = @TestPanelId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += @" AND tp.ClinicId IS NULL
                            AND tpp.ClinicId IS NULL";
            else
                query += @" AND tp.ClinicId = @ClinicId
                            AND tpp.ClinicId = @ClinicId";

            query += " ORDER BY tpp.DisplayOrder";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestPanelId", testPanelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var parameter = MapTestParameter(reader);
                parameter.DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]);
                list.Add(parameter);
            }

            return list;
        }

        public List<TestPanel> GetPanelsByParameter(int testParameterId, int? clinicId, 
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<TestPanel>();

            string query = @"
                SELECT tp.TestPanelId, tp.ClinicId, tp.Code, tp.Name, tp.Description, tp.DepartmentName, tp.IsActive, 
                        tp.DisplayOrder, tp.CreatedDate
                FROM TestPanelParameters tpp
                INNER JOIN TestPanels tp
                    ON tpp.TestPanelId = tp.TestPanelId
                WHERE tpp.TestParameterId = @TestParameterId";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += @" AND tp.ClinicId IS NULL
                            AND tpp.ClinicId IS NULL";
            else
                query += @" AND tp.ClinicId = @ClinicId
                            AND tpp.ClinicId = @ClinicId";

            query += " ORDER BY tp.DisplayOrder";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@TestParameterId", testParameterId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var panel = MapTestPanel(reader);
                list.Add(panel);
            }

            return list;
        }

        public int GetNextPanelDisplayOrder(int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT
                    ISNULL(MAX(DisplayOrder), 0) + 1 AS DisplayOrder
                FROM TestPanels
                WHERE IsActive = 1";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int displayOrder = Convert.ToInt32(reader["DisplayOrder"]);
                return displayOrder;
            }

            return 1;
        }

        public int GetNextParameterDisplayOrder(int panelId, int? clinicId, 
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"
                SELECT
                    ISNULL(MAX(DisplayOrder), 0) + 1 AS DisplayOrder
                FROM TestPanelParameters
                WHERE TestPanelId = @PanelId
                AND IsActive = 1";

            if (!clinicId.HasValue || clinicId.Value == 0)
                query += " AND ClinicId IS NULL";
            else
                query += " AND ClinicId = @ClinicId";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@PanelId", panelId);
            if (clinicId.HasValue && clinicId.Value > 0)
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.Value);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                int displayOrder = Convert.ToInt32(reader["DisplayOrder"]);
                return displayOrder;
            }

            return 1;
        }

        public int CloneMasterDefinitions(int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string queryPanel = @"
                INSERT INTO TestPanels
                    (ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder)
                SELECT
                    @ClinicId, tp.Code, tp.Name, tp.Description, tp.DepartmentName, tp.IsActive, tp.DisplayOrder
                FROM TestPanels tp
                WHERE tp.ClinicId IS NULL
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM TestPanels existing
                    WHERE existing.ClinicId = @ClinicId
                    AND existing.Code = tp.Code
                );
                
                SELECT @@ROWCOUNT;";

            using SqlCommand cmdPanel = new SqlCommand(queryPanel, conn, tran);

            cmdPanel.Parameters.AddWithValue("@ClinicId", clinicId);

            int insertedPanelCount = Convert.ToInt32(cmdPanel.ExecuteScalar());

            string queryParameter = @"
                        INSERT INTO TestParameters
                            (ClinicId, Code, Name, Description, DataType, Unit, NormalRange, 
                            NormalMin, NormalMax, TestMethod, IsActive)
                        SELECT
                            @ClinicId, tp.Code, tp.Name, tp.Description, tp.DataType, tp.Unit, tp.NormalRange, 
                            tp.NormalMin, tp.NormalMax, tp.TestMethod, tp.IsActive
                        FROM TestParameters tp
                        WHERE tp.ClinicId IS NULL
                        AND NOT EXISTS
                        (
                            SELECT 1
                            FROM TestParameters existing
                            WHERE existing.ClinicId = @ClinicId
                            AND existing.Code = tp.Code
                        );
                        
                        SELECT @@ROWCOUNT;";

            using SqlCommand cmdParameter = new SqlCommand(queryParameter, conn, tran);

            cmdParameter.Parameters.AddWithValue("@ClinicId", clinicId);

            int insertedParameterCount = Convert.ToInt32(cmdParameter.ExecuteScalar());

            string queryAssociation = @"
                INSERT INTO TestPanelParameters
                    (ClinicId, TestPanelId, TestParameterId, DisplayOrder, IsActive)
                SELECT
                    @ClinicId, clinicPanel.TestPanelId, clinicParameter.TestParameterId, 
                        masterLink.DisplayOrder, masterLink.IsActive
                FROM TestPanelParameters masterLink

                INNER JOIN TestPanels masterPanel
                    ON masterPanel.TestPanelId = masterLink.TestPanelId
                    AND masterPanel.ClinicId IS NULL

                INNER JOIN TestParameters masterParameter
                    ON masterParameter.TestParameterId = masterLink.TestParameterId
                    AND masterParameter.ClinicId IS NULL

                INNER JOIN TestPanels clinicPanel
                    ON clinicPanel.ClinicId = @ClinicId
                    AND clinicPanel.Code = masterPanel.Code

                INNER JOIN TestParameters clinicParameter
                    ON clinicParameter.ClinicId = @ClinicId
                    AND clinicParameter.Code = masterParameter.Code

                WHERE masterLink.ClinicId IS NULL

                AND NOT EXISTS
                (
                    SELECT 1
                    FROM TestPanelParameters existing
                    WHERE existing.ClinicId = @ClinicId
                    AND existing.TestPanelId = clinicPanel.TestPanelId
                    AND existing.TestParameterId = clinicParameter.TestParameterId
                );
                
                SELECT @@ROWCOUNT;";

            using SqlCommand cmdAssociation = new SqlCommand(queryAssociation, conn, tran);

            cmdAssociation.Parameters.AddWithValue("@ClinicId", clinicId);

            int insertedAssociationCount = Convert.ToInt32(cmdAssociation.ExecuteScalar());

            int insertedRowCount = insertedParameterCount == 0 ? insertedAssociationCount : insertedParameterCount;

            return insertedRowCount;

        }

        private TestPanel MapTestPanel(SqlDataReader reader)
        {
            return new TestPanel
            {
                TestPanelId = Convert.ToInt32(reader["TestPanelId"]),
                ClinicId = reader["ClinicId"] == DBNull.Value ? null : Convert.ToInt32(reader["ClinicId"]),
                Code = reader["Code"].ToString() ?? string.Empty,
                Name = reader["Name"].ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),
                DepartmentName = reader["DepartmentName"] == DBNull.Value
                    ? null
                    : reader["DepartmentName"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };
        }

        private TestParameter MapTestParameter(SqlDataReader reader)
        {
            return new TestParameter
            {
                TestParameterId = Convert.ToInt32(reader["TestParameterId"]),
                ClinicId = reader["ClinicId"] == DBNull.Value ? null : Convert.ToInt32(reader["ClinicId"]),
                Code = reader["Code"].ToString() ?? string.Empty,
                Name = reader["Name"].ToString() ?? string.Empty,
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),
                DataType = reader["DataType"].ToString() ?? "Numeric",
                Unit = reader["Unit"] == DBNull.Value
                    ? null
                    : reader["Unit"].ToString(),
                NormalRange = reader["NormalRange"] == DBNull.Value
                    ? null
                    : reader["NormalRange"].ToString(),
                NormalMin = reader["NormalMin"] == DBNull.Value
                    ? null
                    : Convert.ToDecimal(reader["NormalMin"]),
                NormalMax = reader["NormalMax"] == DBNull.Value
                    ? null
                    : Convert.ToDecimal(reader["NormalMax"]),
                TestMethod = reader["TestMethod"] == DBNull.Value
                    ? null
                    : reader["TestMethod"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
            };

        }

        private TestPanelParameter MapTestPanelParameter(SqlDataReader reader)
        {
            return new TestPanelParameter
            {
                TestPanelParameterId = Convert.ToInt32(reader["TestPanelParameterId"]),
                ClinicId = reader["ClinicId"] == DBNull.Value ? null : Convert.ToInt32(reader["ClinicId"]),
                TestPanelId = Convert.ToInt32(reader["TestPanelId"]),
                TestParameterId = Convert.ToInt32(reader["TestParameterId"]),
                DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                IsActive = Convert.ToBoolean(reader["IsActive"])
            };
        }

    }
}
