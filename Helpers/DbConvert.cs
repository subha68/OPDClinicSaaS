namespace Helpers
{
    public static class DbConvert
    {
        public static int ToInt(object value)
        {
            return value == null || value == DBNull.Value
                ? 0
                : Convert.ToInt32(value);
        }

        public static string ToString(object value)
        {
            return value == null || value == DBNull.Value
                ? ""
                : value.ToString();
        }
    } 

}