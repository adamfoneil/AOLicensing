using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace AOLicensing.Functions.Extensions
{
    public static class HttpRequestExtensions
    {
        public static T BindGet<T>(this HttpRequest request) where T : class, new()
        {
            if (!request.Method.ToLower().Equals("get")) throw new ArgumentException($"Expected 'GET' method, but got '{request.Method}'");

            var result = new T();
            
            var props = typeof(T).GetProperties().Where(p => p.CanWrite).ToArray();

            var matching = request.Query.Join(props, 
                kp => kp.Key.ToLower(), 
                p => p.Name.ToLower(), 
                (kp, p) => new 
                { 
                    KeyPair = kp, 
                    Property = p 
                });

            foreach (var matchedProperty in matching)
            {
                try
                {
                    matchedProperty.Property.SetValue(result, matchedProperty.KeyPair.Value.First());
                }
                catch
                {
                    // ignore
                }
            }

            return result;
        }
    }
}
