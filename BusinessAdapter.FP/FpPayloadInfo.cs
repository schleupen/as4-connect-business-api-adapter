namespace Schleupen.AS4.BusinessAdapter.FP;

public record FpPayloadInfo(EIC Sender, EIC Receiver, string CreationDate, string ValidityDate, string FahrplanHaendlerTyp = "TODO") // TODO
{
}