using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibCinona.Extensions
{
    public static class EncodingExtensions
    {
        public static byte[] Serialize<T>(this T data)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, new JsonSerializerOptions()
            {
                IncludeFields = true,
            }));
        }
        public static T Deserialize<T>(this byte[] data) => 
            JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));
    }
}
