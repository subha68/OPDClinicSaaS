using DAL;
using Models;

namespace BLL
{
    public static class ReportRegistry
    {
        private static readonly Dictionary<string, ReportExecutionDefinition>
        _reports = new(StringComparer.OrdinalIgnoreCase)
                        {
                            ["DAILY_APPOINTMENT_SUMMARY"] = new ReportExecutionDefinition("DAILY_APPOINTMENT_SUMMARY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetDailyAppointmentRows",
                                                                                        "GetDailyAppointmentSummary",
                                                                                        typeof(DailyAppointmentSummaryFilter),
                                                                                        typeof(DailyAppointmentSummaryRow)),
                            ["DOCTOR_APPOINTMENT_SUMMARY"] = new ReportExecutionDefinition("DOCTOR_APPOINTMENT_SUMMARY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetDoctorAppointmentRows",
                                                                                        null,
                                                                                        typeof(DoctorAppointmentSummaryFilter),
                                                                                        typeof(DoctorAppointmentSummaryRow)),
                            ["PATIENT_REGISTRATION_SUMMARY"] = new ReportExecutionDefinition("PATIENT_REGISTRATION_SUMMARY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetPatientRegistrationRows",
                                                                                        null,
                                                                                        typeof(PatientRegistrationSummaryFilter),
                                                                                        typeof(PatientRegistrationSummaryRow)),                            
                            ["DAILY_COLLECTION_SUMMARY"] = new ReportExecutionDefinition("DAILY_COLLECTION_SUMMARY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetDailyCollectionRows",
                                                                                        "GetDailyCollectionSummary",
                                                                                        typeof(DailyCollectionDataFilter),
                                                                                        typeof(DailyCollectionDataRow)),
                            ["OUTSTANDING_PATIENT_BALANCES"] = new ReportExecutionDefinition("OUTSTANDING_PATIENT_BALANCES",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetOutstandingPatientBalanceRows",
                                                                                        "GetOutstandingPatientBalanceSummary",
                                                                                        typeof(OutstandingPatientBalanceFilter),
                                                                                        typeof(OutstandingPatientBalanceRow)),
                            ["CANCELLED_NO_SHOW_APPOINTMENTS"] = new ReportExecutionDefinition("CANCELLED_NO_SHOW_APPOINTMENTS",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetCancelledNoshowAppointmentRows",
                                                                                        "GetCancelledNoshowAppointmentSummary",
                                                                                        typeof(CancelledNoshowAppointmentFilter),
                                                                                        typeof(CancelledNoshowAppointmentRow)),
                            ["DOCTOR_REVENUE_SUMMARY"] = new ReportExecutionDefinition("DOCTOR_REVENUE_SUMMARY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetDoctorRevenueRows",
                                                                                        "GetDoctorRevenueSummary",
                                                                                        typeof(DoctorRevenueFilter),
                                                                                        typeof(DoctorRevenueRow)),
                            ["PATIENT_VISIT_HISTORY"] = new ReportExecutionDefinition("PATIENT_VISIT_HISTORY",
                                                                                        typeof(ReportSpecificDAL),
                                                                                        "GetPatientVisitHistoryRows",
                                                                                        "GetPatientVisitHistorySummary",
                                                                                        typeof(PatientVisitHistoryFilter),
                                                                                        typeof(PatientVisitHistoryRow)),

                        };

        public static ReportExecutionDefinition? Get(string reportCode)
        {
            return _reports.TryGetValue(reportCode, out var definition) ? definition : null;
        }
    }

}