using AOLicensing.Functions.Models;
using AOLicensing.Shared.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AOLicensing.Functions
{
    public class KeyStore
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public KeyStore(StorageAccountOptions options)
        {
            _connectionString = options.ConnectionString;
            _containerName = options.ContainerName;
        }

        public async Task SaveKeyAsync(LicenseKey licenseKey)
        {
            var blobClient = new BlobClient(_connectionString, _containerName, GetBlobName(licenseKey));            
            var exists = await FindKeyAsync(licenseKey);            
            var data = (exists.result) ? exists.data : new HashSet<KeyInfo>();
            
            data.Add(new KeyInfo() { Key = licenseKey.Key });

            var json = JsonConvert.SerializeObject(data);
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                await blobClient.UploadAsync(ms, new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = "application/json"
                    }
                });
            }
        }

        public async Task<(bool result, string message)> ValidateKeyAsync(LicenseKey licenseKey)
        {
            var exists = await FindKeyAsync(licenseKey);

            if (exists.result)
            {
                bool success = exists.data.Contains(new KeyInfo() { Key = licenseKey.Key });
                string message = (success) ? "Key is valid" : "Key is not valid";
                return (success, message);
            }
            else
            {
                return (false, $"Key not found: {GetBlobName(licenseKey)}");
            }
        }

        public async Task<(bool result, HashSet<KeyInfo> data)> FindKeyAsync(Shared.Models.CreateKey keyInfo)
        {
            var blobClient = new BlobClient(_connectionString, _containerName, GetBlobName(keyInfo));
            if (await blobClient.ExistsAsync())
            {
                var result = await blobClient.DownloadAsync();
                using (var reader = new StreamReader(result.Value.Content))
                {
                    string json = await reader.ReadToEndAsync();
                    return (true, JsonConvert.DeserializeObject<HashSet<KeyInfo>>(json));
                }
            }

            return (false, null);
        }

        private string GetBlobName(Shared.Models.CreateKey key) => $"{key.Product}/{key.Email}.json";

        public class KeyInfo
        {
            public string Key { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;

            public override bool Equals(object obj)
            {
                var test = obj as KeyInfo;
                return (test != null) ? test.Key.Equals(Key) : base.Equals(obj);
            }

            public override int GetHashCode() => Key.GetHashCode();            
        }
    }
}
