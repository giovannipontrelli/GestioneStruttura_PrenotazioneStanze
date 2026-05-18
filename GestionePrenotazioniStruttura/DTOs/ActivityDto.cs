public class ActivityCreateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<int>? TrainerIds { get; set; }
    public int StructureId { get; set; }
}

public class ActivityUpdateDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<int>? TrainerIds { get; set; }
    public int StructureId { get; set; }
}

public class ActivityReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<TrainerReadDto> Trainers { get; set; } = new();
    public int StructureId { get; set; }  // rimane solo in lettura
}
