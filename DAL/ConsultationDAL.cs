using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DAL
{
    public class ConsultationDAL : BaseDAL, IDAL<Consultation>
    {
        private readonly DbHelper _db;

        public ConsultationDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Consultation model, SqlConnection conn, SqlTransaction tran)
        {
            int consultationId = 0;

            string queryConsultation = @"
                INSERT INTO Consultations
                (AppointmentId, TemplateId, TemplateName, ClinicId, Status, CreatedBy)
                VALUES
                (@AppointmentId, @TemplateId, @TemplateName, @ClinicId, @Status, @CreatedBy);
                                    
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdConsultation = new SqlCommand(queryConsultation, conn, tran);
            cmdConsultation.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);
            cmdConsultation.Parameters.AddWithValue("@TemplateId", model.TemplateId);
            cmdConsultation.Parameters.AddWithValue("@TemplateName", model.TemplateName);
            cmdConsultation.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmdConsultation.Parameters.AddWithValue("@Status", (int)model.Status);
            cmdConsultation.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

            consultationId = Convert.ToInt32(cmdConsultation.ExecuteScalar());

            return consultationId;
        }

        public int AddSections(List<ConsultationSection> sections, SqlConnection conn, SqlTransaction tran)
        {
            int sectionId = 0;

            string querySection = @"
                INSERT INTO ConsultationSections
                (ConsultationId, ClinicId, SectionName, DisplayOrder, IsVisible, Multiline, IsPharmacologyAdvice,
                IsVital, IsTestResult)
                VALUES
                (@ConsultationId, @ClinicId, @SectionName, @DisplayOrder, @IsVisible, @Multiline, @IsPharmacologyAdvice,
                @IsVital, @IsTestResult);

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdSection = new SqlCommand(querySection, conn, tran);

            foreach (var section in sections) 
            {
                cmdSection.Parameters.Clear();
                cmdSection.Parameters.AddWithValue("@ConsultationId", section.ConsultationId);
                cmdSection.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                cmdSection.Parameters.AddWithValue("@SectionName", section.SectionName);
                cmdSection.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                cmdSection.Parameters.AddWithValue("@IsVisible", section.IsVisible ? 1 : 0);
                cmdSection.Parameters.AddWithValue("@Multiline", section.Multiline ? 1 : 0);
                cmdSection.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice ? 1 : 0);
                cmdSection.Parameters.AddWithValue("@IsVital", section.IsVital ? 1 : 0);
                cmdSection.Parameters.AddWithValue("@IsTestResult", section.IsTestResult ? 1 : 0);

                sectionId = Convert.ToInt32(cmdSection.ExecuteScalar());

                section.SectionId = sectionId;
            }

            return sectionId;
        }

        public int AddTemplateRow(ConsultationRow row, SqlConnection conn, SqlTransaction tran)
        {
            int rowId = 0;

            string queryRow = @"
                INSERT INTO ConsultationRows
                (SectionId, ClinicId, RowNumber, DisplayOrder, IsTemplate, CreatedOn)
                VALUES
                (@SectionId, @ClinicId, @RowNumber, @DisplayOrder, 1, GETDATE());
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            cmdRow.Parameters.AddWithValue("@SectionId", row.SectionId);
            cmdRow.Parameters.AddWithValue("@ClinicId", row.ClinicId);
            cmdRow.Parameters.AddWithValue("@RowNumber", row.RowNumber);
            cmdRow.Parameters.AddWithValue("@DisplayOrder", row.DisplayOrder);

            rowId = Convert.ToInt32(cmdRow.ExecuteScalar());

            row.RowId = rowId;

            return rowId;
        }

        public int AddRows(List<ConsultationRow> rows, SqlConnection conn, SqlTransaction tran)
        {
            int rowId = 0;

            string queryRow = @"
                INSERT INTO ConsultationRows
                (SectionId, ClinicId, RowNumber, DisplayOrder, IsTemplate, CreatedOn)
                VALUES
                (@SectionId, @ClinicId, @RowNumber, @DisplayOrder, 0, GETDATE());
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);
            foreach (var row in rows) 
            {
                cmdRow.Parameters.Clear();
                cmdRow.Parameters.AddWithValue("@SectionId", row.SectionId);
                cmdRow.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                cmdRow.Parameters.AddWithValue("@RowNumber", row.RowNumber);
                cmdRow.Parameters.AddWithValue("@DisplayOrder", row.DisplayOrder);

                rowId = Convert.ToInt32(cmdRow.ExecuteScalar());

                row.RowId = rowId;
            }

            return rowId;
        }

        public int AddFields(List<ConsultationField> fields, SqlConnection conn, SqlTransaction tran)
        {
            int fieldId = 0;

            string queryField = @"
                INSERT INTO ConsultationFields
                (RowId, ClinicId, FieldLabel, FieldTypeCode, FieldCode,
                    FieldSource, FieldDataId, DisplayOrder, IsMandatory, FieldValue)
                VALUES
                (@RowId, @ClinicId, @FieldLabel, @FieldTypeCode, @FieldCode,
                    @Fieldsource, @FieldDataId, @DisplayOrder, @IsMandatory, @FieldValue);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);
            foreach (var field in fields) 
            {
                cmdField.Parameters.Clear();
                cmdField.Parameters.AddWithValue("@RowId", field.RowId);
                cmdField.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                cmdField.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                cmdField.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                cmdField.Parameters.AddWithValue("@FieldCode", field.FieldCode ?? (object)DBNull.Value);
                cmdField.Parameters.AddWithValue("@FieldSource", field.FieldSource ?? (object)DBNull.Value);
                cmdField.Parameters.AddWithValue("@FieldDataId", field.FieldDataId ?? 0);
                cmdField.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                cmdField.Parameters.AddWithValue("@IsMandatory", field.IsMandatory);
                cmdField.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? (object)DBNull.Value);

                fieldId = Convert.ToInt32(cmdField.ExecuteScalar());

                field.FieldId = fieldId;
            }

            return fieldId;
        }

        public int AddOptions(List<ConsultationFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            int optionId = 0;

            string queryOption = @"
                INSERT INTO ConsultationFieldOptions
                (FieldId, ClinicId, OptionText, IsSelected, DisplayOrder)
                VALUES
                (@FieldId, @ClinicId, @OptionText, @IsSelected, @DisplayOrder);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);
            foreach (var option in options)
            {
                cmdOption.Parameters.Clear();
                cmdOption.Parameters.AddWithValue("@FieldId", option.FieldId);
                cmdOption.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                cmdOption.Parameters.AddWithValue("@OptionText", option.OptionText);
                cmdOption.Parameters.AddWithValue("@IsSelected", option.IsSelected);
                cmdOption.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);

                optionId = Convert.ToInt32(cmdOption.ExecuteScalar());

                option.OptionId = optionId;
            }

            return optionId;
        }

        public bool Update(Consultation model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Consultations
                SET Status = @Status
                WHERE ConsultationId = @ConsultationId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ConsultationId", model.ConsultationId);
            cmd.Parameters.AddWithValue("@Status", (int)model.Status);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateSections(List<ConsultationSection> sections, SqlConnection conn, SqlTransaction tran)
        {
            // nothing can be updated at Section level during consultation
            return true;
        }

        public bool UpdateRows(List<ConsultationRow> rows, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"
                UPDATE ConsultationRows
                SET ModifiedOn = GETDATE()
                WHERE RowId = @RowId
                AND ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            foreach (var row in rows)
            {
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                //cmd.Parameters.AddWithValue("@SectionId", row.SectionId);
                cmd.Parameters.AddWithValue("@RowId", row.RowId);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool UpdateFields(List<ConsultationField> fields, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE ConsultationFields
                SET FieldDataId = @FieldDataId,
                    FieldValue = @FieldValue
                WHERE FieldId = @FieldId
                AND ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            int rowsAffected = 0;

            foreach (var field in fields)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                cmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                cmd.Parameters.AddWithValue("@FieldDataId", field.FieldDataId ?? 0);
                cmd.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? (object)DBNull.Value);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool UpdateOptions(List<ConsultationFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE ConsultationFieldOptions
                SET IsSelected = @IsSelected
                WHERE OptionId = @OptionId
                AND ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            int rowsAffected = 0;

            foreach (var option in options)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@IsSelected", option.IsSelected);
                cmd.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                cmd.Parameters.AddWithValue("@OptionId", option.OptionId);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool Delete(int consultationId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string queryOption = @"DELETE CFO
                                    FROM ConsultationFieldOptions CFO
                                    INNER JOIN ConsultationFields CF
                                        ON CF.FieldId = CFO.FieldId
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CF.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CFO.ClinicId = @ClinicId;";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);
            cmdOption.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdOption.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsOptionAffected = cmdOption.ExecuteNonQuery();

            string queryField = @"DELETE CF
                                    FROM ConsultationFields CF
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CF.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CF.ClinicId = @ClinicId;";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);
            cmdField.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdField.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsFieldAffected = cmdField.ExecuteNonQuery();

            string queryVital = @"DELETE CV
                                    FROM ConsultationVitals CV
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CV.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CV.ClinicId = @ClinicId;";
            SqlCommand cmdVital = new SqlCommand(queryVital, conn, tran);
            cmdVital.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdVital.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsVitalAffected = cmdVital.ExecuteNonQuery();

            string queryTestResult = @"DELETE CT
                                    FROM ConsultationTestResults CT
                                    INNER JOIN ConsultationTestResultGroups CG
                                        ON CT.ConsultationTestResultGroupId = CG.ConsultationTestResultGroupId
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CG.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CT.ClinicId = @ClinicId;";
            SqlCommand cmdTestResult = new SqlCommand(queryTestResult, conn, tran);
            cmdTestResult.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdTestResult.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsTestResultAffected = cmdTestResult.ExecuteNonQuery();

            string queryTestResultGroup = @"DELETE CG
                                    FROM ConsultationTestResultGroups CG
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CG.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CT.ClinicId = @ClinicId;";
            SqlCommand cmdTestResultGroup = new SqlCommand(queryTestResultGroup, conn, tran);
            cmdTestResultGroup.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdTestResultGroup.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsTestResultGroupAffected = cmdTestResultGroup.ExecuteNonQuery();

            string queryRow = @"DELETE CR
                                    FROM ConsultationRows CR
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CR.ClinicId = @ClinicId;";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);
            cmdRow.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdRow.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsRowAffected = cmdRow.ExecuteNonQuery();

            string querySection = @"DELETE FROM ConsultationSections 
                                        WHERE ConsultationId = @ConsultationId
                                        AND ClinicId = @ClinicId;";
            SqlCommand cmdSection = new SqlCommand(querySection, conn, tran);
            cmdSection.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdSection.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsSectionAffected = cmdSection.ExecuteNonQuery();

            string queryConsultation = @"DELETE FROM Consultations 
                                            WHERE ConsultationId = @ConsultationId
                                            AND ClinicId = @ClinicId;";
            SqlCommand cmdConsultation = new SqlCommand(queryConsultation, conn, tran);
            cmdConsultation.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdConsultation.Parameters.AddWithValue("@ClinicId", clinicId);
            int rowsConsultAffected = cmdConsultation.ExecuteNonQuery();

            return rowsConsultAffected > 0;
        }

        public bool DeleteRows(List<ConsultationRow> rows, SqlConnection conn, SqlTransaction tran)
        {
            int rowsOptionAffected = 0;
            int rowsFieldAffected = 0;
            int rowsVitalAffected = 0;
            int rowsTestResultAffected = 0;
            int rowsRowAffected = 0;

            string queryOption = @"DELETE CFO
                                    FROM ConsultationFieldOptions CFO
                                    INNER JOIN ConsultationFields CF
                                        ON CF.FieldId = CFO.FieldId
                                    WHERE CF.RowId = @RowId
                                    AND CFO.ClinicId = @ClinicId";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);

            string queryField = @"DELETE FROM ConsultationFields
                                    WHERE RowId = @RowId
                                    AND ClinicId = @ClinicId";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);

            string queryVital = @"DELETE FROM ConsultationVitals
                                    WHERE RowId = @RowId
                                    AND ClinicId = @ClinicId";
            SqlCommand cmdVital = new SqlCommand(queryVital, conn, tran);

            string queryTestResult = @"DELETE CT 
                                    FROM ConsultationTestResults CT
                                    INNER JOIN ConsultationTestResultGroups CG
                                        ON CT.ConsultationTestResultGroupId = CG.ConsultationTestResultGroupId
                                    WHERE CG.RowId = @RowId
                                    AND CT.ClinicId = @ClinicId";
            SqlCommand cmdTestResult = new SqlCommand(queryTestResult, conn, tran);

            string queryTestResultGroup = @"DELETE FROM ConsultationTestResultGroups
                                           WHERE RowId = @RowId
                                            AND ClinicId = @ClinicId";
            SqlCommand cmdTestResultGroup = new SqlCommand(queryTestResultGroup, conn, tran);

            string queryRow = @"DELETE FROM ConsultationRows 
                                    WHERE RowId = @RowId
                                    AND ClinicId = @ClinicId";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            foreach (var row in rows)
            {
                cmdOption.Parameters.Clear();
                cmdOption.Parameters.AddWithValue("@RowId", row.RowId);
                cmdOption.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                rowsOptionAffected += cmdOption.ExecuteNonQuery();

                cmdField.Parameters.Clear();
                cmdField.Parameters.AddWithValue("@RowId", row.RowId);
                cmdField.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                rowsFieldAffected += cmdField.ExecuteNonQuery();

                cmdVital.Parameters.Clear();
                cmdVital.Parameters.AddWithValue("@RowId", row.RowId);
                cmdVital.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                rowsVitalAffected += cmdVital.ExecuteNonQuery();

                cmdTestResult.Parameters.Clear();
                cmdTestResult.Parameters.AddWithValue("@RowId", row.RowId);
                cmdTestResult.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                rowsTestResultAffected += cmdTestResult.ExecuteNonQuery();

                cmdRow.Parameters.Clear();
                cmdRow.Parameters.AddWithValue("@RowId", row.RowId);
                cmdRow.Parameters.AddWithValue("@ClinicId", row.ClinicId);
                rowsRowAffected += cmdRow.ExecuteNonQuery();
            }

            return rowsRowAffected > 0;
        }

        public bool DeleteTemplateRows(int consultationId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsOptionAffected = 0;
            int rowsFieldAffected = 0;
            int rowsRowAffected = 0;

            string queryOption = @"DELETE CFO
                                    FROM ConsultationFieldOptions CFO
                                    INNER JOIN ConsultationFields CF
                                        ON CF.FieldId = CFO.FieldId
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CF.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CFO.ClinicId = @ClinicId 
                                    AND CR.IsTemplate = 1";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);

            cmdOption.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdOption.Parameters.AddWithValue("@ClinicId", clinicId);
            rowsOptionAffected = cmdOption.ExecuteNonQuery();

            string queryField = @"DELETE CF
                                    FROM ConsultationFields CF
                                    INNER JOIN ConsultationRows CR
                                        ON CR.RowId = CF.RowId
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CF.ClinicId = @ClinicId
                                    AND CR.IsTemplate = 1;";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);

            cmdField.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdField.Parameters.AddWithValue("@ClinicId", clinicId);
            rowsFieldAffected = cmdField.ExecuteNonQuery();

            string queryRow = @"DELETE CR
                                    FROM ConsultationRows CR
                                    INNER JOIN ConsultationSections CS
                                        ON CS.SectionId = CR.SectionId
                                    WHERE CS.ConsultationId = @ConsultationId
                                    AND CR.ClinicId = @ClinicId 
                                    AND CR.IsTemplate = 1";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            cmdRow.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdRow.Parameters.AddWithValue("@ClinicId", clinicId);
            rowsRowAffected += cmdRow.ExecuteNonQuery();

            return rowsRowAffected > 0;
        }

        public bool DeleteUnusedOptions(int consultationId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"DELETE CFO
                                FROM ConsultationFieldOptions CFO
                                INNER JOIN ConsultationFields CF
                                    ON CF.FieldId = CFO.FieldId
                                INNER JOIN ConsultationRows CR
                                    ON CR.RowId = CF.RowId
                                INNER JOIN ConsultationSections CS
                                    ON CS.SectionId = CR.SectionId
                                WHERE CS.ConsultationId = @ConsultationId
                                AND CFO.ClinicId = @ClinicId 
                                AND CFO.IsSelected = 0";
            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        public bool DeleteUnusedFields(int consultationId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            // delete all fields which neither have FieldValue nor any Options

            string query = @"DELETE CF
                                FROM ConsultationFields CF
                                INNER JOIN ConsultationRows CR
                                    ON CR.RowId = CF.RowId
                                INNER JOIN ConsultationSections CS
                                    ON CS.SectionId = CR.SectionId
                                WHERE CS.ConsultationId = @ConsultationId
                                AND CF.ClinicId = @ClinicId
                                AND (CF.FieldValue IS NULL OR LTRIM(RTRIM(CF.FieldValue)) = '')
                                AND NOT EXISTS
                                (
                                    SELECT 1
                                    FROM ConsultationFieldOptions CFO
                                    WHERE CFO.FieldId = CF.FieldId
                                );";
            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        public List<Consultation> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Consultation> consultations = new List<Consultation>();

            string query = @"SELECT ConsultationId, AppointmentId, TemplateId, TemplateName, Status 
                                FROM Consultations
                                WHERE ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    consultations.Add(new Consultation
                    {
                        ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        TemplateId = Convert.ToInt32(dr["TemplateId"]),
                        TemplateName = dr["TemplateName"].ToString(),
                        Status = (ConsultationStatus)Convert.ToInt32(dr["Status"])
                    });
                }
            }

            return consultations;
        }

        public Consultation Get(int clinicId, int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return GetConsultation(clinicId, consultationId, 0, conn, tran);
        }

        public Consultation GetByAppointment(int clinicId, int appointmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return GetConsultation(clinicId, 0, appointmentId, conn, tran);
        }

        public Consultation GetConsultation(int clinicId, int consultationId, int appointmentId,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            Consultation consultation = null;

            string query = @"
                SELECT c.ConsultationId, c.TemplateId, c.TemplateName, c.Status,
                    a.AppointmentId, a.AppointmentDate, a.SlotStartTime,
                    p.PatientId, p.FirstName + ' ' + p.LastName as PatientName,
                    p.DOB, p.Gender,
                    d.DoctorId, d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName 
                FROM Consultations c
                INNER JOIN Appointments a
                    ON c.AppointmentId = a.AppointmentId
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                WHERE c.ClinicId = @ClinicId";

            if (consultationId > 0)
                query += " AND c.ConsultationId = @ConsultationId;";
            else if (appointmentId > 0)
                query += " AND a.AppointmentId = @AppointmentId;";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            if (consultationId > 0)
                cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            else if (appointmentId > 0)
                cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                    consultation = new Consultation
                    {
                        ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        TemplateId = Convert.ToInt32(dr["TemplateId"]),
                        TemplateName = dr["TemplateName"].ToString(),
                        Status = (ConsultationStatus)Convert.ToInt32(dr["Status"]),
                        Gender = dr["Gender"].ToString(),
                        AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                        SlotStartTime = (TimeSpan)dr["SlotStartTime"],
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        PatientName = dr["PatientName"].ToString(),
                        DOB = Convert.ToDateTime(dr["DOB"]),
                        DoctorId = Convert.ToInt32(dr["DoctorId"]),
                        DoctorName = dr["DoctorName"].ToString()
                    };
                }
            }

            return consultation;
        }

        public List<ConsultationSection> ListSections(int consultationId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationSection> sections = new List<ConsultationSection>();

            string query = @"
                        SELECT SectionId, ConsultationId, SectionName, DisplayOrder, IsVisible,
                                Multiline, IsPharmacologyAdvice, IsVital, IsTestResult 
                        FROM ConsultationSections 
                        WHERE ConsultationId = @ConsultationId
                        AND ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            using (SqlDataReader drSection = cmd.ExecuteReader())
            {
                while (drSection.Read())
                {
                    ConsultationSection section = new ConsultationSection
                    {
                        SectionId = Convert.ToInt32(drSection["SectionId"]),
                        ConsultationId = Convert.ToInt32(drSection["ConsultationId"]),
                        SectionName = drSection["SectionName"].ToString(),
                        DisplayOrder = Convert.ToInt32(drSection["DisplayOrder"]),
                        IsVisible = Convert.ToBoolean(drSection["IsVisible"]),
                        Multiline = Convert.ToBoolean(drSection["Multiline"]),
                        IsPharmacologyAdvice = Convert.ToBoolean(drSection["IsPharmacologyAdvice"]),
                        IsVital = Convert.ToBoolean(drSection["IsVital"]),
                        IsTestResult = Convert.ToBoolean(drSection["IsTestResult"])
                    };

                    sections.Add(section);
                }
            }

            return sections;
        }

        public List<ConsultationRow> ListTemplateRows(int consultationId, int clinicId,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationRow> rows = new List<ConsultationRow>();

            string query = @"
                        SELECT CR.RowId, CR.SectionId, CR.RowNumber, CR.DisplayOrder
                        FROM ConsultationRows CR
                        INNER JOIN ConsultationSections CS
                            ON CS.SectionId = CR.SectionId
                        WHERE CS.ConsultationId = @ConsultationId
                        AND CR.ClinicId = @ClinicId
                        AND CR.IsTemplate = 1";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ConsultationRow row = new ConsultationRow
                    {
                        RowId = Convert.ToInt32(dr["RowId"]),
                        SectionId = Convert.ToInt32(dr["SectionId"]),
                        RowNumber = Convert.ToInt32(dr["RowNumber"]),
                        DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]),
                    };

                    rows.Add(row);
                }
            }

            return rows;
        }

        public List<ConsultationRow> ListRows(int consultationId, int clinicId,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationRow> rows = new List<ConsultationRow>();

            string query = @"
                        SELECT CR.RowId, CR.SectionId, CR.RowNumber, CR.DisplayOrder, CR.CreatedOn, CR.ModifiedOn
                            FROM ConsultationRows CR
                            INNER JOIN ConsultationSections CS
                                ON CS.SectionId = CR.SectionId
                            WHERE CS.ConsultationId = @ConsultationId
                            AND CR.ClinicId = @ClinicId
                            AND CR.IsTemplate = 0";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ConsultationRow row = new ConsultationRow
                    {
                        RowId = Convert.ToInt32(dr["RowId"]),
                        SectionId = Convert.ToInt32(dr["SectionId"]),
                        RowNumber = Convert.ToInt32(dr["RowNumber"]),
                        DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                        ModifiedOn = dr["ModifiedOn"] != DBNull.Value ? Convert.ToDateTime(dr["ModifiedOn"]) : (DateTime?)null
                    };

                    rows.Add(row);
                }
            }

            return rows;
        }

        public List<ConsultationField> ListFields(int consultationId, int clinicId,
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationField> fields = new List<ConsultationField>();

            string query = @"
                        SELECT CF.FieldId, CF.RowId, CF.FieldLabel, CF.FieldTypeCode, CF.FieldCode, CF.FieldSource, 
                                CF.FieldDataId, CF.DisplayOrder, CF.IsMandatory, CF.FieldValue
                        FROM ConsultationFields CF
                        INNER JOIN ConsultationRows CR
                            ON CR.RowId = CF.RowId
                        INNER JOIN ConsultationSections CS
                            ON CS.SectionId = CR.SectionId
                        WHERE CS.ConsultationId = @ConsultationId
                        AND CF.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (SqlDataReader drField = cmd.ExecuteReader())
            {
                while (drField.Read())
                {
                    ConsultationField field = new ConsultationField
                    {
                        FieldId = Convert.ToInt32(drField["FieldId"]),
                        RowId = Convert.ToInt32(drField["RowId"]),
                        FieldLabel = drField["FieldLabel"].ToString(),
                        FieldTypeCode = drField["FieldTypeCode"].ToString(),
                        FieldCode = drField["FieldCode"] != DBNull.Value ? drField["FieldCode"].ToString() : null,
                        FieldSource = drField["FieldSource"] != DBNull.Value ? drField["FieldSource"].ToString() : null,
                        FieldDataId = drField["FieldDataId"] != DBNull.Value ? Convert.ToInt32(drField["FieldDataId"]) : 0,
                        DisplayOrder = Convert.ToInt32(drField["DisplayOrder"]),
                        IsMandatory = Convert.ToBoolean(drField["IsMandatory"]),
                        FieldValue = drField["FieldValue"] != DBNull.Value ? drField["FieldValue"].ToString() : null
                    };

                    fields.Add(field);
                }
            }

            return fields;
        }

        public List<ConsultationFieldOption> ListOptions(int consultationId, int sectionId, int clinicId,  
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationFieldOption> options = new List<ConsultationFieldOption>();

            string query = @"
                        SELECT CFO.OptionId, CFO.FieldId, CFO.OptionText, CFO.DisplayOrder, CFO.IsSelected
                            FROM ConsultationFieldOptions CFO
                            INNER JOIN ConsultationFields CF
                                ON CF.FieldId = CFO.FieldId
                            INNER JOIN ConsultationRows CR
                                ON CR.RowId = CF.RowId
                            INNER JOIN ConsultationSections CS
                                ON CS.SectionId = CR.SectionId
                            WHERE CS.ConsultationId = @ConsultationId
                            AND CS.SectionId = @SectionId
                            AND CFO.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (SqlDataReader drOption = cmd.ExecuteReader())
            {
                while (drOption.Read())
                {
                    ConsultationFieldOption option = new ConsultationFieldOption
                    {
                        OptionId = Convert.ToInt32(drOption["OptionId"]),
                        FieldId = Convert.ToInt32(drOption["FieldId"]),
                        OptionText = drOption["OptionText"].ToString(),
                        DisplayOrder = Convert.ToInt32(drOption["DisplayOrder"]),
                        IsSelected = Convert.ToBoolean(drOption["IsSelected"])
                    };

                    options.Add(option);
                }
            }

            return options;
        }

        public List<ConsultationFieldOption> ListOptions(int consultationId, int clinicId,
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationFieldOption> options = new List<ConsultationFieldOption>();

            string query = @"
                        SELECT CFO.OptionId, CFO.FieldId, CFO.OptionText, CFO.IsSelected, CFO.DisplayOrder
                            FROM ConsultationFieldOptions CFO
                            INNER JOIN ConsultationFields CF
                                ON CF.FieldId = CFO.FieldId
                            INNER JOIN ConsultationRows CR
                                ON CR.RowId = CF.RowId
                            INNER JOIN ConsultationSections CS
                                ON CS.SectionId = CR.SectionId
                            WHERE CS.ConsultationId = @ConsultationId
                            AND CFO.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (SqlDataReader drOption = cmd.ExecuteReader())
            {
                while (drOption.Read())
                {
                    ConsultationFieldOption option = new ConsultationFieldOption
                    {
                        OptionId = Convert.ToInt32(drOption["OptionId"]),
                        FieldId = Convert.ToInt32(drOption["FieldId"]),
                        OptionText = drOption["OptionText"].ToString(),
                        IsSelected = Convert.ToBoolean(drOption["IsSelected"]),
                        DisplayOrder = Convert.ToInt32(drOption["DisplayOrder"])
                    };

                    options.Add(option);
                }
            }

            return options;
        }

        public List<Consultation> ListByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Consultation> consultations = new List<Consultation>();

            string sql = @"SELECT c.ConsultationId, c.AppointmentId, c.TemplateId, c.TemplateName, c.Status,
                                a.AppointmentDate, a.SlotStartTime,
                                p.PatientId,
                                p.FirstName + ' ' + p.LastName as PatientName,
                                p.DOB, p.Gender,
                                d.DoctorId,
                                d.Salutation + ' ' + d.FirstName + ' ' + d.LastName as DoctorName
                            FROM Consultations c
                            INNER JOIN Appointments a ON c.AppointmentId = a.AppointmentId
                            INNER JOIN Patients p ON a.PatientId = p.PatientId
                            INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
                            WHERE p.PatientId = @PatientId";

            using var cmd = new SqlCommand(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                consultations.Add(new Consultation
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    SlotStartTime = (TimeSpan)dr["SlotStartTime"],
                    PatientId = Convert.ToInt32(dr["PatientId"]),
                    PatientName = dr["PatientName"].ToString(),
                    DOB = Convert.ToDateTime(dr["DOB"]),
                    Gender = dr["Gender"].ToString(),
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    DoctorName = dr["DoctorName"].ToString(),
                    TemplateId = Convert.ToInt32(dr["TemplateId"]),
                    TemplateName = dr["TemplateName"].ToString(),
                    Status = (ConsultationStatus)Convert.ToInt32(dr["Status"])
                });
            }

            return consultations;
        }

        public PrescriptionVM GetPrescription(int appointmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionVM vm = new();

            // consultation + doctor + patient + clinic
            string sql = @"
            SELECT
                d.Salutation + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                d.Qualification,
                d.RegistrationNo,
                c.Name ClinicName,
                c.Address ClinicAddress,
                c.LogoPath as ClinicLogo,
                c.Phone as ClinicPhoneNo,
                p.PatientId,
                p.FirstName + ' ' + p.LastName AS PatientName,
                p.Phone AS PatientPhoneNo,
                DATEDIFF(YEAR, p.DOB, GETDATE())
                    - CASE
                            WHEN DATEADD(YEAR,
                                        DATEDIFF(YEAR, p.DOB, GETDATE()),
                                        p.DOB) > GETDATE()
                            THEN 1
                            ELSE 0
                        END AS Age,
                p.Gender,
                a.AppointmentDate,
                a.SlotStartTime,
                a.VisitId,
                v.VisitDate,
                con.ConsultationId
            FROM Appointments a
            JOIN Consultations con ON a.AppointmentId=con.AppointmentId
            JOIN Patients p ON a.PatientId=p.PatientId
            JOIN Doctors d ON a.DoctorId=d.DoctorId
            JOIN Clinics c ON a.ClinicId=c.ClinicId
            JOIN Visits v ON a.VisitId = v.VisitId
            WHERE a.AppointmentId=@AppointmentId";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    vm.DoctorName = rdr["DoctorName"].ToString();
                    vm.Qualification = rdr["Qualification"].ToString();
                    vm.RegistrationNumber = rdr["RegistrationNo"].ToString();

                    vm.ClinicName = rdr["ClinicName"].ToString();
                    vm.ClinicAddress = rdr["ClinicAddress"].ToString();
                    vm.ClinicLogo = rdr["ClinicLogo"].ToString();
                    vm.ClinicPhoneNo = rdr["ClinicPhoneNo"].ToString();

                    vm.PatientId = Convert.ToInt32(rdr["PatientId"]);
                    vm.PatientName = rdr["PatientName"].ToString();
                    vm.Age = Convert.ToInt32(rdr["Age"]);
                    vm.Gender = rdr["Gender"].ToString();
                    vm.PatientPhoneNo = rdr["PatientPhoneNo"].ToString();

                    vm.VisitDateTime = Convert.ToDateTime(rdr["AppointmentDate"]).Add((TimeSpan)rdr["SlotStartTime"]);

                    vm.ConsultationId = Convert.ToInt32(rdr["ConsultationId"]);
                    vm.VisitId = Convert.ToInt32(rdr["VisitId"]);
                    vm.VisitDate = Convert.ToDateTime(rdr["VisitDate"]);
                    vm.AppointmentId = Convert.ToInt32(appointmentId);
                }
            }

            return vm;
        }

        public List<PrescriptionSectionVM> ListPrescriptionSections(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionSectionVM> vms = new List<PrescriptionSectionVM> ();

            string sql = @"
                SELECT cs.SectionId, cs.ConsultationId, cs.SectionName, cs.DisplayOrder, cs.IsVisible,
                    cs.Multiline, cs.IsPharmacologyAdvice, cs.IsVital, cs.IsTestResult
                FROM ConsultationSections cs
                INNER JOIN ConsultationRows cr
                    ON cr.SectionId = cs.SectionId
                WHERE cs.ConsultationId = @ConsultationId
                AND (EXISTS 
                        (SELECT 1
                        FROM ConsultationFields cf
                        WHERE cr.RowId = cf.RowId
                        AND (cf.FieldValue IS NOT NULL
                                OR EXISTS
                                (
                                    SELECT 1
                                    FROM ConsultationFieldOptions cfo
                                    WHERE cfo.FieldId = cf.FieldId
                                        AND cfo.IsSelected = 1
                                )
                            )
                        )
                    OR EXISTS (SELECT 1
                               FROM ConsultationVitals cv
                               WHERE cr.RowId = cv.RowId)
                    OR EXISTS (SELECT 1
                                FROM ConsultationTestResultGroups cg
                                WHERE cr.RowId = cg.RowId)
                )
                ORDER BY cs.DisplayOrder;";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    PrescriptionSectionVM section = new PrescriptionSectionVM
                    {
                        SectionId = Convert.ToInt32(rdr["SectionId"]),
                        ConsultationId = Convert.ToInt32(rdr["ConsultationId"]),
                        SectionName = rdr["SectionName"].ToString(),
                        DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"]),
                        IsVisible = Convert.ToBoolean(rdr["IsVisible"]),
                        Multiline = Convert.ToBoolean(rdr["Multiline"]),
                        IsPharmacologyAdvice = Convert.ToBoolean(rdr["IsPharmacologyAdvice"]),
                        IsVital = Convert.ToBoolean(rdr["IsVital"]),
                        IsTestResult = Convert.ToBoolean(rdr["IsTestResult"])
                    };

                    vms.Add(section);
                }
            }

            return vms;
        }

        public List<PrescriptionRowVM> ListPrescriptionRows(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionRowVM> vms = new List<PrescriptionRowVM> ();

            string sql = @"
                SELECT
                    cr.RowId,
                    cr.SectionId,
                    cr.RowNumber,
                    cr.DisplayOrder
                FROM ConsultationRows cr
                INNER JOIN ConsultationSections cs
                    ON cs.SectionId = cr.SectionId
                WHERE cs.ConsultationId = @ConsultationId
                AND cr.IsTemplate = 0
                AND (EXISTS 
                        (SELECT 1
                        FROM ConsultationFields cf
                        WHERE cr.RowId = cf.RowId
                        AND (cf.FieldValue IS NOT NULL
                                OR EXISTS
                                (
                                    SELECT 1
                                    FROM ConsultationFieldOptions cfo
                                    WHERE cfo.FieldId = cf.FieldId
                                        AND cfo.IsSelected = 1
                                )
                            )
                        )
                    OR EXISTS (SELECT 1
                               FROM ConsultationVitals cv
                               WHERE cr.RowId = cv.RowId)
                    OR EXISTS (SELECT 1
                                FROM ConsultationTestResultGroups cg
                                WHERE cr.RowId = cg.RowId)
                )
                ORDER BY
                    cr.SectionId,
                    cr.DisplayOrder;";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    PrescriptionRowVM row = new PrescriptionRowVM
                    {
                        RowId = Convert.ToInt32(rdr["RowId"]),
                        SectionId = Convert.ToInt32(rdr["SectionId"]),
                        RowNumber = Convert.ToInt32(rdr["RowNumber"]),
                        DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"]),
                    };

                    vms.Add(row);
                }
            }

            return vms;
        }

        public List<PrescriptionFieldVM> ListPrescriptionFields(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionFieldVM> vms = new List<PrescriptionFieldVM> ();

            string sql = @"
                SELECT
                    cf.FieldId,
                    cf.RowId,
                    cf.FieldLabel,
                    cf.FieldValue,
                    cf.DisplayOrder
                FROM ConsultationFields cf
                INNER JOIN ConsultationRows cr
                    ON cr.RowId = cf.RowId
                INNER JOIN ConsultationSections cs
                    ON cs.SectionId = cr.SectionId
                WHERE cs.ConsultationId = @ConsultationId
                AND
                (
                    cf.FieldValue IS NOT NULL
                    OR EXISTS
                    (
                        SELECT 1
                        FROM ConsultationFieldOptions cfo
                        WHERE cfo.FieldId = cf.FieldId
                            AND cfo.IsSelected = 1
                    )
                )
                ORDER BY
                    cf.RowId,
                    cf.DisplayOrder;";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    PrescriptionFieldVM field = new PrescriptionFieldVM
                    {
                        FieldId = Convert.ToInt32(rdr["FieldId"]),
                        RowId = Convert.ToInt32(rdr["RowId"]),
                        FieldLabel = rdr["FieldLabel"].ToString(),
                        FieldValue = rdr["FieldValue"].ToString(),
                        DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"]),
                    };

                    vms.Add(field);
                }
            }

            return vms;
        }

        public List<PrescriptionVitalVM> ListPrescriptionVitals(int consultationId, int clinicId, 
                                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<PrescriptionVitalVM>();

            const string sql = @"
                SELECT v.VitalId, v.RowId, v.VisitDate, v.VitalLabel, v.VitalCode, v.VitalType, v.VitalValue, v.VitalUnit,
                        ISNULL(v.DisplayOrder,0) AS DisplayOrder
                FROM ConsultationVitals v
                INNER JOIN ConsultationRows r
                    ON r.RowId = v.RowId
                INNER JOIN ConsultationSections s
                    ON s.SectionId = r.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND v.ClinicId = @ClinicId
                ORDER BY
                    ISNULL(v.DisplayOrder,0),
                    v.VitalLabel;";

            using var cmd = new SqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new PrescriptionVitalVM
                    {
                        VitalId = Convert.ToInt32(rdr["VitalId"]),
                        RowId = Convert.ToInt32(rdr["RowId"]),
                        VisitDate = Convert.ToDateTime(rdr["VisitDate"]),
                        VitalLabel = Convert.ToString(rdr["VitalLabel"]) ?? "",
                        VitalCode = Convert.ToString(rdr["VitalCode"]) ?? "",
                        VitalType = Convert.ToString(rdr["VitalType"]) ?? "",
                        VitalValue = Convert.ToString(rdr["VitalValue"]) ?? "",
                        VitalUnit = Convert.ToString(rdr["VitalUnit"]) ?? "",
                        DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"])
                    });
            }

            return list;
        }

        public List<PrescriptionTestResultVM> ListPrescriptionTestResults(int consultationId, int clinicId,
                                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<PrescriptionTestResultVM>();

            const string sql = @"
                SELECT tr.TestResultId, tg.RowId, tg.TestDate, tr.TestPanelCode, tr.TestPanelName, tr.TestParameterCode,
                        tr.TestParameterName, tr.Value, tr.Unit,  tr.ResultFlag, tr.PanelDisplayOrder, tr.ParameterDisplayOrder
                FROM ConsultationTestResults tr
                INNER JOIN ConsultationTestResultGroups tg
                    ON tr.ConsultationTestResultGroupId = tg.ConsultationTestResultGroupId
                INNER JOIN ConsultationRows r
                    ON r.RowId = tg.RowId
                INNER JOIN ConsultationSections s
                    ON s.SectionId = r.SectionId
                WHERE s.ConsultationId = @ConsultationId
                AND tr.ClinicId = @ClinicId
                ORDER BY
                    tg.TestDate DESC,
                    tr.PanelDisplayOrder,
                    tr.ParameterDisplayOrder,
                    tr.TestPanelName,
                    tr.TestParameterName;";

            using var cmd = new SqlCommand(sql, conn, tran);

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                list.Add(new PrescriptionTestResultVM
                    {
                        TestResultId = Convert.ToInt32(rdr["TestResultId"]),
                        RowId = Convert.ToInt32(rdr["RowId"]),
                        TestDate = Convert.ToDateTime(rdr["TestDate"]),
                        TestPanelCode = Convert.ToString(rdr["TestPanelCode"]) ?? "",
                        TestPanelName = Convert.ToString(rdr["TestPanelName"]) ?? "",
                        TestParameterCode = Convert.ToString(rdr["TestParameterCode"]) ?? "",
                        TestParameterName = Convert.ToString( rdr["TestParameterName"]) ?? "",
                        Value = Convert.ToString(rdr["Value"]) ?? "",
                        Unit = Convert.ToString(rdr["Unit"]) ?? "",
                        ResultFlag = Convert.ToString(rdr["ResultFlag"]) ?? "",
                        PanelDisplayOrder = Convert.ToInt32(rdr["PanelDisplayOrder"]),
                        ParameterDisplayOrder = Convert.ToInt32(rdr["ParameterDisplayOrder"])
                    });
            }

            return list;
        }

        public List<PrescriptionOptionVM> ListPrescriptionOptions(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionOptionVM> vms = new List<PrescriptionOptionVM> ();

            string sql = @"
                SELECT
                    cfo.OptionId,
                    cfo.FieldId,
                    cfo.OptionText,
                    cfo.DisplayOrder
                FROM ConsultationFieldOptions cfo
                INNER JOIN ConsultationFields cf
                    ON cf.FieldId = cfo.FieldId
                INNER JOIN ConsultationRows cr
                    ON cr.RowId = cf.RowId
                INNER JOIN ConsultationSections cs
                    ON cs.SectionId = cr.SectionId
                WHERE cs.ConsultationId = @ConsultationId
                AND cfo.IsSelected = 1
                ORDER BY
                    cfo.FieldId,
                    cfo.DisplayOrder;";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    PrescriptionOptionVM option = new PrescriptionOptionVM
                    {
                        OptionId = Convert.ToInt32(rdr["OptionId"]),
                        FieldId = Convert.ToInt32(rdr["FieldId"]),
                        OptionText = rdr["OptionText"].ToString(),
                        DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"])
                    };

                    vms.Add(option);
                }
            }

            return vms;
        }

        public List<PrescriptionVM> ListPrescriptionsByPatient(int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var prescriptions = new List<PrescriptionVM>();

            // consultation + doctor + patient + clinic
            string sql = @"
            SELECT
                d.Salutation + ' ' + d.FirstName + ' ' + d.LastName AS DoctorName,
                d.Qualification,
                d.RegistrationNo,
                c.Name ClinicName,
                c.Address ClinicAddress,
                c.LogoPath ClinicLogo,
                p.PatientId,
                p.FirstName + ' ' + p.LastName AS PatientName,
                p.Phone AS PatientPhoneNo,
                DATEDIFF(YEAR, p.DOB, GETDATE())
                    - CASE
                            WHEN DATEADD(YEAR,
                                        DATEDIFF(YEAR, p.DOB, GETDATE()),
                                        p.DOB) > GETDATE()
                            THEN 1
                            ELSE 0
                        END AS Age,
                p.Gender,
                a.AppointmentDate,
                a.SlotStartTime,
                con.ConsultationId,
                a.AppointmentId
            FROM Appointments a
            JOIN Consultations con ON a.AppointmentId=con.AppointmentId
            JOIN Patients p ON a.PatientId=p.PatientId
            JOIN Doctors d ON a.DoctorId=d.DoctorId
            JOIN Clinics c ON a.ClinicId=c.ClinicId
            WHERE a.AppointmentId=@PatientId";

            SqlCommand cmd = new(sql, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var vm = new PrescriptionVM();

                    vm.DoctorName = rdr["DoctorName"].ToString();
                    vm.Qualification = rdr["Qualification"].ToString();
                    vm.RegistrationNumber = rdr["RegistrationNo"].ToString();

                    vm.ClinicName = rdr["ClinicName"].ToString();
                    vm.ClinicAddress = rdr["ClinicAddress"].ToString();
                    vm.ClinicLogo = rdr["ClinicLogo"].ToString();

                    vm.PatientId = Convert.ToInt32(rdr["PatientId"]);
                    vm.PatientName = rdr["PatientName"].ToString();
                    vm.Age = Convert.ToInt32(rdr["Age"]);
                    vm.Gender = rdr["Gender"].ToString();
                    vm.PatientPhoneNo = rdr["PatientPhoneNo"].ToString();
                    vm.VisitDateTime = Convert.ToDateTime(rdr["AppointmentDate"]).Add((TimeSpan)rdr["SlotStartTime"]);
                    vm.AppointmentId = Convert.ToInt32(rdr["AppointmentId"]);                    
                    vm.ConsultationId = Convert.ToInt32(rdr["ConsultationId"]);

                    prescriptions.Add(vm);
                }
            }

            return prescriptions;
        }
    }

}

