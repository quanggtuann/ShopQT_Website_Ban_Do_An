using ShopAPI.DTOs;
using System.Text.Json;

namespace ShopAPI.Services
{
    public class LocationService
    {
        private const string VietnamLabsUrl = "https://vietnamlabs.com/api/vietnamprovince";
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProvinceDto>> GetAll()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, VietnamLabsUrl);
            request.Headers.UserAgent.ParseAdd("ShopDAL/1.0");

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return ParseVietnamLabsProvinces(json);
        }

        private static List<ProvinceDto> ParseVietnamLabsProvinces(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var provinceArray = root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("data", out var data)
                && data.ValueKind == JsonValueKind.Array
                    ? data
                    : root;

            if (provinceArray.ValueKind != JsonValueKind.Array)
            {
                return new List<ProvinceDto>();
            }

            return provinceArray
                .EnumerateArray()
                .Select(MapProvince)
                .Where(province => !string.IsNullOrWhiteSpace(province.Province))
                .ToList();
        }

        private static ProvinceDto MapProvince(JsonElement provinceElement)
        {
            return new ProvinceDto
            {
                Province = GetString(provinceElement, "province", "name", "full_name", "name_with_type"),
                Wards = ReadWards(provinceElement).ToList(),
                Districts = ReadDistricts(provinceElement).ToList()
            };
        }

        private static IEnumerable<DistrictDto> ReadDistricts(JsonElement provinceElement)
        {
            if (!TryGetArray(provinceElement, out var districtsElement, "districts", "Districts"))
            {
                yield break;
            }

            foreach (var districtElement in districtsElement.EnumerateArray())
            {
                var district = new DistrictDto
                {
                    District = GetString(districtElement, "district", "name", "full_name", "name_with_type"),
                    Wards = ReadWards(districtElement).ToList()
                };

                if (!string.IsNullOrWhiteSpace(district.District))
                {
                    yield return district;
                }
            }
        }

        private static IEnumerable<string> ReadWards(JsonElement element)
        {
            if (!TryGetArray(element, out var wardsElement, "wards", "Wards"))
            {
                yield break;
            }

            foreach (var wardElement in wardsElement.EnumerateArray())
            {
                var ward = wardElement.ValueKind == JsonValueKind.String
                    ? wardElement.GetString()
                    : GetString(wardElement, "ward", "name", "full_name", "name_with_type");

                if (!string.IsNullOrWhiteSpace(ward))
                {
                    yield return ward;
                }
            }
        }

        private static bool TryGetArray(JsonElement element, out JsonElement array, params string[] propertyNames)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var propertyName in propertyNames)
                {
                    if (element.TryGetProperty(propertyName, out array) && array.ValueKind == JsonValueKind.Array)
                    {
                        return true;
                    }
                }
            }

            array = default;
            return false;
        }

        private static string GetString(JsonElement element, params string[] propertyNames)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            foreach (var propertyName in propertyNames)
            {
                if (element.TryGetProperty(propertyName, out var value))
                {
                    return value.ValueKind == JsonValueKind.String
                        ? value.GetString() ?? string.Empty
                        : value.ToString();
                }
            }

            return string.Empty;
        }
    }
}
