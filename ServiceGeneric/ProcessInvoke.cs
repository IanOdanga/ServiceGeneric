using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceGeneric.NAVWEBREF;
using ServiceGeneric.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
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
                UTILITIES.WriteLog(" Error occured ==>" + ex.Message.ToString());
            }

            return result.Result.ToString();
        }
        public static async Task<string> InvokeProcess()
        {
            string result = string.Empty;
            string bookingDate = string.Empty;
            string responseContent = string.Empty;
            var userName = Settings.Default.Uname;
            var userPassword = Settings.Default.pass;
            int lineNo = 1000;

            var authenticationString = $"{userName}:{userPassword}";
            var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(authenticationString));

            DateTime date = DateTime.Now;
            var dateTo = new DateTime(date.Year, date.Month, date.Day);
            var dateFrom = dateTo.AddHours(-24);
            dateTo = dateFrom.AddHours(23).AddMinutes(59).AddSeconds(59);

            try
            {
                for (int offset = 1000; offset < 50000; offset += 1000)
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
                            Console.WriteLine("================================");
                            Console.WriteLine("Fatched data from API => Status Code : " + response.StatusCode);
                            UTILITIES.WriteLog("Fatched data from API => Status Code : " + response.StatusCode);

                            if (responseContent != null)
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    List<Root> glEntries = JsonConvert.DeserializeObject<List<Root>>(responseContent);
                                    for (int i = 0; i < glEntries.Count; i++)
                                    {
                                        string[] arr = glEntries[i].bookingDate.Split('T');
                                        bookingDate = arr[0].ToString();

                                        GlEntries entries = new GlEntries();
                                        entries.entryID = glEntries[i].entryID;
                                        entries.amount = glEntries[i].glJournalEntryForeignAmount.amount;
                                        entries.description = glEntries[i].productType;
                                        entries.transactionID = glEntries[i].transactionID;
                                        entries.type = glEntries[i].type;
                                        entries.glCode = glEntries[i].glAccount.glCode;
                                        entries.accountNo = glEntries[i].accountKey;
                                        entries.postingDate = DateTime.Parse(bookingDate);
                                        entries.documentDate = DateTime.Parse(bookingDate);
                                        entries.currencyCode = glEntries[i].glAccount.currency.code;
                                        if (i > 0)
                                            lineNo = lineNo + 1000;
                                        service.RunMambuAPI(entries.transactionID, Decimal.Parse(entries.amount), entries.postingDate, int.Parse(entries.transactionID), entries.accountNo, entries.description, entries.type, entries.entryID.ToString(), lineNo, entries.glCode, entries.currencyCode);
                                        Console.WriteLine("Processed transaction of entry no " + lineNo);
                                        UTILITIES.WriteLog("Processed transaction of entry no " + lineNo);
                                    }
                                    result = "Completed";
                                    Console.WriteLine("Finished processing GL records for date : " + DateTime.Parse(bookingDate));
                                    UTILITIES.WriteLog("Finished processing GL records for date : " + DateTime.Parse(bookingDate));
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
                            else
                            {
                                Console.WriteLine("================================");
                                UTILITIES.WriteLog("================================");
                                Console.WriteLine("Maximum offset reached");
                                UTILITIES.WriteLog("Maximum offset reached");
                                result = "End of loop";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }
            catch(Exception ex) 
            {
                result = ex.Message;
                Console.WriteLine(ex.Message.ToString());
            }
            return result;

        }
    }
}
