using Newtonsoft.Json.Linq;
using Sagitec.ExceptionPub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.BusinessTier
{
    [Serializable]
    public class OIDCHelper
    {
        /// <summary>
        /// Returns all the tokens, ID Token, Refresh Token and Access Token.
        /// </summary>
        /// <param name="astrAuthCode"></param>
        /// <param name="astrTokenEndpoint"></param>
        /// <param name="astrClientId"></param>
        /// <param name="astrClientSecret"></param>
        /// <param name="astrRedirectUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetTokens(string astrAuthCode, string astrTokenEndpoint, string astrClientId, string astrClientSecret, string astrRedirectUrl)
        {
            var values = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", astrClientId},
                { "client_secret", astrClientSecret },
                { "code" , astrAuthCode },
                { "redirect_uri", astrRedirectUrl}
            };

            HttpClient tokenClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var content = new FormUrlEncodedContent(values);
            var response = tokenClient.PostAsync(astrTokenEndpoint, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                return responseContent.ReadAsStringAsync().Result;
            }

            throw new Exception("Token request failed with status code: " + response.StatusCode);
        }


        public String SafeDecodeBase64(string str)
        {
            return System.Text.Encoding.UTF8.GetString(
                getPaddedBase64String(str));
        }

        private byte[] getPaddedBase64String(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/").Replace("-", "+");
            return Convert.FromBase64String(base64);
        }

        /// <summary>
        /// Gets all the tokens
        /// Validates the jwt
        /// return the user id
        /// </summary>
        /// <param name="astrAuthCode"></param>
        /// <param name="astrTokenEndpoint"></param>
        /// <param name="astrClientId"></param>
        /// <param name="astrClientSecret"></param>
        /// <param name="astrRedirectUrl"></param>
        /// <param name="astrJwksUri"></param>
        /// <param name="astrIssuer"></param>
        /// <param name="astrRefreshToken"></param>
        /// <returns></returns>
        public Dictionary<string, string> ValidateJwtAndGetUniqueId(string astrAuthCode, string astrTokenEndpoint, string astrClientId, string astrClientSecret, string astrRedirectUrl, string astrJwksUri, string astrIssuer, out string astrIdToken)
        {
            try
            {
                Dictionary<string, string> ldictUserInfo = new Dictionary<string, string>();
                string lstrJson = GetTokens(astrAuthCode, astrTokenEndpoint, astrClientId, astrClientSecret, astrRedirectUrl);
                JObject jsonObj = JObject.Parse(lstrJson);
                string lstrjwt = jsonObj.GetValue("id_token").ToString();
                string[] jwtParts = lstrjwt.Split('.');
                string decodedHeader = SafeDecodeBase64(jwtParts[0]);
                string decodedPayload = SafeDecodeBase64(jwtParts[1]);
                JObject id_token_obj = new JObject { { "decoded_header", decodedHeader }, { "decoded_payload", decodedPayload } };
                string keyId = JObject.Parse(decodedHeader).GetValue("kid").ToString();
                JToken keyFound = FetchKeys(keyId, astrJwksUri);
                if (keyFound == null)
                {
                    throw new Exception("Key not found in JWKS endpoint or Application State");
                }
                if (!JObject.Parse(decodedPayload).GetValue("iss").ToString().Equals(astrIssuer))
                {
                    throw new Exception("Issuer not validated");
                }
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(
                  new RSAParameters()
                  {
                      Modulus = getPaddedBase64String(keyFound["n"].ToString()),
                      Exponent = getPaddedBase64String(keyFound["e"].ToString())
                  });

                SHA256 sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(jwtParts[0] + '.' + jwtParts[1]));

                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
                rsaDeformatter.SetHashAlgorithm("SHA256");
                if (rsaDeformatter.VerifySignature(hash, getPaddedBase64String(jwtParts[2])))
                {
                    // jwt validated.
                    // returning the unique user id.
                    astrIdToken = lstrjwt;
                    ldictUserInfo.Add("username", Convert.ToString(JObject.Parse(decodedPayload).GetValue("username")));
                    ldictUserInfo.Add("email", Convert.ToString(JObject.Parse(decodedPayload).GetValue("email")));
                    ldictUserInfo.Add("given_name", Convert.ToString(JObject.Parse(decodedPayload).GetValue("given_name")));
                    ldictUserInfo.Add("family_name", Convert.ToString(JObject.Parse(decodedPayload).GetValue("family_name")));
                    return ldictUserInfo;
                }
                else
                {
                    throw new Exception("Could not validate signature of JWT");
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                astrIdToken = null;
                return null;
            }
        }

        /// <summary>
        /// Get the Key for Jwt Validation.
        /// </summary>
        /// <param name="lstrkeyId"></param>
        /// <param name="lstrJwksUri"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public JToken FetchKeys(string lstrkeyId, string lstrJwksUri)
        {
            var jwksclient = new HttpClient();
            jwksclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = jwksclient.GetAsync(lstrJwksUri).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;
                string responseString = responseContent.ReadAsStringAsync().Result;

                foreach (JToken key in JObject.Parse(responseString).GetValue("keys").ToArray())
                {
                    if (key["kid"].ToString().Equals(lstrkeyId))
                    {
                        return key;
                    }
                }

                throw new Exception("Key not found in JWKS endpoint");
            }

            throw new Exception("Could not contact JWKS endpoint");
        }
    }
}
