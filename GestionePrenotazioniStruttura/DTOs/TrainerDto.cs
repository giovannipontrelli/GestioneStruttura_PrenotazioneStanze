public class TrainerReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<ActivityReadDto> Activities { get; set; } = new();
}

public class TrainerCreateDto
{
    public string Name { get; set; } = null!;
    public List<int>? ActivityIds { get; set; }
}

public class TrainerUpdateDto
{
    public string Name { get; set; } = null!;
    public List<int>? ActivityIds { get; set; }
}
