public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int StructureId { get; set; }
}

public class CreateRoomDto
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int StructureId { get; set; }
}

public class UpdateRoomDto
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int StructureId { get; set; }
}
