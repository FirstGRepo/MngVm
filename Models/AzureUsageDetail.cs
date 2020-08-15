using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MngVm.Models
{
    public class AzureUsageDetail
    {
        [JsonProperty("value")]
        public IList<AzureUsageData> value { get; set; }
    }

    public class AzureUsageData
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public Object tags { get; set; }
        public AzureUsageProperties properties { get; set; }

    }

    public class AzureUsageProperties
    {
        public string additionalInfo { get; set; }
        public string billingAccountId { get; set; }
        public string billingAccountName { get; set; }
        public string billingCurrencyCode { get; set; }
        public DateTime billingPeriodEndDate { get; set; }
        public DateTime billingPeriodStartDate { get; set; }
        public string billingProfileId { get; set; }
        public string billingProfileName { get; set; }
        public string chargeType { get; set; }
        public string consumedService { get; set; }
        public decimal costInBillingCurrency { get; set; }
        public decimal costInPricingCurrency { get; set; }
        public string costCenter { get; set; }
        public DateTime date { get; set; }
        public string exchangeRate { get; set; }
        public DateTime exchangeRateDate { get; set; }
        public string instanceName { get; set; }
        public string invoiceId { get; set; }
        public string invoiceSectionId { get; set; }
        public string invoiceSectionName { get; set; }
        public string isAzureCreditEligible { get; set; }
        public string meterCategory { get; set; }
        public string meterId { get; set; }
        public string meterName { get; set; }
        public string meterRegion { get; set; }
        public string meterSubCategory { get; set; }
        public string previousInvoiceId { get; set; }
        public string pricingCurrencyCode { get; set; }
        public string product { get; set; }
        public string productIdentifier { get; set; }
        public string productOrderId { get; set; }
        public string productOrderName { get; set; }
        public string publisherName { get; set; }
        public string publisherType { get; set; }
        public double quantity { get; set; }
        public string resourceGroup { get; set; }
        public string resourceLocation { get; set; }
        public string resourceLocationNormalized { get; set; }
        public string serviceFamily { get; set; }
        public string serviceInfo1 { get; set; }
        public string serviceInfo2 { get; set; }
        public DateTime servicePeriodEndDate { get; set; }
        public DateTime servicePeriodStartDate { get; set; }
        public string subscriptionGuid { get; set; }
        public string subscriptionName { get; set; }
        public string unitOfMeasure { get; set; }
        public decimal unitPrice { get; set; }
        public string customerTenantId { get; set; }
        public string customerName { get; set; }
        public string partnerTenantId { get; set; }
        public string partnerName { get; set; }
        public string resellerMpnId { get; set; }
        public string resellerName { get; set; }
        public string publisherId { get; set; }
        public string reservationId { get; set; }
        public string reservationName { get; set; }
        public string frequency { get; set; }
        public string term { get; set; }
        public decimal payGPrice { get; set; }
        public decimal costInUSD { get; set; }
        public decimal paygCostInBillingCurrency { get; set; }
        public decimal paygCostInUSD { get; set; }
        public decimal exchangeRatePricingToBilling { get; set; }
        public decimal partnerEarnedCreditRate { get; set; }
        public string partnerEarnedCreditApplied { get; set; }
        public string pricingModel { get; set; }
        public decimal effectivePrice { get; set; }
    }

    public class AzureUsageApiRequest
    {

        public string timeframe { get; set; }
        public string type { get; set; }
        public Dataset dataset { get; set; }

    }


    public class Dimensions
    {
        public string name { get; set; }
        public string Operator { get; set; }
        public List<string> values { get; set; }
    }

    public class And
    {
        public Dimensions dimensions { get; set; }
    }

    public class Filter
    {
        public List<And> and { get; set; }
    }

    public class TotalCost
    {
        public string name { get; set; }
        public string function { get; set; }
    }

    public class Aggregation
    {
        public TotalCost totalCost { get; set; }
    }

    public class Grouping
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Dataset
    {
        public string granularity { get; set; }
        public Filter filter { get; set; }
        public Aggregation aggregation { get; set; }
        public List<Grouping> grouping { get; set; }
    }


    //============================================================================= 

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);  
    public class Column
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Properties
    {
        public object nextLink { get; set; }
        public List<Column> columns { get; set; }
        public List<List<object>> rows { get; set; }
    }

    public class AzureUsageApiResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public object location { get; set; }
        public object sku { get; set; }
        public object eTag { get; set; }
        public Properties properties { get; set; }
    }



}