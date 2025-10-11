using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

public class JwtTokenService
{
    private readonly RSA _privateKey;
    private readonly IConfiguration _configuration;
    private readonly string _issuer;
    private readonly string _audience;
    public JwtTokenService(IConfiguration configuration, string privateKeyFilePath = null!)
    {
        _configuration = configuration;
        var jwtSettings = configuration.GetSection("JwtSettings");

        _issuer = jwtSettings["Issuer"] ?? "ChangeMeIssuer";
        _audience = jwtSettings["Audience"] ?? "ChangeMeAudience";

        string privateKeyPem;
        privateKeyPem = File.ReadAllText(privateKeyFilePath);
        Console.WriteLine($"privateKeyPem length = {privateKeyPem.Length}");
        Console.WriteLine(privateKeyPem.Substring(0, 50));  // affiche un extrait


        _privateKey = CreateRsaFromPem(privateKeyPem);
    }


    private RSA CreateRsaFromPem(string privateKeyPem)
    {
        var rsa = RSA.Create();
        var base64 = privateKeyPem
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim();
        var privateKeyBytes = Convert.FromBase64String(base64);
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        return rsa;
    }

    public string GenerateToken(string userId, string role, TimeSpan? expiresIn = null)
    {
        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromMinutes(60));

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Optionnel : Expose la clé publique pour validation côté client/microservices
    public string GetPublicKey()
    {
        var publicKey = _privateKey.ExportParameters(false); // false = clé publique seulement
        using var rsa = RSA.Create();
        rsa.ImportParameters(publicKey);
        return Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
    }

}
