using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace DAL
{
    public class ConsultationDAL : IDAL<Consultation>
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
                (AppointmentId, TemplateId, TemplateName, Status, CreatedBy)
                VALUES
                (@AppointmentId, @TemplateId, @TemplateName, @Status, @CreatedBy);
                                    
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdConsultation = new SqlCommand(queryConsultation, conn, tran);
            cmdConsultation.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);
            cmdConsultation.Parameters.AddWithValue("@TemplateId", model.TemplateId);
            cmdConsultation.Parameters.AddWithValue("@TemplateName", model.TemplateName);
            cmdConsultation.Parameters.AddWithValue("@Status", model.Status);
            cmdConsultation.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);

            consultationId = Convert.ToInt32(cmdConsultation.ExecuteScalar());

            return consultationId;
        }

        public int AddSections(int consultationId, List<ConsultationSection> sections, SqlConnection conn, SqlTransaction tran)
        {
            int sectionId = 0;

            string querySection = @"
                INSERT INTO ConsultationSections
                (ConsultationId, TemplateId, TemplateSectionId, SectionName, DisplayOrder, IsVisible, Multiline, IsPharmacologyAdvice)
                VALUES
                (@ConsultationId, @TemplateId, @TemplateSectionId, @SectionName, @DisplayOrder, @IsVisible, @Multiline, @IsPharmacologyAdvice);

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdSection = new SqlCommand(querySection, conn, tran);

            foreach (var section in sections) 
            {
                cmdSection.Parameters.Clear();
                cmdSection.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmdSection.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                cmdSection.Parameters.AddWithValue("@TemplateSectionId", section.TemplateSectionId);
                cmdSection.Parameters.AddWithValue("@SectionName", section.SectionName);
                cmdSection.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                cmdSection.Parameters.AddWithValue("@IsVisible", section.IsVisible);
                cmdSection.Parameters.AddWithValue("@Multiline", section.Multiline);
                cmdSection.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice);

                sectionId = Convert.ToInt32(cmdSection.ExecuteScalar());
                section.SectionId = sectionId;
            }

            return sectionId;
        }

        public int AddPharmacologyAdvices(int consultationId, List<PharmacologyAdvice> rxs, SqlConnection conn, SqlTransaction tran)
        {
            int pharmacologyId = 0;

            string queryRX = @"
                INSERT INTO PharmacologyAdvices
                (ConsultationId, LineNumber, MedicineId, MedicineName, Dosage, Frequency, Duration, Instructions)
                VALUES
                (@ConsultationId, @LineNumber, @MedicineId, @MedicineName, @Dosage, @Frequency, @Duration, @Instructions);

                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdRX = new SqlCommand(queryRX, conn, tran);

            foreach (var rx in rxs)
            {
                cmdRX.Parameters.Clear();
                cmdRX.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmdRX.Parameters.AddWithValue("@LineNumber", rx.LineNumber);
                cmdRX.Parameters.AddWithValue("@MedicineId", rx.MedicineId);
                cmdRX.Parameters.AddWithValue("@MedicineName", rx.MedicineName);
                cmdRX.Parameters.AddWithValue("@Dosage", rx.Dosage);
                cmdRX.Parameters.AddWithValue("@Frequency", rx.Frequency);
                cmdRX.Parameters.AddWithValue("@Duration", rx.Duration);
                cmdRX.Parameters.AddWithValue("@Instructions", rx.Instructions);

                pharmacologyId = Convert.ToInt32(cmdRX.ExecuteScalar());
            }

            return pharmacologyId;
        }

        public int AddTemplateRow(int consultationId, int sectionId, int clinicId,
                                ConsultationRow row, SqlConnection conn, SqlTransaction tran)
        {
            int rowId = 0;

            string queryRow = @"
                INSERT INTO ConsultationRows
                (ConsultationId, SectionId, ClinicId, RowNumber, DisplayOrder, IsTemplate, CreatedOn)
                VALUES
                (@ConsultationId, @SectionId, @ClinicId, @RowNumber, @DisplayOrder, 1, GETDATE());
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            cmdRow.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmdRow.Parameters.AddWithValue("@SectionId", sectionId);
            cmdRow.Parameters.AddWithValue("@ClinicId", clinicId);
            cmdRow.Parameters.AddWithValue("@RowNumber", row.RowNumber);
            cmdRow.Parameters.AddWithValue("@DisplayOrder", row.DisplayOrder);

            rowId = Convert.ToInt32(cmdRow.ExecuteScalar());
            row.RowId = rowId;

            return rowId;
        }

        public int AddRows(int consultationId, int sectionId, int clinicId,
                                List<ConsultationRow> rows, SqlConnection conn, SqlTransaction tran)
        {
            int rowId = 0;

            string queryRow = @"
                INSERT INTO ConsultationRows
                (ConsultationId, SectionId, ClinicId, RowNumber, DisplayOrder, IsTemplate, CreatedOn)
                VALUES
                (@ConsultationId, @SectionId, @ClinicId, @RowNumber, @DisplayOrder, 0, GETDATE());
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);
            foreach (var row in rows) 
            {
                cmdRow.Parameters.Clear();
                cmdRow.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmdRow.Parameters.AddWithValue("@SectionId", sectionId);
                cmdRow.Parameters.AddWithValue("@ClinicId", clinicId);
                cmdRow.Parameters.AddWithValue("@RowNumber", row.RowNumber);
                cmdRow.Parameters.AddWithValue("@DisplayOrder", row.DisplayOrder);

                rowId = Convert.ToInt32(cmdRow.ExecuteScalar());
                row.RowId = rowId;
            }

            return rowId;
        }

        public int AddFields(int consultationId, int sectionId, int rowId, int clinicId,
                                List<ConsultationField> fields, SqlConnection conn, SqlTransaction tran)
        {
            int fieldId = 0;

            string queryField = @"
                INSERT INTO ConsultationFields
                (ConsultationId, TemplateId, SectionId, RowId, ClinicId, TemplateFieldId, FieldLabel, FieldTypeCode, DisplayOrder, IsMandatory, FieldValue, Unit)
                VALUES
                (@ConsultationId, @TemplateId, @SectionId, @RowId, @ClinicId, @TemplateFieldId, @FieldLabel, @FieldTypeCode, @DisplayOrder, @IsMandatory, @FieldValue, @Unit);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);
            foreach (var field in fields) 
            {
                cmdField.Parameters.Clear();
                cmdField.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmdField.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                cmdField.Parameters.AddWithValue("@SectionId", sectionId);
                cmdField.Parameters.AddWithValue("@RowId", rowId);
                cmdField.Parameters.AddWithValue("@ClinicId", clinicId);
                cmdField.Parameters.AddWithValue("@TemplateFieldId", field.TemplateFieldId);
                cmdField.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                cmdField.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                cmdField.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                cmdField.Parameters.AddWithValue("@IsMandatory", field.IsMandatory);
                cmdField.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? (object)DBNull.Value);
                cmdField.Parameters.AddWithValue("@Unit", field.Unit ?? (object)DBNull.Value);

                fieldId = Convert.ToInt32(cmdField.ExecuteScalar());
                field.FieldId = fieldId;
            }

            return fieldId;
        }

        public int AddOptions(int consultationId, int sectionId, int rowId, int fieldId, 
                                List<ConsultationFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            int optionId = 0;

            string queryOption = @"
                INSERT INTO ConsultationFieldOptions
                (ConsultationId, TemplateId, SectionId, RowId, FieldId, TemplateOptionId, OptionText, IsSelected, DisplayOrder)
                VALUES
                (@ConsultationId, @TemplateId, @SectionId, @RowId, @FieldId, @TemplateOptionId, @OptionText, @IsSelected, @DisplayOrder);
                
                SELECT SCOPE_IDENTITY();";

            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);
            foreach (var option in options)
            {
                cmdOption.Parameters.Clear();
                cmdOption.Parameters.AddWithValue("@ConsultationId", consultationId);
                cmdOption.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                cmdOption.Parameters.AddWithValue("@SectionId", sectionId);
                cmdOption.Parameters.AddWithValue("@RowId", rowId);
                cmdOption.Parameters.AddWithValue("@FieldId", fieldId);
                cmdOption.Parameters.AddWithValue("@TemplateOptionId", option.TemplateOptionId);
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
            cmd.Parameters.AddWithValue("@Status", model.Status);

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
                WHERE ConsultationId = @ConsultationId
                AND SectionId =  @SectionId
                AND RowId = @RowId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            foreach (var row in rows)
            {
                cmd.Parameters.AddWithValue("@ConsultationId", row.ConsultationId);
                cmd.Parameters.AddWithValue("@SectionId", row.SectionId);
                cmd.Parameters.AddWithValue("@RowId", row.RowId);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool UpdateFields(List<ConsultationField> fields, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE ConsultationFields
                SET FieldValue = @FieldValue,
                    Unit = @Unit
                WHERE ConsultationId = @ConsultationId
                AND SectionId = @SectionId
                AND FieldId = @FieldId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            int rowsAffected = 0;

            foreach (var field in fields)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConsultationId", field.ConsultationId);
                cmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                cmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                cmd.Parameters.AddWithValue("@FieldValue", field.FieldValue ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Unit", field.Unit ?? (object)DBNull.Value);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool UpdatePharmacologyAdvices(List<PharmacologyAdvice> rxs, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE PharmacologyAdvices
                SET MedicineId = @MedicineId,
                    MedicineName = @MedicineName,
                    Dosage = @Dosage,
                    Frequency = @Frequency,
                    Duration = @Duration,
                    Instructions = @Instructions
                WHERE ConsultationId = @ConsultationId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            int rowsAffected = 0;

            foreach (var rx in rxs)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ConsultationId", rx.ConsultationId);
                cmd.Parameters.AddWithValue("@MedicineId", rx.MedicineId);
                cmd.Parameters.AddWithValue("@MedicineName", rx.MedicineName);
                cmd.Parameters.AddWithValue("@Dosage", rx.Dosage);
                cmd.Parameters.AddWithValue("@Frequency", rx.Frequency);
                cmd.Parameters.AddWithValue("@Duration", rx.Duration);
                cmd.Parameters.AddWithValue("@Instructions", rx.Instructions);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool UpdateOptions(List<ConsultationFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE ConsultationFieldOptions
                SET IsSelected = @IsSelected
                WHERE ConsultationId = @ConsultationId
                AND SectionId = @SectionId
                AND FieldId = @FieldId
                AND OptionId = @OptionId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            int rowsAffected = 0;

            foreach (var option in options)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@IsSelected", option.IsSelected);
                cmd.Parameters.AddWithValue("@ConsultationId", option.ConsultationId);
                cmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                cmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                cmd.Parameters.AddWithValue("@OptionId", option.OptionId);

                rowsAffected += cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string queryRX = @"DELETE FROM PharmacologyAdvices WHERE ConsultationId = @id";
            SqlCommand cmdRX = new SqlCommand(queryRX, conn, tran);
            cmdRX.Parameters.AddWithValue("@id", id);
            int rowsRXAffected = cmdRX.ExecuteNonQuery();

            string queryOption = @"DELETE FROM ConsultationFieldOptions WHERE ConsultationId = @id";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);
            cmdOption.Parameters.AddWithValue("@id", id);
            int rowsOptionAffected = cmdOption.ExecuteNonQuery();

            string queryField = @"DELETE FROM ConsultationFields WHERE ConsultationId = @id";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);
            cmdField.Parameters.AddWithValue("@id", id);
            int rowsFieldAffected = cmdField.ExecuteNonQuery();

            string queryRow = @"DELETE FROM ConsultationFields WHERE ConsultationId = @id";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);
            cmdRow.Parameters.AddWithValue("@id", id);
            int rowsRowAffected = cmdRow.ExecuteNonQuery();

            string querySection = @"DELETE FROM ConsultationSections WHERE ConsultationId = @id";
            SqlCommand cmdSection = new SqlCommand(querySection, conn, tran);
            cmdSection.Parameters.AddWithValue("@id", id);
            int rowsSectionAffected = cmdSection.ExecuteNonQuery();

            string queryConsultation = @"DELETE FROM Consultations WHERE ConsultationId = @id";
            SqlCommand cmdConsultation = new SqlCommand(queryConsultation, conn, tran);
            cmdConsultation.Parameters.AddWithValue("@id", id);
            int rowsConsultAffected = cmdConsultation.ExecuteNonQuery();

            return rowsConsultAffected > 0;
        }

        public bool DeleteRows(List<ConsultationRow> rows, SqlConnection conn, SqlTransaction tran)
        {
            int rowsOptionAffected = 0;
            int rowsFieldAffected = 0;
            int rowsRowAffected = 0;

            string queryOption = @"DELETE FROM ConsultationFieldOptions 
                                    WHERE ConsultationId = @ConsultationId 
                                    AND SectionId = @SectionId
                                    AND FieldId = @FieldId";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);

            string queryField = @"DELETE FROM ConsultationFields 
                                    WHERE ConsultationId = @ConsultationId 
                                    AND SectionId = @SectionId
                                    AND FieldId = @FieldId";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);

            string queryRow = @"DELETE FROM ConsultationRows 
                                    WHERE ConsultationId = @ConsultationId 
                                    AND SectionId = @SectionId
                                    AND RowId = @RowId";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            foreach (var row in rows)
            {
                foreach (var field in row.Fields)
                {
                    cmdOption.Parameters.Clear();
                    cmdOption.Parameters.AddWithValue("@ConsultationId", field.ConsultationId);
                    cmdOption.Parameters.AddWithValue("@SectionId", field.SectionId);
                    cmdOption.Parameters.AddWithValue("@FieldId", field.FieldId);
                    rowsOptionAffected += cmdOption.ExecuteNonQuery();

                    cmdField.Parameters.Clear();
                    cmdField.Parameters.AddWithValue("@ConsultationId", field.ConsultationId);
                    cmdField.Parameters.AddWithValue("@SectionId", field.SectionId);
                    cmdField.Parameters.AddWithValue("@FieldId", field.FieldId);
                    rowsFieldAffected += cmdField.ExecuteNonQuery();
                }

                cmdRow.Parameters.Clear();
                cmdRow.Parameters.AddWithValue("@ConsultationId", row.ConsultationId);
                cmdRow.Parameters.AddWithValue("@SectionId", row.SectionId);
                cmdRow.Parameters.AddWithValue("@RowId", row.RowId);
                rowsRowAffected += cmdField.ExecuteNonQuery();
            }

            return rowsRowAffected > 0;
        }

        public bool DeleteTemplateRows(int consultationId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsOptionAffected = 0;
            int rowsFieldAffected = 0;
            int rowsRowAffected = 0;

            string queryOption = @"DELETE co
                                    FROM ConsultationFieldOptions co
                                    INNER JOIN ConsultationRows cr
                                        ON co.RowId = cr.RowId 
                                    WHERE co.ConsultationId = @ConsultationId 
                                    AND cr.IsTemplate = 1";
            SqlCommand cmdOption = new SqlCommand(queryOption, conn, tran);

            cmdOption.Parameters.AddWithValue("@ConsultationId", consultationId);
            rowsOptionAffected = cmdOption.ExecuteNonQuery();

            string queryField = @"DELETE cf
                                    FROM ConsultationFields cf
                                    INNER JOIN ConsultationRows cr
                                        ON cf.RowId = cr.RowId
                                    WHERE cf.ConsultationId = @ConsultationId
                                    AND cr.IsTemplate = 1;";
            SqlCommand cmdField = new SqlCommand(queryField, conn, tran);

            cmdField.Parameters.AddWithValue("@ConsultationId", consultationId);
            rowsFieldAffected = cmdField.ExecuteNonQuery();

            string queryRow = @"DELETE FROM ConsultationRows 
                                    WHERE ConsultationId = @ConsultationId 
                                    AND IsTemplate = 1";
            SqlCommand cmdRow = new SqlCommand(queryRow, conn, tran);

            cmdRow.Parameters.AddWithValue("@ConsultationId", consultationId);
            rowsRowAffected += cmdRow.ExecuteNonQuery();

            return rowsRowAffected > 0;
        }

        public List<Consultation> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Consultation> consultations = new List<Consultation>();

            string query = @"SELECT ConsultationId, AppointmentId, TemplateId, TemplateName, Status FROM Consultations";

            SqlCommand cmd = new SqlCommand(query, conn);
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
                        Status = dr["Status"].ToString()
                    });
                }
            }

            return consultations;
        }

        public Consultation Get(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return GetConsultation(consultationId, 0, conn, tran);
        }

        public Consultation GetByAppointment(int appointmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return GetConsultation(0, appointmentId, conn, tran);
        }

        public Consultation GetConsultation(int consultationId, int appointmentId,
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
                WHERE (c.ConsultationId = @ConsultationId OR @ConsultationId = 0)
                AND (a.AppointmentId = @AppointmentId OR @AppointmentId = 0)";
                //AND (p.PatientId = @PatientId OR @PatientId = 0)";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
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
                        Status = dr["Status"].ToString(),
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

        public List<ConsultationSection> ListSections(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationSection> sections = new List<ConsultationSection>();

            string query = @"
                        SELECT SectionId, ConsultationId, TemplateId, TemplateSectionId, SectionName, DisplayOrder, IsVisible,
                                Multiline, IsPharmacologyAdvice 
                        FROM ConsultationSections 
                        WHERE ConsultationId = @ConsultationId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            using (SqlDataReader drSection = cmd.ExecuteReader())
            {
                while (drSection.Read())
                {
                    ConsultationSection section = new ConsultationSection
                    {
                        SectionId = Convert.ToInt32(drSection["SectionId"]),
                        ConsultationId = Convert.ToInt32(drSection["ConsultationId"]),
                        TemplateId = Convert.ToInt32(drSection["TemplateId"]),
                        TemplateSectionId = Convert.ToInt32(drSection["TemplateSectionId"]),
                        SectionName = drSection["SectionName"].ToString(),
                        DisplayOrder = Convert.ToInt32(drSection["DisplayOrder"]),
                        IsVisible = Convert.ToBoolean(drSection["IsVisible"]),
                        Multiline = Convert.ToBoolean(drSection["Multiline"]),
                        IsPharmacologyAdvice = Convert.ToBoolean(drSection["IsPharmacologyAdvice"])
                    };

                    sections.Add(section);
                }
            }

            return sections;
        }

        public List<ConsultationRow> ListTemplateRows(int consultationId, 
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationRow> rows = new List<ConsultationRow>();

            string query = @"
                        SELECT RowId, ConsultationId, ClinicId, SectionId, RowNumber, DisplayOrder
                        FROM ConsultationRows 
                        WHERE ConsultationId = @ConsultationId
                        AND IsTemplate = 1";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            // cmd.Parameters.AddWithValue("@SectionId", sectionId);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ConsultationRow row = new ConsultationRow
                    {
                        RowId = Convert.ToInt32(dr["RowId"]),
                        ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
                        SectionId = Convert.ToInt32(dr["SectionId"]),
                        RowNumber = Convert.ToInt32(dr["RowNumber"]),
                        DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]),
                    };

                    rows.Add(row);
                }
            }

            return rows;
        }

        public List<ConsultationRow> ListRows(int consultationId, 
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationRow> rows = new List<ConsultationRow>();

            string query = @"
                        SELECT RowId, ConsultationId, ClinicId, SectionId, RowNumber, DisplayOrder, 
                                CreatedOn, ModifiedOn
                        FROM ConsultationRows 
                        WHERE ConsultationId = @ConsultationId
                        AND IsTemplate = 0";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            // cmd.Parameters.AddWithValue("@SectionId", sectionId);
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ConsultationRow row = new ConsultationRow
                    {
                        RowId = Convert.ToInt32(dr["RowId"]),
                        ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                        ClinicId = Convert.ToInt32(dr["ClinicId"]),
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

        public List<ConsultationField> ListFields(int consultationId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationField> fields = new List<ConsultationField>();

            string query = @"
                        SELECT FieldId, ConsultationId, TemplateId, SectionId, RowId, TemplateFieldId, 
                                FieldLabel, FieldTypeCode, DisplayOrder, IsMandatory, FieldValue, Unit
                        FROM ConsultationFields 
                        WHERE ConsultationId = @ConsultationId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            // cmd.Parameters.AddWithValue("@SectionId", sectionId);

            using (SqlDataReader drField = cmd.ExecuteReader())
            {
                while (drField.Read())
                {
                    ConsultationField field = new ConsultationField
                    {
                        FieldId = Convert.ToInt32(drField["FieldId"]),
                        ConsultationId = Convert.ToInt32(drField["ConsultationId"]),
                        TemplateId = Convert.ToInt32(drField["TemplateId"]),
                        SectionId = Convert.ToInt32(drField["SectionId"]),
                        RowId = Convert.ToInt32(drField["RowId"]),
                        TemplateFieldId = Convert.ToInt32(drField["TemplateFieldId"]),
                        FieldLabel = drField["FieldLabel"].ToString(),
                        FieldTypeCode = drField["FieldTypeCode"].ToString(),
                        DisplayOrder = Convert.ToInt32(drField["DisplayOrder"]),
                        IsMandatory = Convert.ToBoolean(drField["IsMandatory"]),
                        FieldValue = drField["FieldValue"] != DBNull.Value ? drField["FieldValue"].ToString() : null,
                        Unit = drField["Unit"] != DBNull.Value ? drField["Unit"].ToString() : null
                    };

                    fields.Add(field);
                }
            }

            return fields;
        }

        public List<ConsultationFieldOption> ListOptions(int consultationId, int sectionId, 
                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationFieldOption> options = new List<ConsultationFieldOption>();

            string query = @"
                        SELECT OptionId, ConsultationId, TemplateId, SectionId, RowId, FieldId, TemplateOptionId, 
                                OptionText, DisplayOrder, IsSelected
                        FROM ConsultationFieldOptions 
                        WHERE ConsultationId = @ConsultationId 
                        AND SectionId = @SectionId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);

            using (SqlDataReader drOption = cmd.ExecuteReader())
            {
                while (drOption.Read())
                {
                    ConsultationFieldOption option = new ConsultationFieldOption
                    {
                        OptionId = Convert.ToInt32(drOption["OptionId"]),
                        ConsultationId = Convert.ToInt32(drOption["ConsultationId"]),
                        TemplateId = Convert.ToInt32(drOption["TemplateId"]),
                        SectionId = Convert.ToInt32(drOption["SectionId"]),
                        RowId = Convert.ToInt32(drOption["RowId"]),
                        FieldId = Convert.ToInt32(drOption["FieldId"]),
                        TemplateOptionId = Convert.ToInt32(drOption["TemplateOptionId"]),
                        DisplayOrder = Convert.ToInt32(drOption["DisplayOrder"]),
                        IsSelected = Convert.ToBoolean(drOption["IsSelected"]),
                        OptionText = drOption["OptionText"].ToString()
                    };

                    options.Add(option);
                }
            }

            return options;
        }

        public List<PharmacologyAdvice> ListPharmacologyAdvices(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PharmacologyAdvice> rxs = new List<PharmacologyAdvice>();

            string query = @"
                    SELECT PharmacologyId, SectionId, LineNumber, MedicineId, MedicineName, Dosage, 
                    Frequency, Duration, Instructions
                    FROM PharmacologyAdvices 
                    WHERE ConsultationId = @ConsultationId"; 
                   // AND SectionId = @SectionId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }
            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            //cmd.Parameters.AddWithValue("@SectionId", section.SectionId);

            using (SqlDataReader drRX = cmd.ExecuteReader())
            {
                while (drRX.Read())
                {
                    PharmacologyAdvice rx = new PharmacologyAdvice
                    {
                        PharmacologyId = Convert.ToInt32(drRX["PharmacologyId"]),
                        SectionId = Convert.ToInt32(drRX["SectionId"]),
                        LineNumber = Convert.ToInt32(drRX["LineNumber"]),
                        MedicineId = Convert.ToInt32(drRX["MedicineId"]),
                        MedicineName = drRX["MedicineName"].ToString(),
                        Dosage = drRX["Dosage"].ToString(),
                        Frequency = drRX["Frequency"].ToString(),
                        Duration = drRX["Duration"].ToString(),
                        Instructions = drRX["Instructions"].ToString()
                    };

                    rxs.Add(rx);
                }
            }

            return rxs;
        }

        public List<ConsultationFieldOption> ListOptions(int consultationId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<ConsultationFieldOption> options = new List<ConsultationFieldOption>();

            string query = @"
                        SELECT OptionId, ConsultationId, TemplateId, SectionId, FieldId, TemplateOptionId, 
                                OptionText, IsSelected, DisplayOrder
                        FROM ConsultationFieldOptions 
                        WHERE ConsultationId = @ConsultationId"; 
                        //AND SectionId = @SectionId 
                        //AND FieldId = @FieldId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ConsultationId", consultationId);
            //cmd.Parameters.AddWithValue("@SectionId", section.SectionId);
            //cmd.Parameters.AddWithValue("@FieldId", field.FieldId);

            using (SqlDataReader drOption = cmd.ExecuteReader())
            {
                while (drOption.Read())
                {
                    ConsultationFieldOption option = new ConsultationFieldOption
                    {
                        OptionId = Convert.ToInt32(drOption["OptionId"]),
                        ConsultationId = Convert.ToInt32(drOption["ConsultationId"]),
                        TemplateId = Convert.ToInt32(drOption["TemplateId"]),
                        SectionId = Convert.ToInt32(drOption["SectionId"]),
                        FieldId = Convert.ToInt32(drOption["FieldId"]),
                        TemplateOptionId = Convert.ToInt32(drOption["TemplateOptionId"]),
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
                    Status = dr["Status"].ToString()
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
                con.Symptoms,
                con.Diagnosis,
                con.Notes
            FROM Appointments a
            JOIN Consultations con ON a.AppointmentId=con.AppointmentId
            JOIN Patients p ON a.PatientId=p.PatientId
            JOIN Doctors d ON a.DoctorId=d.DoctorId
            JOIN Clinics c ON a.ClinicId=c.ClinicId
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

                    vm.PatientId = Convert.ToInt32(rdr["PatientId"]);
                    vm.PatientName = rdr["PatientName"].ToString();
                    vm.Age = Convert.ToInt32(rdr["Age"]);
                    vm.Gender = rdr["Gender"].ToString();
                    vm.PatientPhoneNo = rdr["PatientPhoneNo"].ToString();

                    vm.VisitDateTime = Convert.ToDateTime(rdr["AppointmentDate"]).Add((TimeSpan)rdr["SlotStartTime"]);

                    vm.Symptoms = rdr["Symptoms"].ToString();
                    vm.Diagnosis = rdr["Diagnosis"].ToString();
                    vm.Notes = rdr["Notes"].ToString();
                    vm.AppointmentId = Convert.ToInt32(appointmentId);
                }
            }
            vm.Medicines = new List<PharmacologyAdvice>();

            string medSql = @"
                SELECT MedicineName,
                        Dosage,
                        Frequency,
                        Duration,
                        Instructions
                FROM PharmacologyAdvices
                WHERE AppointmentId=@AppointmentId
                ORDER BY PharmacologyId";

            SqlCommand medCmd = new(medSql, conn);
            if (tran != null)
            {
                medCmd.Transaction = tran;
            }

            medCmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using (var medRdr = medCmd.ExecuteReader())
            {
                while (medRdr.Read())
                {
                    vm.Medicines.Add(new PharmacologyAdvice
                    {
                        MedicineName = medRdr["MedicineName"].ToString(),
                        Dosage = medRdr["Dosage"].ToString(),
                        Frequency = medRdr["Frequency"].ToString(),
                        Duration = medRdr["Duration"].ToString(),
                        Instructions = medRdr["Instructions"].ToString()
                    });
                }
            }

            return vm;
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
                con.Symptoms,
                con.Diagnosis,
                con.Notes,
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
                    vm.Symptoms = rdr["Symptoms"].ToString();
                    vm.Diagnosis = rdr["Diagnosis"].ToString();
                    vm.Notes = rdr["Notes"].ToString();
                    vm.AppointmentId = Convert.ToInt32(rdr["AppointmentId"]);                    
                    vm.ConsultationId = Convert.ToInt32(rdr["ConsultationId"]);

                    prescriptions.Add(vm);
                }
            }

            foreach (PrescriptionVM prescription in prescriptions)
            {
                string medSql = @"
                    SELECT MedicineName,
                            Dosage,
                            Frequency,
                            Duration,
                            Instructions
                    FROM PharmacologyAdvices
                    WHERE ConsultationId=@ConsultationId
                    ORDER BY PharmacologyId";

                using SqlCommand medCmd = new(medSql, conn);
                if (tran != null)
                {
                    medCmd.Transaction = tran;
                }
                medCmd.Parameters.AddWithValue("@ConsultationId", prescription.ConsultationId);

                using var medRdr = medCmd.ExecuteReader();

                while (medRdr.Read())
                {
                    prescription.Medicines.Add(new PharmacologyAdvice
                    {
                        MedicineName = medRdr["MedicineName"].ToString(),
                        Dosage = medRdr["Dosage"].ToString(),
                        Frequency = medRdr["Frequency"].ToString(),
                        Duration = medRdr["Duration"].ToString(),
                        Instructions = medRdr["Instructions"].ToString()
                    });
                }
            }

            return prescriptions;
        }
    }

}

