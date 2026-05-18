public class BookingDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CreateBookingDto
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UpdateBookingDto
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public int ActivityId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
