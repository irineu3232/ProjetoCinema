namespace Cinema.Models
{
    public class Usuario
    {
        public int id { get; set; }
        
        public string? Nome { get; set; }
        
        public string? Email { get; set; }
        
        public string? Senha { get; set; }
        
        public string? role { get; set; }

        public string? Ativo { get; set; }
    }
}
