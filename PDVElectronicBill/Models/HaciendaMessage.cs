namespace Products.Models
{
    public class HaciendaMessage
    {
        public string clave {get; set;} = string.Empty;
        public string fecha {get; set;} = string.Empty;
        public Person emisor {get; set;} = new();
        public Person receptor {get; set;} = new();
        public string comprobanteXml {get; set;} =  string.Empty;

        public override string ToString()
        {
            return $"Mensaje de hacienda con clave {clave} emitido por {emisor} para {receptor}";
        }
    }
}