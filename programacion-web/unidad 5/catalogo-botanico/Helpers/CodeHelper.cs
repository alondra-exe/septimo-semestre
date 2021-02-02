using System;

namespace CatalogoBotanico.Helpers
{
    public static class CodeHelper
    {
        public static int GetCode()
        {
            Random r = new Random();
            int code1 = r.Next(1000, 9999);
            int code2 = r.Next(1000, 9999);
            return (code1 + code2);
        }
    }
}