// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System;
	using System.IdentityModel.Tokens.Jwt;
	using Microsoft.IdentityModel.Tokens;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Receiving;

	/// <summary>
	/// Helper for the creation of JWT.
	/// </summary>
	public sealed class JwtHelper : IJwtHelper
	{
		private readonly IMarketpartnerCertificateProvider marketpartnerCertificateProvider;

		public JwtHelper(IMarketpartnerCertificateProvider marketpartnerCertificateProvider)
		{
			this.marketpartnerCertificateProvider = marketpartnerCertificateProvider;
		}

		/// <summary>
		/// Creates a signed JWT.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The signed token for the given message.</returns>
		public string CreateSignedToken(InboxMessage message)
		{
			// To validate the authenticy and integrity of the received EDIFACT file in AS4 connect a JWT according to RFC7519 is used. The payload of the JWT has to contain the SHA256 hash value of the EDIFACT file according to RFC6234.
			// Signing is done using the private key of the client certificate which is also used to authenticate against AS4 Connect. The token is not encrypted.

			// The token has to at least contain the following claims:

			//// HEADER:
			//{
			//    "alg": "ES384", // the used signature algorith (always ES384 - ECDSA384) , (SECP384R1) ??? ECCurve.NamedCurves.nistP384
			//    "typ": "JWT" // the type of the token (always JWT)
			//}

			//// PAYLOAD:
			//{
			//    "hash": "07d8d11084e8d500852664c0f64ade1299d418cbe489edefcd422ad698666b33",  // the SHA256 hash value of the payload, as a hexadecimal string
			//    "iss": "9904843000000@BDEW", // the issuer of the token (party identification number of the sender, "MP-ID"@"Typ", analogous to the "OU" field in the certificate)
			//    "aud": "schleupen", // the receiver of the token (always "schleupen") 
			//    "mid": "f613cfa2-a7a2-446f-8599-ce2c9525bbb1", // the message identification 
			//    "iat": 1516239022 // timestamp of the creation of the token
			//}

			//// SIGNATURE:
			//{
			//    ...
			//}

			IAs4Certificate as4Certificate = marketpartnerCertificateProvider.GetMarketpartnerCertificate(message.Receiver.Id);
			SecurityKey securityKey = as4Certificate.GetPrivateSecurityKey();
			SigningCredentials jwtCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha384);
			string certBase64 = Convert.ToBase64String(as4Certificate.GetRawCertData());

			JwtHeader header = new JwtHeader(jwtCredentials);
			JwtPayload payload = new JwtPayload
			{
				{ "hash", message.ContentHashSha256 },
				{ "iss", $"{message.Receiver.Id}@{message.Receiver.Type}" },
				{ "aud", "schleupen" },
				{ "mid", message.MessageId },
				{ "iat", DateTimeOffset.Now.ToUnixTimeSeconds() },
				{ "cert", certBase64 },
			};

			JwtSecurityToken secToken = new JwtSecurityToken(header, payload);
			string tokenString = new JwtSecurityTokenHandler().WriteToken(secToken);

			return tokenString;
		}
	}
}