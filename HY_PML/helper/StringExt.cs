namespace HY_PML.helper
{
    public static class StringExt
    {
        public static bool IsEmpty(this string val)
        {
            return string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val);
        }

        public static bool IsNotEmpty(this string val)
        {
            return !IsEmpty(val);
        }
    }
}
