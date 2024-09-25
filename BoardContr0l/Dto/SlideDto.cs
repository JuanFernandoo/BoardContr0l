using BoardContr0l.Dto;
using System.Text;
using System.Text.Json.Serialization;

namespace BoardContr0l.Dto
{
    public class SlideDto // Transporte de objetos para que no repita un ciclo
    {
        public int SlideId { get; set; }
        public string Description { get; set; } 
        public string URL { get; set; } 
        public int Time { get; set; }

        [JsonIgnore] 
        public BoardDto Board { get; set; } // Asocia propiedades de la tabla category
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ? ModifiedDate { get; set; }
    }
}
