public class ReadPaymentDto
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public int SubscriptionId { get; set; }
}

public class CreatePaymentDto
{
    public double Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public int SubscriptionId { get; set; }
}

public class UpdatePaymentDto : CreatePaymentDto { }
