﻿//
// Copyright © 2003-2020 https://github.com/vincoss/Gnu.Licensing
//
// Author:
//  Ferdinand Lukasak
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Gnu.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;


namespace Gnu.Licensing
{
    public abstract class BaseLicenseService
    {
        private readonly HttpClient _httpClient;

        public BaseLicenseService(HttpClient httpClient)
        {
            if(httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            _httpClient = httpClient; ;
        }

        public async Task<LicenseResult> RegisterAsync(Guid licenseKey, Guid productId, IDictionary<string, string> attributes)
        {
            if (licenseKey == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(licenseKey));
            }
            if (productId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            if (attributes == null)
            {
                attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            LicenseRegisterResult result = null;

            try
            {
                var url = GetLicenseServerUrl();
                result = await RegisterHttpAsync(licenseKey, productId, attributes, url);
            }
            catch (Exception ex)
            {
                var failure = FailureStrings.Get(FailureStrings.ACT00Code);
                return new LicenseResult(null, ex, new[] { failure });
                // TODO: log
            }

            if (string.IsNullOrWhiteSpace(result.License))
            {
                return new LicenseResult(null, null, new[] { result.Failure });
            }

            try
            {
                using (var sw = LicenseOpenWrite())
                {
                    var element = XElement.Parse(result.License);
                    element.Save(sw);
                }
            }
            catch (Exception ex)
            {
                var failure = FailureStrings.Get(FailureStrings.VAL01Code);
                return new LicenseResult(null, ex, new[] { failure });
                // TODO: log
            }

            var validationResult = await ValidateAsync();
            if (validationResult.Failures.Any())
            {
                return new LicenseResult(null, null, validationResult.Failures);
            }

            return new LicenseResult(validationResult.License, null, null);
        }

        public async Task<bool> CheckAsync(Guid activationUuid)
        {
            if (activationUuid == Guid.Empty) throw new ArgumentNullException(nameof(activationUuid));

            /*
                NOTE: Must return true if http request is not successfull. 
            */

            var result = true;

            try
            {
                var data = new
                {
                    Id = activationUuid
                };

                var url = $"{GetLicenseServerUrl()}/check";
                var json = JsonSerializer.Serialize(data);
                result = await PostHttpAsync<bool>(url, json);

            }
            catch (Exception ex)
            {
                // TODO: Logging
            }
            return result;
        }

        public Task<LicenseResult> ValidateAsync(bool onlineCheck = false)
        {
            var task = Task.Run(async() =>
            {
                var results = new List<IValidationFailure>();
                License actual = null;

                try
                {
                    using (var stream = LicenseOpenRead())
                    {
                        if (stream == null)
                        {
                            var nf = FailureStrings.Get(FailureStrings.VAL00Code);
                            return new LicenseResult(null, null, new[] { nf });
                        }

                        actual = License.Load(stream);
                    }

                    var failures = await ValidateInternalAsync(actual);

                    foreach (var f in failures)
                    {
                        results.Add(f);
                    }

                    if(results.Any() == false && onlineCheck && IsConnected())
                    {
                        var activationActiveResult = await CheckAsync(actual.ActivationUuid);
                        if(activationActiveResult == false)
                        {
                            var vf = FailureStrings.Get(FailureStrings.VAL06Code);
                            results.Add(vf);
                        }
                    }

                    return new LicenseResult(results.Any() ? null : actual, null, results);
                }
                catch (Exception ex)
                {
                    // TODO: log

                    var failure = FailureStrings.Get(FailureStrings.VAL01Code);

                    results.Add(failure);
                    return new LicenseResult(null, ex, results);
                }
            });
            return task;
        }

        #region Abstract methods

        protected abstract Task<IEnumerable<IValidationFailure>> ValidateInternalAsync(License actual);

        protected abstract Stream LicenseOpenRead();

        protected abstract Stream LicenseOpenWrite();

        protected abstract bool IsConnected();

        public abstract string GetLicenseServerUrl();

        #endregion

        #region Http methods

        public async Task<LicenseRegisterResult> RegisterHttpAsync(Guid licenseKey, Guid productId, IDictionary<string, string> attributes, string serverUrl)
        {
            if (licenseKey == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(licenseKey));
            }
            if (productId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(productId));
            }
            if (attributes == null)
            {
                throw new ArgumentNullException(nameof(attributes));
            }
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new ArgumentNullException(nameof(serverUrl));
            }

            // TODO: Must pass model insted of dynamic type here

            var data = new
            {
                LicenseUuid = licenseKey,
                ProductUuid = productId,
                Attributes = attributes
            };

            var json = JsonSerializer.Serialize(data);
            var result = await PostHttpAsync<LicenseRegisterResult>(serverUrl, json);
            return result;
        }

        public async Task<TResult> PostHttpAsync<TResult>(string uri, string data)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            var content = new StringContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(uri, content);

            await HandleResponse(response);
            string serialized = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            TResult result = await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized, options));
            return result;
        }

        public static HttpClient CreateHttpClient()
        {
#if DEBUG   // TODO:
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            HttpClient dclient = new HttpClient(clientHandler);
            return dclient;
#endif
            HttpClient client = new HttpClient();
            return client;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(content);
            }
        }

        #endregion
    }
}
