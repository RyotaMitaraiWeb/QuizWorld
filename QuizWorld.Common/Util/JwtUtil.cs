namespace QuizWorld.Common.Util
{
    /// <summary>
    /// Utility methods for working with JWTs
    /// </summary>
    public static class JwtUtil
    {
        /// <summary>
        /// Removes the "Bearer " (with the space included) part of the token
        /// attached to the Authorization header
        /// </summary>
        /// <param name="bearer">If null, returns an empty string</param>
        /// <returns></returns>
        public static string RemoveBearer(string? bearer)
        {
            if (bearer is null) return string.Empty;

            return bearer.Replace("Bearer ", string.Empty);
        }
    }
}
