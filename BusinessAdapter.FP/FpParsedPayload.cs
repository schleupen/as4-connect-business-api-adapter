namespace Schleupen.AS4.BusinessAdapter.FP;

public class FpParsedPayload(
    EIC sender,
    EIC receiver,
    string creationDate,
    string validationDate
)
{
    public EIC Sender { get; } = sender;

    public EIC Receiver { get; } = receiver;
    
    public string CreationDate { get; } = creationDate;
    
    public string ValidityDate { get; } = validationDate;
}