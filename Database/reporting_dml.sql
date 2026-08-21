/* ============================================================
   CLINIC REPORTING ENGINE
   INITIAL REPORT METADATA
   ============================================================ */


/* ============================================================
   1. REPORT DEFINITIONS
   ============================================================ */

insert into reporting.report
(report_id, report_code, report_name, description, data_source_code, report_type, is_active)
values
(1001, 'daily_appointment_summary', 'Daily Appointment Summary', 'Summary of clinic appointments for a selected date.', 'daily_appointment_summary', 'tabular', 1),
(1002, 'doctor_appointment_summary', 'Doctor-wise Appointment Summary', 'Appointment count and status summary by doctor for a selected date range.', 'doctor_appointment_summary', 'tabular', 1),
(1003, 'patient_registration_summary', 'Patient Registration Summary', 'Summary of new patient registrations for a selected date range.', 'patient_registration_summary', 'tabular', 1),
(1004, 'daily_collection_summary', 'Daily Collection Summary', 'Summary of collections by payment mode for a selected date range.', 'daily_collection_summary', 'tabular', 1),
(1005, 'outstanding_patient_balances', 'Outstanding Patient Balances', 'List of patient invoices and balances that remain outstanding.', 'outstanding_patient_balances', 'tabular', 1),
(1006, 'cancelled_no_show_appointments', 'Cancelled and No-show Appointments', 'Appointments that were cancelled or marked as no-show during a selected period.', 'cancelled_no_show_appointments', 'tabular', 1),
(1007, 'doctor_revenue_summary', 'Doctor-wise Revenue Summary', 'Revenue summary by doctor for a selected date range.', 'doctor_revenue_summary', 'tabular', 1),
(1008, 'patient_visit_history', 'Patient Visit History', 'Visit and consultation history for a selected patient.', 'patient_visit_history', 'tabular', 1);


/* ============================================================
   2. REPORT COLUMNS
   ============================================================ */


/* ------------------------------------------------------------
   1001 - Daily Appointment Summary
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100101, 1001, 'appointment_time', 'Time', 'datetime', 1, 1, 'HH:mm'),
(100102, 1001, 'patient_name', 'Patient', 'string', 2, 1, null),
(100103, 1001, 'patient_number', 'Patient No.', 'string', 3, 1, null),
(100104, 1001, 'doctor_name', 'Doctor', 'string', 4, 1, null),
(100105, 1001, 'appointment_type', 'Appointment Type', 'string', 5, 1, null),
(100106, 1001, 'appointment_status', 'Status', 'string', 6, 1, null);


/* ------------------------------------------------------------
   1002 - Doctor-wise Appointment Summary
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100201, 1002, 'doctor_name', 'Doctor', 'string', 1, 1, null),
(100202, 1002, 'appointment_count', 'Appointments', 'integer', 2, 1, '#,##0'),
(100203, 1002, 'completed_count', 'Completed', 'integer', 3, 1, '#,##0'),
(100204, 1002, 'cancelled_count', 'Cancelled', 'integer', 4, 1, '#,##0'),
(100205, 1002, 'no_show_count', 'No-show', 'integer', 5, 1, '#,##0');


/* ------------------------------------------------------------
   1003 - Patient Registration Summary
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100301, 1003, 'registration_date', 'Date', 'date', 1, 1, 'yyyy-mm-dd'),
(100302, 1003, 'registration_count', 'New Patients', 'integer', 2, 1, '#,##0');


/* ------------------------------------------------------------
   1004 - Daily Collection Summary
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100401, 1004, 'collection_date',   'Date',         'date',    1, 1, 'yyyy-mm-dd'),
(100402, 1004, 'payment_mode',      'Payment Mode', 'string',  2, 1, null),
(100403, 1004, 'transaction_count', 'Transactions', 'integer', 3, 1, '#,##0'),
(100404, 1004, 'collection_amount', 'Amount',        'decimal', 4, 1, '#,##0.00'),
(100405, 1004, 'refund_amount',     'Refund',        'decimal', 5, 1, '#,##0.00'),
(100406, 1004, 'total_amount',       'Total',         'decimal', 6, 1, '#,##0.00');


/* ------------------------------------------------------------
   1005 - Outstanding Patient Balances
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100501, 1005, 'patient_number', 'Patient No.', 'string', 1, 1, null),
(100502, 1005, 'patient_name', 'Patient', 'string', 2, 1, null),
(100503, 1005, 'invoice_number', 'Invoice No.', 'string', 3, 1, null),
(100504, 1005, 'invoice_date', 'Invoice Date', 'date', 4, 1, 'yyyy-mm-dd'),
(100505, 1005, 'invoice_amount', 'Invoice Amount', 'decimal', 5, 1, '#,##0.00'),
(100506, 1005, 'paid_amount', 'Paid Amount', 'decimal', 6, 1, '#,##0.00'),
(100507, 1005, 'outstanding_amount', 'Outstanding', 'decimal', 7, 1, '#,##0.00');


/* ------------------------------------------------------------
   1006 - Cancelled / No-show Appointments
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100601, 1006, 'appointment_date', 'Date', 'date', 1, 1, 'yyyy-mm-dd'),
(100602, 1006, 'appointment_time', 'Time', 'datetime', 2, 1, 'HH:mm'),
(100603, 1006, 'patient_name', 'Patient', 'string', 3, 1, null),
(100604, 1006, 'doctor_name', 'Doctor', 'string', 4, 1, null),
(100605, 1006, 'appointment_status', 'Status', 'string', 5, 1, null),
(100606, 1006, 'remarks', 'Remarks', 'string', 6, 1, null);


/* ------------------------------------------------------------
   1007 - Doctor-wise Revenue Summary
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100701, 1007, 'doctor_name', 'Doctor', 'string', 1, 1, null),
(100702, 1007, 'consultation_count', 'Consultations', 'integer', 2, 1, '#,##0'),
(100703, 1007, 'gross_amount', 'Gross Amount', 'decimal', 3, 1, '#,##0.00'),
(100704, 1007, 'discount_amount', 'Discount', 'decimal', 4, 1, '#,##0.00'),
(100705, 1007, 'net_amount', 'Net Amount', 'decimal', 5, 1, '#,##0.00');


/* ------------------------------------------------------------
   1008 - Patient Visit History
   ------------------------------------------------------------ */

