﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using System.Threading.Tasks;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.FP.Sending;

	public interface IBusinessApiGateway : IDisposable
	{
		Task<BusinessApiResponse<FpOutboxMessage>> SendMessageAsync(FpOutboxMessage message, CancellationToken cancellationToken);
	}
}