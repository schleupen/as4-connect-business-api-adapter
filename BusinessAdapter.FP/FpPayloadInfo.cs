namespace Schleupen.AS4.BusinessAdapter.FP;

public record FpPayloadInfo(EIC Sender, EIC Receiver, DateTime MessageDateTime, string FahrplanHaendlerTyp = "TODO") // TODO
{
}