insert into reporting.report_column
(report_column_id, report_id, column_code, display_label, data_type, display_order, is_visible, format_pattern)
values
(100801, 1008, 'visit_date', 'Visit Date', 'date', 1, 1, 'yyyy-mm-dd'),
(100802, 1008, 'doctor_name', 'Doctor', 'string', 2, 1, null),
(100803, 1008, 'visit_type', 'Visit Type', 'string', 3, 1, null),
(100804, 1008, 'diagnosis', 'Diagnosis', 'string', 4, 1, null),
(100805, 1008, 'amount', 'Amount', 'decimal', 5, 1, '#,##0.00');


/* ============================================================
   3. REPORT FILTERS
   ============================================================ */


/* ------------------------------------------------------------
   1001 - Daily Appointment Summary
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100101, 1001, 'appointment_date', 'Appointment Date', 'appointment_date', 'date', 'date', null, null, 1, 1, 'date', NULL),
(100102, 1001, 'doctor_id', 'Doctor', 'doctor_id', 'integer', 'lookup', null, null, 0, 2, NULL, NULL);


/* ------------------------------------------------------------
   1002 - Doctor-wise Appointment Summary
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100201, 1002, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 1, 1, 'date_before_or_equal', 'date_to'),
(100202, 1002, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 1, 2, 'date', null),
(100203, 1002, 'doctor_id', 'Doctor', 'doctor_id', 'integer', 'lookup', null, null, 0, 3, null, null);


/* ------------------------------------------------------------
   1003 - Patient Registration Summary
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100301, 1003, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 1, 1, 'date_before_or_equal', 'date_to'),
(100302, 1003, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 1, 2, 'date', null);


/* ------------------------------------------------------------
   1004 - Daily Collection Summary
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100401, 1004, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 1, 1, 'date_before_or_equal', 'date_to'),
(100402, 1004, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 1, 2, 'date', null),
(100403, 1004, 'payment_mode', 'Payment Mode', 'payment_mode', 'string', 'select', 'PAYMENT_MODE', null, 0, 3, null, null);


/* ------------------------------------------------------------
   1005 - Outstanding Patient Balances
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100501, 1005, 'as_of_date', 'As of Date', 'as_of_date', 'date', 'date', null, null, 1, 1, 'date', null),
(100502, 1005, 'patient_id', 'Patient', 'patient_id', 'integer', 'lookup', null, null, 0, 2, null, null);


/* ------------------------------------------------------------
   1006 - Cancelled / No-show Appointments
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100601, 1006, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 1, 1, 'date_before_or_equal', 'date_to'),
(100602, 1006, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 1, 2, 'date', null),
(100603, 1006, 'doctor_id', 'Doctor', 'doctor_id', 'integer', 'lookup', null, null, 0, 3, null, null),
(100604, 1006, 'appointment_status', 'Status', 'appointment_status', 'string', 'select', 'APPOINTMENT_STATUS', null, 0, 4, null, null);


/* ------------------------------------------------------------
   1007 - Doctor-wise Revenue Summary
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100701, 1007, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 1, 1, 'date_before_or_equal', 'date_to'),
(100702, 1007, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 1, 2, 'date', null),
(100703, 1007, 'doctor_id', 'Doctor', 'doctor_id', 'integer', 'lookup', null, null, 0, 3, null, null);


/* ------------------------------------------------------------
   1008 - Patient Visit History
   ------------------------------------------------------------ */

