public class TrocaPlantao
{
    public int Id { get; set; }

    public string SolicitanteId { get; set; }
    public string DestinatarioId { get; set; }

    public DateTime PlantaoA { get; set; }
    public DateTime PlantaoB { get; set; }

    public string Status { get; set; }
}