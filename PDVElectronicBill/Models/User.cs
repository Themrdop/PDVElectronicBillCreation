namespace Products.Models{
    public class User
    {
        public string Id {get; set;} = string.Empty;
        public string Name {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string Password {get; set;} = string.Empty;
        public string Role {get; set;} = string.Empty;
        public string Token {get; set;} = string.Empty;
        public string RefreshToken {get; set;} = string.Empty;
        public DateTime? RefreshTokenExpiryTime {get; set;} = null;
        public DateTime? TokenExpiryTime {get; set;} = null;
        public string Salt {get; set;} = string.Empty;
    }
}