insert into reporting.report_filter
(report_filter_id, report_id, filter_code, filter_label, parameter_name, data_type, control_type, lookup_code, default_value, is_required, display_order, validation_rule, validation_value)
values
(100801, 1008, 'patient_id', 'Patient', 'patient_id', 'integer', 'lookup', null, null, 1, 1, null, null),
(100802, 1008, 'date_from', 'From Date', 'date_from', 'date', 'date', null, null, 0, 2, 'date_before_or_equal', 'date_to'),
(100803, 1008, 'date_to', 'To Date', 'date_to', 'date', 'date', null, null, 0, 3, 'date', null);


/* ============================================================
   4. BASIC PERMISSIONS
   ============================================================ */

insert into reporting.report_permission
(report_permission_id, report_id, principal_type, principal_id, permission_code)
values
(1001001, 1001, 'role', 1, 'view'),
(1001002, 1002, 'role', 1, 'view'),
(1001003, 1003, 'role', 1, 'view'),
(1001004, 1004, 'role', 5, 'export'),
(1001005, 1004, 'role', 5, 'print'),
(1001006, 1005, 'role', 1, 'view'),
(1001007, 1006, 'role', 1, 'view'),
(1001008, 1007, 'role', 1, 'view'),
(1001009, 1008, 'role', 1, 'view');


-- ============================================================
-- Report menus
-- ============================================================

INSERT INTO Features (Name, Code) 
VALUES
('Daily Appointment Summary',        'REPORT_DAILY_APPOINTMENT_SUMMARY'),
('Doctor-wise Appointment Summary',  'REPORT_DOCTOR_APPOINTMENT_SUMMARY'),
('Patient Registration Summary',     'REPORT_PATIENT_REGISTRATION_SUMMARY'),
('Daily Collection Summary',         'REPORT_DAILY_COLLECTION_SUMMARY'),
('Outstanding Patient Balances',     'REPORT_OUTSTANDING_PATIENT_BALANCES'),
('Cancelled and No-show Appointments','REPORT_CANCELLED_NO_SHOW_APPOINTMENTS'),
('Doctor-wise Revenue Summary',      'REPORT_DOCTOR_REVENUE_SUMMARY'),
('Patient Visit History',             'REPORT_PATIENT_VISIT_HISTORY');

-- Clinic Admin Access
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 2, FeatureId FROM Features WHERE Code IN ('REPORT_DAILY_APPOINTMENT_SUMMARY',
                                                'REPORT_DOCTOR_APPOINTMENT_SUMMARY',
                                                'REPORT_PATIENT_REGISTRATION_SUMMARY',
                                                'REPORT_DAILY_COLLECTION_SUMMARY',
                                                'REPORT_OUTSTANDING_PATIENT_BALANCES',
                                                'REPORT_CANCELLED_NO_SHOW_APPOINTMENTS',
                                                'REPORT_DOCTOR_REVENUE_SUMMARY',
                                                'REPORT_PATIENT_VISIT_HISTORY'
                                             );

