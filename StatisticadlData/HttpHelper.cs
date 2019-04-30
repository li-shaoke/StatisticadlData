using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace StatisticadlData
{
	public class HttpHelper
	{
		public static string PostHttps(string url, string bodyContent)
		{
			ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.ProtocolVersion = HttpVersion.Version10;
			request.Method = "POST";
			request.ContentType = "application/json";

			//将json写入request的body中
			//不需要转成字节数组
			var streamWriter = new StreamWriter(request.GetRequestStream());
			streamWriter.Write(bodyContent);
			streamWriter.Flush();
			streamWriter.Close();

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			string encoding = response.ContentEncoding;
			if (encoding == null || encoding.Length < 1) encoding = "UTF-8";
			StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
			
			string result=reader.ReadToEnd();
			if (reader != null) reader.Close();
			if (response != null) response.Close();
			return result;
		}
		public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{ 
			return true; //总是接受
		}
	}
}
