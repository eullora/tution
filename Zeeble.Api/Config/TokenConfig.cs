namespace Zeeble.Api.Config
{
    public interface ITokenConfig
    {
        string SymmetricKey { get; set; }
        string Issuer { get; set; }
    }
    public class TokenConfig : ITokenConfig
    {
        public string SymmetricKey { get; set; }
        public string Issuer { get; set; }

        public TokenConfig(string symmetricKey, string issuer)
        {
            SymmetricKey = symmetricKey;
            Issuer = issuer;
        }
    }
}
