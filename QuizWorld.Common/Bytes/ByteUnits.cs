namespace QuizWorld.Common.Bytes
{
    /// <summary>
    /// All enum values are mapped to their powers of 1024 
    /// (for example, megabytes are 1024 bytes to the power of 2), which you can cast to
    /// integers if required.
    /// </summary>
    public enum ByteUnits
    {
        Bytes = 0,
        Kilobytes = 1,
        Megabytes = 2,
    }
}
