namespace AdaBridge
{
    public enum GenderValues
    {
        Male, 
        Female
    }

    public static class GenderValuesHelper
    {
        public static GenderValues Parse(string stringGender)
        {
            return stringGender.Trim().ToLower() == "male" ? GenderValues.Male : GenderValues.Female;
        }
    }
}