-- ClinicUser Access
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 3, FeatureId FROM Features WHERE Code IN ('REPORT_DAILY_APPOINTMENT_SUMMARY',
                                                'REPORT_DOCTOR_APPOINTMENT_SUMMARY',
                                                'REPORT_PATIENT_REGISTRATION_SUMMARY',
                                                'REPORT_CANCELLED_NO_SHOW_APPOINTMENTS',
                                                'REPORT_PATIENT_VISIT_HISTORY'
                                             );

-- Doctor Access
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 4, FeatureId FROM Features WHERE Code IN ('REPORT_DAILY_APPOINTMENT_SUMMARY',
                                                'REPORT_DOCTOR_APPOINTMENT_SUMMARY',
                                                'REPORT_CANCELLED_NO_SHOW_APPOINTMENTS',
                                                'REPORT_DOCTOR_REVENUE_SUMMARY',
                                                'REPORT_PATIENT_VISIT_HISTORY'
                                             );

-- BillingUser Access
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 5, FeatureId FROM Features WHERE Code IN ('REPORT_DAILY_COLLECTION_SUMMARY',
                                                'REPORT_OUTSTANDING_PATIENT_BALANCES',
                                                'REPORT_DOCTOR_REVENUE_SUMMARY'
                                             );

-- Nurse Access
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 7, FeatureId FROM Features WHERE Code IN ('REPORT_DAILY_APPOINTMENT_SUMMARY',
                                                'REPORT_DOCTOR_APPOINTMENT_SUMMARY',
                                                'REPORT_PATIENT_REGISTRATION_SUMMARY',
                                                'REPORT_CANCELLED_NO_SHOW_APPOINTMENTS',
                                                'REPORT_PATIENT_VISIT_HISTORY'
                                             );

-- ============================================================
-- Reports parent menu
-- FeatureCode is NULL because this is a container menu.
-- ============================================================

INSERT INTO Menus
(Title, Controller, Action, Icon, FeatureCode, ParentId, DisplayOrder, IsActive, ReportCode)
VALUES
('Reports', NULL, NULL, 'fa-chart-column', NULL, NULL, 100, 1, NULL);

DECLARE @ReportsMenuId INT = SCOPE_IDENTITY();

INSERT INTO Menus
(Title, Controller, Action, Icon, FeatureCode, ParentId, DisplayOrder, IsActive, ReportCode)
VALUES
('Daily Appointment Summary', 'Report', 'Index', 'fa-calendar-day', 'REPORT_DAILY_APPOINTMENT_SUMMARY', @ReportsMenuId, 1, 1, 'daily_appointment_summary'),
('Doctor-wise Appointment Summary', 'Report', 'Index', 'fa-user-doctor', 'REPORT_DOCTOR_APPOINTMENT_SUMMARY', @ReportsMenuId, 2, 1, 'doctor_appointment_summary'),
('Patient Registration Summary', 'Report', 'Index', 'fa-user-plus', 'REPORT_PATIENT_REGISTRATION_SUMMARY', @ReportsMenuId, 3, 1, 'patient_registration_summary'),
('Daily Collection Summary', 'Report', 'Index', 'fa-money-bill-wave', 'REPORT_DAILY_COLLECTION_SUMMARY', @ReportsMenuId, 4, 1, 'daily_collection_summary'),
('Outstanding Patient Balances', 'Report', 'Index', 'fa-file-invoice-dollar', 'REPORT_OUTSTANDING_PATIENT_BALANCES', @ReportsMenuId, 5, 1, 'outstanding_patient_balances'),
('Cancelled and No-show Appointments', 'Report', 'Index', 'fa-calendar-xmark', 'REPORT_CANCELLED_NO_SHOW_APPOINTMENTS', @ReportsMenuId, 6, 1, 'cancelled_no_show_appointments'),
('Doctor-wise Revenue Summary', 'Report', 'Index', 'fa-chart-line', 'REPORT_DOCTOR_REVENUE_SUMMARY', @ReportsMenuId, 7, 1, 'doctor_revenue_summary'),
('Patient Visit History', 'Report', 'Index', 'fa-clock-rotate-left', 'REPORT_PATIENT_VISIT_HISTORY', @ReportsMenuId, 8, 1, 'patient_visit_history');