using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KZTEK.Invoice.SDKs.Abstractions;
using KZTEK.Invoice.SDKs.Constants;
using KZTEK.Invoice.SDKs.Enums;
using KZTEK.Invoice.SDKs.Extensions;
using KZTEK.Invoice.SDKs.Models;
using KZTEK.Invoice.SDKs.Models.Abstractions;
using Newtonsoft.Json;

namespace KZTEK.Invoice.SDKs.Internals;

public class VnptInvoiceService(IHttpClientFactory httpClientFactory) : IInvoiceService
{
    public async Task CreateAsync(CreateInvoice @base, BaseInvoice entity)
    {
        var command = @base as CreateVnptInvoice ?? throw new ArgumentException("Invalid command type");

        if (string.IsNullOrEmpty(command.LookupCode))
        {
            throw new ArgumentException("LookupCode must be provided from backend");
        }

        if (string.IsNullOrEmpty(command.XmlInvData))
        {
            command.XmlInvData = BuildInvoiceXmlData(command, command.LookupCode);
        }

        var request = BuildCreateHttpContent(command);
        Console.WriteLine($"VNPT Request: {request}");

        var requestContent = CreateHttpRequestContent(request);

        var httpClient = httpClientFactory.CreateClient(HttpClients.VNPT);
        var httpRequest = await httpClient.PostAsync(Endpoints.Vnpt.IMPORT_AND_PUBLISH, requestContent);

        if (httpRequest.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Request failed: {httpRequest.StatusCode}");
        }

        var httpResponse = await httpRequest.Content.ReadAsStringAsync();
        Console.WriteLine($"VNPT Response: {httpResponse}");

        var actualResult = ExtractSoapResult(httpResponse);
        var responseData = ParseCreateResponse(actualResult);

        if (!responseData.Success)
        {
            throw new Exception($"Invoice creation failed: {actualResult}");
        }

        entity.Amount = command.Amount;
        entity.TaxRate = command.TaxRate;
        entity.TaxAmount = command.TaxAmount;
        entity.TotalAmount = command.TotalAmount;
        entity.Status = InvoiceStatus.SENT;
    }

    public async Task<string> DownloadAsync(DownloadInvoice @base)
    {
        var command = @base as DownloadVnptInvoice ?? throw new ArgumentException("Invalid command type");

        var request = BuildDownloadHttpContent(command);
        var requestContent = CreateHttpRequestContent(request);

        var httpClient = httpClientFactory.CreateClient(HttpClients.VNPT);
        var httpRequest = await httpClient.PostAsync(Endpoints.Vnpt.DOWNLOAD_PDF, requestContent);

        if (httpRequest.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"Request failed: {httpRequest.StatusCode}");
        }

