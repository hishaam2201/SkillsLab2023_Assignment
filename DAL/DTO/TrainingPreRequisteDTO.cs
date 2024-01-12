
namespace DAL.DTO
{
    public class TrainingPreRequisteDTO
    {
        public short? TrainingId { get; set; }
        public int? PreRequisiteId { get; set; }
        public string PreRequisiteName { get; set; }
        public string PreRequisiteDescription { get; set; }
    }
}
