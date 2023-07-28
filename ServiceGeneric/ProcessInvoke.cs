using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using ServiceGeneric.NAVWEBREF;
using ServiceGeneric.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static ServiceGeneric.Response;

namespace ServiceGeneric
{
    internal class ProcessInvoke
    {
        public static Mambu service = new Mambu();
        
        public static string ExecuteFunction()
        {
            service = new Mambu();
            service.Url = "http://40.85.80.247:6027/Y9/WS/Y9%20Microfinance/Codeunit/Mambu";
            service.UseDefaultCredentials = false;
            service.PreAuthenticate = true;
            service.Credentials = new NetworkCredential(Settings.Default.Navuser, Settings.Default.Navpass);

            Task<string> result = null;
            try
            {
                result = InvokeProcess();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error occured ==>" + ex.Message.ToString());
            }

            return result.Result.ToString();
        }
        public static async Task<string> InvokeProcess()
        {
            string result = string.Empty;
            string responseContent = string.Empty;
            var userName = Settings.Default.Uname;
            var userPassword = Settings.Default.pass;

            var authenticationString = $"{userName}:{userPassword}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            DateTime date = DateTime.Now;
            var dateTo = new DateTime(date.Year, date.Month, date.Day);
            var dateFrom = dateTo.AddDays(-1);

            for (int offset = 0; offset < 50000; offset += 1000)
            {
                var builder = new UriBuilder("http://y9bank.mambu.com/api/gljournalentries");
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["offset"] = offset.ToString();
                query["Limit"] = "50";
                query["to"] = dateTo.ToString("yyyy-MM-dd");
                query["from"] = dateFrom.ToString("yyyy-MM-dd");
                builder.Query = query.ToString();
                string url = builder.ToString();

                try
                {

                    using (HttpClient client = new HttpClient())
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var request = new HttpRequestMessage(HttpMethod.Get, url);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
                        request.Headers.Add("User-Agent", "my-user-agent");
                        request.Headers.Add("Consumer", "dynamicsUser");
                        request.Headers.Add("Secret-key", "vxBq5FgR1ky22IIky3e3YdzDIZzmB4S9");
                        request.Headers.Add("apiKey", "jYQItMR8vsB73onUmQf2KXt54004Pdnw");
                        var response = await client.SendAsync(request);
                        responseContent = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            int lineNo = 1000;
                            List<Root> glEntries = JsonConvert.DeserializeObject<List<Root>>(responseContent);
                            for (int i = 0; i < glEntries.Count; i++)
                            {
                                GlEntries entries = new GlEntries();
                                entries.entryID = glEntries[i].entryID;
                                entries.amount = glEntries[i].glJournalEntryForeignAmount.amount;
                                entries.description = glEntries[i].productType;
                                entries.transactionID = glEntries[i].transactionID;
                                entries.type = glEntries[i].type;
                                entries.glCode = glEntries[i].glAccount.glCode;
                                entries.accountNo = glEntries[i].accountKey;
                                entries.postingDate = glEntries[i].bookingDate;
                                entries.documentDate = glEntries[i].bookingDate;
                                entries.currencyCode = glEntries[i].glAccount.currency.code;
                                if (i > 0)
                                    lineNo = lineNo + 1000;
                                service.RunMambuAPI(entries.transactionID, Decimal.Parse(entries.amount), entries.postingDate, int.Parse(entries.transactionID), entries.accountNo, entries.description, entries.type, entries.entryID.ToString(), lineNo, entries.glCode, entries.currencyCode);
                                Console.WriteLine("Processed transaction of entry no " + lineNo);
                            }
                            result = "Completed";
                            Console.WriteLine("Finished processing GL records for date : " + DateTime.Today);
                        }
                        else
                        {
                            ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                            if (errorResponse.errorSource != null)
                            {
                                Console.WriteLine(" Error occured at ===> " + "\"error code =>" + errorResponse.returnCode + ", error source=> " + errorResponse.errorSource);
                                UTILITIES.WriteLog("\" Error occured \" ===> " + "\"error code =>" + errorResponse.returnCode + ", error source=> " + errorResponse.errorSource);

                                result = errorResponse.errorSource;
                            }
                            else
                            {
                                Console.WriteLine(" Error occured at ===> " + "\"error code =>" + errorResponse.returnCode + ", error source=> " + errorResponse.returnStatus);
                                UTILITIES.WriteLog("\" Error occured \" ===> " + "\"error code =>" + errorResponse.returnCode + ", error source=> " + errorResponse.returnStatus);

                                result = errorResponse.returnStatus;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            return result;

        }

        public List<string> GetAllFiles(string sDirt)
        {
            List<string> allFiles = new List<string>();
            try
            {
                foreach (string file in Directory.GetFiles(sDirt))
                    allFiles.Add(file);
                foreach (string directory in Directory.GetDirectories(sDirt))
                    allFiles.AddRange((IEnumerable<string>)this.GetAllFiles(directory));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR====>" + ex.Message);
            }
            return allFiles;
        }
    }
}
