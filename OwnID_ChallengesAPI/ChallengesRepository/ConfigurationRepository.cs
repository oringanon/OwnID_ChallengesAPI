using CsvHelper;
using CsvHelper.Configuration;
using OwnID_ChallengesAPI.Models.Config;
using System.Formats.Asn1;
using System.Globalization;

namespace OwnID_ChallengesAPI.ChallengesRepository
{
    public class ConfigurationRepository
    {
        private readonly Dictionary<string, SMSConfiguration> _smsConfigurations;
        private readonly Dictionary<string, EmailConfiguration> _emailConfigurations;

        public ConfigurationRepository()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectRootPath = Directory.GetParent(basePath).Parent.Parent.Parent.FullName;

            var smsConfigPath = Path.Combine(projectRootPath, "SMSConfigurationTable.csv");
            var emailConfigPath = Path.Combine(projectRootPath, "EmailConfigurationTable.csv");

            _emailConfigurations = ReadCsvFile<EmailConfiguration>(emailConfigPath);
            _smsConfigurations = ReadCsvFile<SMSConfiguration>(smsConfigPath);
        }

        public SMSConfiguration GetSMSConfiguration(string appId)
        {
            _smsConfigurations.TryGetValue(appId, out var config);
            return config;
        }

        public T GetConfiguration<T>(string appId) where T : class
        {
            if (typeof(T) == typeof(SMSConfiguration))
            {
                return _smsConfigurations.TryGetValue(appId, out var config) ? config as T : null;
            }
            else if (typeof(T) == typeof(EmailConfiguration))
            {
                return _emailConfigurations.TryGetValue(appId, out var config) ? config as T : null;
            }
            else
            {
                throw new InvalidOperationException($"Configuration type {typeof(T).Name} is not supported.");
            }
        }

        public EmailConfiguration GetEmailConfiguration(string appId)
        {
            _emailConfigurations.TryGetValue(appId, out var config);
            return config;
        }

        private Dictionary<string, T> ReadCsvFile<T>(string filePath) where T : class
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            }))
            {
                var records = new Dictionary<string, T>();
                foreach (var record in csv.GetRecords<T>())
                {
                    var appIdProperty = typeof(T).GetProperty("AppId");
                    if (appIdProperty != null)
                    {
                        var appId = appIdProperty.GetValue(record)?.ToString();
                        if (appId != null)
                        {
                            records.Add(appId, record);
                        }
                    }
                }
                return records;
            }
        }
    }
}