        return await httpRequest.Content.ReadAsStringAsync();
    }

    private static string ExtractSoapResult(string soapResponse)
    {
        try
        {
            var doc = XDocument.Parse(soapResponse);
            var result = doc.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "ImportAndPublishInvResult")?.Value;

            return result ?? soapResponse;
        }
        catch
        {
            return soapResponse;
        }
    }

    private static HttpContent CreateHttpRequestContent(string content)
    {
        // Use UTF-8 encoding explicitly and set proper content type with charset
        var requestContent = new StringContent(content, Encoding.UTF8, "text/xml");
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml")
        {
            CharSet = "utf-8"
        };

        // Add SOAPAction header if required by the service
        requestContent.Headers.Add("SOAPAction", "http://tempuri.org/ImportAndPublishInv");

        return requestContent;
    }

    private static string BuildCreateHttpContent(CreateVnptInvoice command)
    {
        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
        XNamespace ns = "http://tempuri.org/";

        var soapEnvelope = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                new XAttribute(XNamespace.Xmlns + "soap", soap),
                new XElement(soap + "Body",
                    new XElement(ns + "ImportAndPublishInv",
                        new XElement(ns + "Account", command.Account),
                        new XElement(ns + "ACpass", command.ACpass),
                        new XElement(ns + "xmlInvData", new XCData(command.XmlInvData)),
                        new XElement(ns + "username", command.Username),
                        new XElement(ns + "password", command.Password),
                        new XElement(ns + "pattern", command.Pattern ?? ""),
                        new XElement(ns + "serial", command.Serial ?? ""),
                        new XElement(ns + "convert", command.Convert)
                    )
                )
            )
        );

        // Ensure the XML is saved with UTF-8 encoding
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        soapEnvelope.Save(xmlWriter);
        return stringWriter.ToString();
    }

    private static string BuildDownloadHttpContent(DownloadVnptInvoice command)
    {
        var soapEnvelope = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(XName.Get("Envelope", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                new XAttribute(XNamespace.Xmlns + "soap", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XElement(XName.Get("Body", "http://schemas.xmlsoap.org/soap/envelope/"),
                    new XElement(XName.Get("downloadNewInvPDFFkey", "http://tempuri.org/"),
                        new XElement("userName", command.Username),
                        new XElement("userPass", command.Password),
                        new XElement("fkey", command.LookupCode)
                    )
                )
            )
        );

        // Ensure the XML is saved with UTF-8 encoding
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        soapEnvelope.Save(xmlWriter);
        return stringWriter.ToString();
    }

    private static string BuildInvoiceXmlData(CreateVnptInvoice command, string lookupCode)
    {
        var invoiceDate = DateTime.UtcNow.ToTimeZone("Asia/Ho_Chi_Minh");

        var xml = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Invoices",
                new XElement("Inv",
                    new XElement("key", lookupCode),
                    new XElement("Invoice",
                        new XElement("CusCode", "KL"),
                        new XElement("CusName", "Khách lẻ"),
                        new XElement("CusAddress", ""),
                        new XElement("CusPhone", ""),
                        new XElement("CusTaxCode", ""),
                        new XElement("CCCDan", ""),
                        new XElement("PaymentMethod", "TM/CK/Cấn công trừ nợ"),
                        new XElement("Buyer", ""),
                        new XElement("CurrencyUnit", "VND"),
                        new XElement("ExchangeRate", "1"),
                        new XElement("ArisingDate", invoiceDate.ToString("dd/MM/yyyy")),
                        new XElement("Extra", command.PlateNumber),
                        new XElement("Extra1", command.EntryUtc.ToTimeZone("Asia/Ho_Chi_Minh").ToString("dd-MM-yyyy HH:mm:ss")),
                        new XElement("Extra2", command.ExitUtc.ToTimeZone("Asia/Ho_Chi_Minh").ToString("dd-MM-yyyy HH:mm:ss")),
                        new XElement("Products",
                            new XElement("Product",
                                new XElement("ProdName", "Phí gửi xe"),
                                new XElement("ProdUnit", "Lượt"),
                                new XElement("ProdQuantity", "1"),
                                new XElement("ProdPrice", command.Amount.ToString("0")),
                                new XElement("Total", command.Amount.ToString("0")),
                                new XElement("VATRate", command.TaxRate.ToString("0")),
                                new XElement("VATAmount", command.TaxAmount.ToString("0")),
                                new XElement("Discount", "0"),
                                new XElement("DiscountAmount", "0"),
                                new XElement("Amount", command.TotalAmount.ToString("0")),
                                new XElement("IsSum", "0")
                            )
                        ),

                        new XElement("Total", command.Amount.ToString("0")),
                        new XElement("VATRate", command.TaxRate.ToString("0")),
                        new XElement("VATAmount", command.TaxAmount.ToString("0")),
                        new XElement("Amount", command.TotalAmount.ToString("0")),
                        new XElement("AmountInWords", SayMoney.MISASaysMoney.MISASayMoney(command.TotalAmount, sCurrencyID: "VND", beforeWordAmount: "")),

                        new XElement("GrossValue", "0"),
                        new XElement("GrossValue0", "0"),
                        new XElement("VatAmount0", "0"),
                        new XElement("GrossValue5", command.TaxRate == 5 ? command.Amount.ToString("0") : "0"),
                        new XElement("VatAmount5", command.TaxRate == 5 ? command.TaxAmount.ToString("0") : "0"),
                        new XElement("GrossValue10", command.TaxRate == 10 ? command.Amount.ToString("0") : "0"),
                        new XElement("VatAmount10", command.TaxRate == 10 ? command.TaxAmount.ToString("0") : "0")
                    )
                )
            )
        );

        // Use proper XML writer settings to ensure UTF-8 encoding
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        xml.Save(xmlWriter);
        return stringWriter.ToString();
    }

    private static VnptCreateResponse ParseCreateResponse(string response)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(response) || !response.StartsWith("OK:"))
            {
                return new VnptCreateResponse { Success = false };
            }

            var result = new VnptCreateResponse { Success = true };

            var parts = response.Substring(3).Split(';');
            if (parts.Length >= 2)
            {
                result.Pattern = parts[0];

                var serialAndInvoices = parts[1].Split('-');
                if (serialAndInvoices.Length >= 2)
                {
                    result.Serial = serialAndInvoices[0];

                    var invoicePairs = serialAndInvoices[1].Split(',');
                    foreach (var pair in invoicePairs)
                    {
                        var keyNumber = pair.Split('_');
                        if (keyNumber.Length == 2)
                        {
                            result.InvoiceKeys.Add(keyNumber[0]);
                            result.InvoiceNumbers.Add(keyNumber[1]);
                        }
                    }
                }
            }

            return result;
        }
        catch
        {
            return new VnptCreateResponse { Success = false };
        }
    }

    internal class VnptCreateResponse
    {
        public bool Success { get; set; }
        public string Pattern { get; set; } = "";
        public string Serial { get; set; } = "";
        public List<string> InvoiceKeys { get; set; } = new();
        public List<string> InvoiceNumbers { get; set; } = new();
    }
}