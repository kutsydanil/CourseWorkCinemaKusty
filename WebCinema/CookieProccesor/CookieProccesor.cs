namespace WebCinema.CookieProccesorNamespace
{
    public class CookieProccesor
    {
        public static string? GetSetValue(string key, string value, bool first, HttpRequest request, HttpResponse response)
        {
            if (!string.IsNullOrEmpty(value))
            {
                response.Cookies.Append(key, value);
                return value;
            }
            else
            {
                if (request.Cookies.ContainsKey(key) && first)
                {
                    value = request.Cookies[key];
                    response.Cookies.Delete(key);
                }
                return value;
            }
        }

        public static T? GetSetValue<T>(string key, string value, bool first, HttpRequest request, HttpResponse response)
        {
            try
            {
                return (T)Convert.ChangeType(GetSetValue(key, value, first, request, response), typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
}
