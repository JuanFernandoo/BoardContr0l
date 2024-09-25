namespace BoardContr0l.Dto
{
    public class CategoryDto // Transporte de objetos para que no repita un ciclo
    {
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ? ModifiedDate { get; set; }
        public bool Status { get; set; }

    }
}
