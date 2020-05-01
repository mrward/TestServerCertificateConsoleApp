//
// Program.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2020 Microsoft Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace TestServerCertificateConsoleApp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			try {
				if (args.Length == 0) {
					Console.WriteLine ("Missing url argument.");
					Console.WriteLine ("Usage:");
					Console.WriteLine ("    TestServerCertificateConsoleApp https://expired.badssl.com");
					return;
				}
				Run (args[0]);
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}

		static void Run (string url)
		{
			ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				if (sslPolicyErrors == SslPolicyErrors.None)
					return true;

				if (sender is WebRequest webRequest) {
					sender = webRequest.RequestUri.Host;
					Console.WriteLine ("SslPolicyErrors: {0}", sslPolicyErrors);
				}
				return GetIsCertificateTrusted (sender as string, certificate.GetPublicKeyString ());
			};

			var request = WebRequest.CreateHttp(url);
			var response = request.GetResponse();
		}

		static bool GetIsCertificateTrusted (string uri, string certificate)
		{
			Console.WriteLine ("Untrusted certificate for '{0}'", uri);
			return false;
		}
	}
}
