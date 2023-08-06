using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGeneric
{
    public class Response
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        public class AccountingRate
        {
            public string encodedKey { get; set; }
            public string userKey { get; set; }
            public string fromCurrencyCode { get; set; }
            public string toCurrencyCode { get; set; }
            public string rate { get; set; }
            public DateTime startDate { get; set; }
        }

        public class Currency
        {
            public string code { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public int digitsAfterDecimal { get; set; }
            public string currencySymbolPosition { get; set; }
            public bool isBaseCurrency { get; set; }
            public string type { get; set; }
            public DateTime creationDate { get; set; }
            public DateTime lastModifiedDate { get; set; }
        }

        public class GlAccount
        {
            public string encodedKey { get; set; }
            public DateTime creationDate { get; set; }
            public DateTime lastModifiedDate { get; set; }
            public string glCode { get; set; }
            public string type { get; set; }
            public string usage { get; set; }
            public string name { get; set; }
            public bool activated { get; set; }
            public string description { get; set; }
            public bool allowManualJournalEntries { get; set; }
            public bool stripTrailingZeros { get; set; }
            public Currency currency { get; set; }
        }

        public class GlJournalEntryForeignAmount
        {
            public string encodedKey { get; set; }
            public string amount { get; set; }
            public Currency currency { get; set; }
            public AccountingRate accountingRate { get; set; }
        }

        public class Root
        {
            public string encodedKey { get; set; }
            public int entryID { get; set; }
            public DateTime creationDate { get; set; }
            public DateTime entryDate { get; set; }
            public string bookingDate { get; set; }
            public string transactionID { get; set; }
            public string accountKey { get; set; }
            public string productKey { get; set; }
            public string productType { get; set; }
            public string amount { get; set; }
            public GlAccount glAccount { get; set; }
            public string type { get; set; }
            public string assignedBranchKey { get; set; }
            public GlJournalEntryForeignAmount glJournalEntryForeignAmount { get; set; }
            public string userKey { get; set; }
        }

        public class Test
        {
            public List<Root> Words { get; set; }
        }

    }
}
