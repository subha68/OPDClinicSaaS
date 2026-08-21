using DAL;
using Models;
using ViewModels;

namespace BLL
{
    public class ReportLookupRegistry
    {
        private readonly IConfiguration _config;

        private static readonly Dictionary<string, ReportLookupDefinition>
            _lookups = new(StringComparer.OrdinalIgnoreCase)
                            {
                                ["PAYMENT_MODE"] = new ReportLookupDefinition("PAYMENT_MODE", 
                                                                                typeof(ReportLookupDAL), 
                                                                                "GetPaymentModes")
                            };

        public ReportLookupRegistry(IConfiguration config)
        {
            _config = config;
        }

        public List<ReportLookupItem> Get(string lookupCode, int clinicId)
        {
            if (string.IsNullOrWhiteSpace(lookupCode))
            {
                return new List<ReportLookupItem>();
            }

            if (!_lookups.TryGetValue(lookupCode, out ReportLookupDefinition? definition))
            {
                throw new InvalidOperationException($"Lookup '{lookupCode}' is not registered.");
            }

            object dal = Activator.CreateInstance(definition.DALType, _config)!
                                                        ?? throw new InvalidOperationException(
                                                            $"Unable to create lookup DAL " +
                                                            $"'{definition.DALType.Name}'.");

            var method = definition.DALType.GetMethod(definition.MethodName);

            if (method == null)
            {
                throw new InvalidOperationException($"Lookup method '{definition.MethodName}' " +
                                                        $"was not found on " +
                                                        $"'{definition.DALType.Name}'.");
            }

            using var conn = new DbHelper(_config).GetConnection();

            conn.Open();

            object? result = method.Invoke(dal, new object?[]
                                                    {
                                                        clinicId,
                                                        conn,
                                                        null
                                                    });

            return result as List<ReportLookupItem>
                   ?? throw new InvalidOperationException($"Lookup '{lookupCode}' returned " + $"an invalid result.");
        }
    }
}