namespace TinyUrlBackend.Services;

public static class ShortenerService
{
    public static string GenerateCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        // Requirement: Generate a 6-character short code
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}