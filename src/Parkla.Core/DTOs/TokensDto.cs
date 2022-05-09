namespace Parkla.Core.DTOs
{
    public class TokensDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Expires { get; set; }
    }
}