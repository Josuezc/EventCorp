namespace EventCorp.Models
{
    public class Dashboard
    {
        public int TotalEventos { get; set; }
        public int TotalUsuarios { get; set; }
        public int AsistentesxMes { get; set; }
        public List<Evento> Top5Eventos { get; set; } = new();
    }
}
