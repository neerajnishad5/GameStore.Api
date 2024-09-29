using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
namespace GameStore.Api.Services;

public enum PasswordVerificationResult {
    Failed, Success, SuccessRehashNeeded
}

public static class UserService {
    private const byte VersionId1 = 0x01;
    private const byte DefaultVersionId = VersionId1; 

    // Defines versions for password hashing
    private static readonly Dictionary<byte, PasswordHasherVersion> Versions = new() {
        [VersionId1] = new PasswordHasherVersion(HashAlgorithmName.SHA256, 32, 32, 600000), // Sizes in bytes
    };

    // Hashes a password
    public static string HashPassword(string password) {
        ArgumentNullException.ThrowIfNull(password);

        var version = Versions[DefaultVersionId];
        var hashedPasswordBytes = new byte[1 + version.SaltSize + version.KeySize];

        // Set the version ID and generate a salt
        hashedPasswordBytes[0] = DefaultVersionId;
        RandomNumberGenerator.Fill(hashedPasswordBytes.AsSpan(1, version.SaltSize));

        // Derive the key using the password and salt
        Rfc2898DeriveBytes.Pbkdf2(password, hashedPasswordBytes.AsSpan(1, version.SaltSize), hashedPasswordBytes.AsSpan(1 + version.SaltSize), version.Iterations, version.Algorithm);

        return Convert.ToBase64String(hashedPasswordBytes);
    }

    // Verifies a hashed password against a provided password
    public static PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword) {
        ArgumentNullException.ThrowIfNull(hashedPassword);
        ArgumentNullException.ThrowIfNull(providedPassword);

        // Calculate the size of the decoded hashed password
        var hashedPasswordByteCount = ComputeDecodedBase64ByteCount(hashedPassword);
        var hashedPasswordBytes = new byte[hashedPasswordByteCount];

        // Convert Base64 string to byte array
        if (!Convert.TryFromBase64String(hashedPassword, hashedPasswordBytes, out _)) {
            return PasswordVerificationResult.Failed; // Invalid format
        }

        if (hashedPasswordBytes.Length == 0 || 
            !Versions.TryGetValue(hashedPasswordBytes[0], out var version) ||
            hashedPasswordBytes.Length != 1 + version.SaltSize + version.KeySize) {
            return PasswordVerificationResult.Failed; // Invalid data
        }

        var saltBytes = hashedPasswordBytes.AsSpan(1, version.SaltSize);
        var expectedKeyBytes = hashedPasswordBytes.AsSpan(1 + version.SaltSize, version.KeySize);
        var actualKeyBytes = new byte[version.KeySize];

        // Create a new key from the provided password
        Rfc2898DeriveBytes.Pbkdf2(providedPassword, saltBytes.ToArray(), actualKeyBytes, version.Iterations, version.Algorithm);

        // Compare the keys securely
        if (!CryptographicOperations.FixedTimeEquals(expectedKeyBytes.ToArray(), actualKeyBytes)) {
            return PasswordVerificationResult.Failed; // Keys do not match
        }

        // Check if rehashing is needed
        return hashedPasswordBytes[0] != DefaultVersionId ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Success;
    }

    // Computes the byte count for a Base64 string
    private static int ComputeDecodedBase64ByteCount(string base64Str) {
        var characterCount = base64Str.Length;
        var paddingCount = 0;

        // Count padding characters at the end
        if (characterCount > 0) {
            if (base64Str[characterCount - 1] == '=') {
                paddingCount++;
                if (characterCount > 1 && base64Str[characterCount - 2] == '=') {
                    paddingCount++;
                }
            }
        }

        // Calculate and return the byte count
        return (characterCount * 3 / 4) - paddingCount;
    }
    // A record to hold version details for password hashing
    private sealed record PasswordHasherVersion(HashAlgorithmName Algorithm, int SaltSize, int KeySize, int Iterations);
}