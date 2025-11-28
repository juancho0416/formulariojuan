namespace form.Models
{
    public class Auditoria
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }
        public string Entidad { get; set; }
        public int EntidadId { get; set; }
        public string Fecha { get; set; }
    }
}
