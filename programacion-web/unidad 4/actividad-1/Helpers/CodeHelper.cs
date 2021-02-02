using System;

namespace Actividad1.Helpers
{
    public static class CodeHelper
    {
        public static int GetCode()
        {
            Random random = new Random();
            int codigo1 = random.Next(1000, 9999);
            int codigo2 = random.Next(1000, 9999);
            return (codigo1 + codigo2);
        }
    }
}