using Microsoft.Extensions.Options;
using System.Text;

namespace DotNetAPI.SandBox
{

    public class AppSettings
    {
        public string? Test { get; set; }

        public MultiSourcesConfig? EncryptApiListString { get; set; }
        public ConnectionStrings? ConnectionStrings { get; set; }

        public string[]? EncryptApiList { get; set; }

        public void Configure()
        {
            var encryptAPIList = EncryptApiListString?.GetValue();
            if (encryptAPIList != null)
            {
                EncryptApiList = System.Text.Json.JsonSerializer.Deserialize<string[]>(encryptAPIList);
            }
        }
    }

    public class ConnectionStrings
    {
        public MultiSourcesConfig? SandBoxConnectionString { get; set; }
    }

    public class MultiSourcesConfig
    {
        private string? _value;

        public bool IsBase64Encoded { get; set; }

        public string? Value { get; set; }

        public string? ENV { get; set; }

        public string GetValue()
        {
            if (_value != null)
            {
                return _value;
            }

            string text = null;
            text = !string.IsNullOrWhiteSpace(ENV) ? Environment.GetEnvironmentVariable(ENV) : Value;
            if (IsBase64Encoded)
            {
                _value = Encoding.UTF8.GetString(Convert.FromBase64String(text));
            }
            else
            {
                _value = text;
            }

            return _value;
        }
    }
}
