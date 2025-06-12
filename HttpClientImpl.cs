using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections;
using System.Reflection.PortableExecutable;

namespace CSHCSDKDemo
{
    class Constants
    {
        //HmacSha256
        public const string HMAC_SHA256 = "HmacSHA256";
        //UTF-8
        public const string ENCODING = "UTF-8";
        //UserAgent
        public const string USER_AGENT = "demo/aliyun/net";
        //
        public const string LF = "\n";
        //
        public const string SPE1 = ",";
        //
        public const string SPE2 = ":";

        //
        public const int DEFAULT_TIMEOUT = 1000;

        //
        public const string CA_HEADER_TO_SIGN_PREFIX_SYSTEM = "x-ca-";
    }

    /// <summary>
    /// create Sign in info
    /// </summary>
    class SignUtil
    {
        public static string HmacSHA256(string str, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(str);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        private static string BuildStringToSign(ref Dictionary<string, string> headers, string url, Dictionary<string, string> formParamMap, string method, List<string> signHeaderPrefixList)
        {
            string strToSign = "";

            method = method.ToUpper();
            strToSign += method;
            strToSign += Constants.LF;

            if (headers.ContainsKey(HttpClientImpl.HTTP_HEADER_ACCEPT))
            {
                strToSign += headers[HttpClientImpl.HTTP_HEADER_ACCEPT];
                strToSign += Constants.LF;
            }

            if (headers.ContainsKey(HttpClientImpl.HTTP_HEADER_CONTENT_MD5))
            {
                strToSign += headers[HttpClientImpl.HTTP_HEADER_CONTENT_MD5];
                strToSign += Constants.LF;
            }

            if (headers.ContainsKey(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE))
            {
                strToSign += headers[HttpClientImpl.HTTP_HEADER_CONTENT_TYPE];
                strToSign += Constants.LF;
            }

            if (headers.ContainsKey(HttpClientImpl.HTTP_HEADER_DATE))
            {
                strToSign += headers[HttpClientImpl.HTTP_HEADER_DATE];
                strToSign += Constants.LF;
            }

            strToSign += BuildHeaders(ref headers, signHeaderPrefixList);
            strToSign += BuildResource(url, formParamMap);

            return strToSign;
        }

        public static string BuildHeaders(ref Dictionary<string, string> headers, List<string> signHeaderPrefixList)
        {
            string signHeadersString = "";
            string sb = "";

            headers.Remove(HttpClientImpl.X_CA_SIGNATURE_HEADERS);

            signHeaderPrefixList.Remove(HttpClientImpl.X_CA_SIGNATURE);
            signHeaderPrefixList.Remove(HttpClientImpl.HTTP_HEADER_ACCEPT);
            signHeaderPrefixList.Remove(HttpClientImpl.HTTP_HEADER_CONTENT_MD5);
            signHeaderPrefixList.Remove(HttpClientImpl.HTTP_HEADER_CONTENT_TYPE);
            signHeaderPrefixList.Remove(HttpClientImpl.HTTP_HEADER_DATE);

            foreach (var kvp in headers)
            {
                if (IsHeaderToSign(kvp.Key, signHeaderPrefixList))
                {
                    sb += (kvp.Key);
                    sb += (Constants.SPE2);
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        sb += (kvp.Value);
                    }
                    sb += (Constants.LF);

                    if (!string.IsNullOrEmpty(signHeadersString))
                    {
                        signHeadersString += (",");
                    }
                    signHeadersString += (kvp.Key);
                }
            }

            headers.Add(HttpClientImpl.X_CA_SIGNATURE_HEADERS, signHeadersString);

            return sb;
        }

        public static bool IsHeaderToSign(string headerName, List<string> signHeaderPrefixList)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                return false;
            }

            if (headerName.Contains(Constants.CA_HEADER_TO_SIGN_PREFIX_SYSTEM))
            {
                return true;
            }

            foreach (var stu in signHeaderPrefixList)
            {
                if (headerName.Contains(Constants.CA_HEADER_TO_SIGN_PREFIX_SYSTEM))
                {
                    return true;
                }
            }

