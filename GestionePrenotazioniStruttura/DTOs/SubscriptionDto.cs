public class CreateSubscriptionDto
{
    public string Name { get; set; } = null!;
    public double TotalPrice { get; set; }
    public int DurationInMonths { get; set; }
    public List<int> ActivitiesIds { get; set; } = new();
    public int UserId { get; set; }
    public int PaymentId { get; set; }
}

public class UpdateSubscriptionDto
{
    public string Name { get; set; } = null!;
    public double TotalPrice { get; set; }
    public int DurationInMonths { get; set; }
    public List<int> ActivitiesIds { get; set; } = new();
    public int UserId { get; set; }
    public int PaymentId { get; set; }
}

public class ReadSubscriptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double TotalPrice { get; set; }
    public int DurationInMonths { get; set; }
    public List<int> ActivitiesIds { get; set; } = new();
    public int UserId { get; set; }
    public int PaymentId { get; set; }
}