            return false;
        }

        public static string BuildResource(string url, Dictionary<string, string> formParamMap)
        {
            //delete http://10.6.131.112:9999
            string[] sArray = Regex.Split(url, "://", RegexOptions.IgnoreCase);
            if (sArray.Length == 2)
            {
                string second_str = sArray[1];
                int index = second_str.IndexOf('/');
                if (index >= 0)
                {
                    url = second_str.Remove(0, index);
                }
            }
            return url;
        }

        public static string Sign(string method, string url, ref Dictionary<string, string> headers, Dictionary<string, string> formParamMap, string secret, List<string> signHeaderPrefixList)
        {
            string stringToSign = BuildStringToSign(ref headers, url, formParamMap, method, signHeaderPrefixList);

            return HmacSHA256(stringToSign, secret);
        }

    }

    /// <summary> 
    /// OpenAPI http client
    /// </summary> 
    public class HttpClientImpl
    {
        //POST method
        public const string HTTP_POST = "POST";

        //PUT method
        public const string HTTP_PUT = "PUT";

        //GET method
        public const string HTTP_GET = "GET";

        //DELETE method
        public const string HTTP_DELETE = "DELETE";

        //Header
        public const string X_CA_SIGNATURE = "x-ca-signature";

        //Headers
        public const string X_CA_SIGNATURE_HEADERS = "x-ca-signature-headers";

        //timestamp
        public const string X_CA_TIMESTAMP = "x-ca-timestamp";

        //GUID
        public const string X_CA_NONCE = "x-ca-nonce";

        //APP KEY
        public const string X_CA_KEY = "x-ca-key";

        //Header Accept
        public const string HTTP_HEADER_ACCEPT = "Accept";

        //Body Content MD5 Header
        public const string HTTP_HEADER_CONTENT_MD5 = "Content-MD5";

        //Header Content-Type
        public const string HTTP_HEADER_CONTENT_TYPE = "ContentType";

        //Header UserAgent
        public const string HTTP_HEADER_USER_AGENT = "User-Agent";

        //Header Date
        public const string HTTP_HEADER_DATE = "Date";

        public HttpClientImpl()
        {
            //spt = SecurityProtocolType.Tls;
            requestEncoding = Encoding.GetEncoding("UTF-8");
            requestUserAgent = DefaultUserAgent;
        }

        //UTF-8 Encoding
        public static Encoding requestEncoding { get; set; }

        //set encode
        public static void SetEncoding(string encode) { requestEncoding = Encoding.GetEncoding(encode); }

        private static string requestUserAgent = DefaultUserAgent;

        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
        
        //set user agent
        public static void SetUserAgent(string userAgent = null) { if (!String.IsNullOrWhiteSpace(userAgent)) requestUserAgent = userAgent; else requestUserAgent = DefaultUserAgent; }

        /// <summary>
        /// create HTTP/HTTPS request
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="body">body</param>
        /// <param name="timeout">request timeout</param>
        /// <param name="userAgent">user agent</param>
        /// <param name="requestEncoding">encode</param>
        /// <param name="headers">http headers</param>
        /// <param name="AllowAutoRedirect">Allow Request Auto Redirect</param>
        /// <returns></returns>
        private static HttpWebResponse CreateRequest(
            string url, 
            string body = null, 
            string method = HTTP_GET, 
            string headers = null, 
            int timeout = 30, 
            Encoding requestEncoding = null, 
            bool AllowAutoRedirect = true)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            if (requestEncoding == null)
                requestEncoding = Encoding.GetEncoding("UTF-8");

            HttpWebRequest request = null;

            //https request need set SecurityProtocol
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = method;
            request.UserAgent = requestUserAgent;
            request.AllowAutoRedirect = AllowAutoRedirect;
            
            request.Timeout = timeout;

            if (!string.IsNullOrWhiteSpace(headers))
            {
                string[] arr1 = headers.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in arr1)
                {
                    string[] arr2 = s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr2[0] == "Referer")
                        request.Referer = arr2[1];
                    else if (arr2[0] == HTTP_HEADER_USER_AGENT)
                        request.UserAgent = arr2[1];
                    else if (arr2[0] == HTTP_HEADER_ACCEPT)
                        request.Accept = arr2[1];
                    else if (arr2[0] == HTTP_HEADER_CONTENT_TYPE)
                        request.ContentType = arr2[1];
                    else if (arr2[0] == "Range")
                    {
                        string[] arr3 = arr2[1].Split('-');
                        long f = long.Parse(arr3[0]);
                        long t = 0;
                        if (arr3.Length > 1)
                            t = long.Parse(arr3[1]);
                        if (t < f)
                            request.AddRange(f, t);
                        else
                            request.AddRange(f);
                    }
                    else
                        request.Headers.Add(arr2[0], arr2[1]);
                }
            }

            if (string.IsNullOrWhiteSpace(request.ContentType))
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            if (!string.IsNullOrWhiteSpace(body))
            {
                byte[] data = requestEncoding.GetBytes(body);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Check Certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //receive all
        }

        /// <summary>
        /// Request GET
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">headers</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="timeout">Timeout/S</param>
        /// <returns>Response</returns>
        /// 
        public static string Get(
            string url, 
            Dictionary<string, string> headers, 
            string appKey, 
            string appSecret, 
            int timeout = 10)
        {
            List<string> signHeaderPrefixList = new List<string>();
            Dictionary<string, string> formParam = null;
            initialBasicHeader(ref headers, appKey, appSecret, HTTP_GET, url, formParam, signHeaderPrefixList);

            string header_string = "";
            foreach (var kvp in headers)
            {
                header_string += (kvp.Key + ":" + kvp.Value + "\n");
            }
            
            string content = "";
            try
            {
                HttpWebResponse rq = CreateRequest(url, null, HTTP_GET, header_string, timeout * 1000);
                Stream rs = rq.GetResponseStream();
                HttpStatusCode http_Code = rq.StatusCode;
                using (StreamReader sr = new StreamReader(rs, Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                content = e.Message;
            }
            return content;
        }

        /// <summary>
        /// Request DELETE
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">headers</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="timeout">Timeout/S</param>
        /// <returns>Response</returns>
        /// 
        public static string Delete(
            string url, 
            Dictionary<string, string> headers, 
            string appKey, 
            string appSecret, 
            int timeout = 10)
        {
            List<string> signHeaderPrefixList = new List<string>();
            Dictionary<string, string> formParam = null;
            initialBasicHeader(ref headers, appKey, appSecret, HTTP_DELETE, url, formParam, signHeaderPrefixList);

            string header_string = "";
            foreach (var kvp in headers)
            {
                header_string += (kvp.Key + ":" + kvp.Value + "\n");
            }

            string content = "";
            try
            {
                HttpWebResponse rq = CreateRequest(url, null, HTTP_DELETE, header_string, timeout * 1000);
                Stream rs = rq.GetResponseStream();
                HttpStatusCode http_Code = rq.StatusCode;
                using (StreamReader sr = new StreamReader(rs, Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                content = e.Message;
            }
            return content;
        }

        /// <summary>
        /// Request POST
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">headers</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="body">body</param>
        /// <param name="timeout">Timeout/S</param>
        /// <returns>Response</returns>
        /// 
        public static string Post(
            string url, 
            Dictionary<string, string> headers, 
            string appKey, 
            string appSecret, 
            string body = null, 
            int timeout = 10)
        {
            List<string> signHeaderPrefixList = new List<string>();
            Dictionary<string, string> formParam = null;
            initialBasicHeader(ref headers, appKey, appSecret, HTTP_POST, url, formParam, signHeaderPrefixList);

            string header_string = "";
            foreach (var kvp in headers)
            {
                header_string += (kvp.Key + ":" + kvp.Value + "\n");
            }

            string content = "";
            try
            {
                HttpWebResponse rq = CreateRequest(url, body, HTTP_POST, header_string, timeout * 1000);
                Stream rs = rq.GetResponseStream();
                HttpStatusCode http_Code = rq.StatusCode;
                using (StreamReader sr = new StreamReader(rs, Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                content = e.Message;
            }

            //headers.Remove(X_CA_KEY);
            //headers.Remove(X_CA_NONCE);
            //headers.Remove(X_CA_TIMESTAMP);
            //headers.Remove(X_CA_SIGNATURE);
            return content;
        }

        /// <summary>
        /// Request PUT
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="headers">headers</param>
        /// <param name="appKey">appKey</param>
        /// <param name="appSecret">appSecret</param>
        /// <param name="body">body</param>
        /// <param name="timeout">Timeout/S</param>
        /// <returns>Response</returns>
        /// 
        public static string Put(
            string url, 
            Dictionary<string, string> headers, 
            string appKey, 
            string appSecret, 
            string body = null, 
            int timeout = 10)
        {
            List<string> signHeaderPrefixList = new List<string>();
            Dictionary<string, string> formParam = null;
            initialBasicHeader(ref headers, appKey, appSecret, HTTP_PUT, url, formParam, signHeaderPrefixList);

            string header_string = "";
            foreach (var kvp in headers)
            {
                header_string += (kvp.Key + ":" + kvp.Value + "\n");
            }

            string content = "";
            try
            {
                HttpWebResponse rq = CreateRequest(url, body, HTTP_PUT, header_string, timeout * 1000);
                Stream rs = rq.GetResponseStream();
                HttpStatusCode http_Code = rq.StatusCode;
                using (StreamReader sr = new StreamReader(rs, Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                content = e.Message;
            }
            return content;
        }

        /// <summary>
        /// create http headers and sign in info
        /// </summary>
        /// <param name="header"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <param name="method"></param>
        /// <param name="requestAddress"></param>
        /// <param name="formParam"></param>
        /// <param name="signHeaderPrefixList"></param>
        private static void initialBasicHeader(
            ref Dictionary<string, string> header, 
            string appKey, 
            string appSecret, 
            string method, 
            string requestAddress, 
            Dictionary<string, string> formParam, 
            List<string> signHeaderPrefixList)
        { 

            //if (!header.ContainsKey("X_CA_KEY"))
            //{
            header.Add(X_CA_KEY, appKey);
            //}
            //if (!header.ContainsKey("X_CA_NONCE"))
            //{
                header.Add(X_CA_NONCE, GetGUIDString());
            //}
            //if (!header.ContainsKey("X_CA_TIMESTAMP"))
            //{
                header.Add(X_CA_TIMESTAMP, GetUTCTimeStamp());
            //}
            //if (!header.ContainsKey("X_CA_SIGNATURE"))
            //{
                header.Add(X_CA_SIGNATURE, SignUtil.Sign(method, requestAddress, ref header, formParam, appSecret, signHeaderPrefixList));
            //}
           
            
            
        }

        /// <summary> 
        /// Get UTC TimeStamp:ms
        /// </summary> 
        /// <returns>UTC</returns> 
        private static string GetUTCTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary> 
        /// get new GUID
        /// </summary> 
        private static string GetGUIDString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
