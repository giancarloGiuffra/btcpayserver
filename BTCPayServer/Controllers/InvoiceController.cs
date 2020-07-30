using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BTCPayServer.Client.Models;
using BTCPayServer.Data;
using BTCPayServer.HostedServices;
using BTCPayServer.Logging;
using BTCPayServer.Models;
using BTCPayServer.Payments;
using BTCPayServer.Rating;
using BTCPayServer.Security;
using BTCPayServer.Services.Invoices;
using BTCPayServer.Services.Rates;
using BTCPayServer.Services.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NBitpayClient;
using Newtonsoft.Json;
using CreateInvoiceRequest = BTCPayServer.Models.CreateInvoiceRequest;
using StoreData = BTCPayServer.Data.StoreData;

namespace BTCPayServer.Controllers
{
    [Filters.BitpayAPIConstraint(false)]
    public partial class InvoiceController : Controller
    {
        readonly InvoiceRepository _InvoiceRepository;
        readonly ContentSecurityPolicies _CSP;
        readonly RateFetcher _RateProvider;
        readonly StoreRepository _StoreRepository;
        readonly UserManager<ApplicationUser> _UserManager;
        private readonly CurrencyNameTable _CurrencyNameTable;
        readonly EventAggregator _EventAggregator;
        readonly BTCPayNetworkProvider _NetworkProvider;
        private readonly PaymentMethodHandlerDictionary _paymentMethodHandlerDictionary;
        private readonly ApplicationDbContextFactory _dbContextFactory;
        private readonly PullPaymentHostedService _paymentHostedService;
        readonly IServiceProvider _ServiceProvider;
        private readonly InvoiceService _InvoiceService;

        public InvoiceController(
            IServiceProvider serviceProvider,
            InvoiceRepository invoiceRepository,
            CurrencyNameTable currencyNameTable,
            UserManager<ApplicationUser> userManager,
            RateFetcher rateProvider,
            StoreRepository storeRepository,
            EventAggregator eventAggregator,
            ContentSecurityPolicies csp,
            BTCPayNetworkProvider networkProvider,
            PaymentMethodHandlerDictionary paymentMethodHandlerDictionary,
            ApplicationDbContextFactory dbContextFactory,
            PullPaymentHostedService paymentHostedService,
            InvoiceService invoiceService)
        {
            _ServiceProvider = serviceProvider;
            _CurrencyNameTable = currencyNameTable ?? throw new ArgumentNullException(nameof(currencyNameTable));
            _StoreRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
            _InvoiceRepository = invoiceRepository ?? throw new ArgumentNullException(nameof(invoiceRepository));
            _RateProvider = rateProvider ?? throw new ArgumentNullException(nameof(rateProvider));
            _UserManager = userManager;
            _EventAggregator = eventAggregator;
            _NetworkProvider = networkProvider;
            _paymentMethodHandlerDictionary = paymentMethodHandlerDictionary;
            _dbContextFactory = dbContextFactory;
            _paymentHostedService = paymentHostedService;
            _CSP = csp;
            _InvoiceService = invoiceService;
        }

        internal async Task<DataWrapper<InvoiceResponse>> CreateInvoiceCore(CreateInvoiceRequest invoice, StoreData store, string serverUrl, List<string> additionalTags = null, CancellationToken cancellationToken = default)
        {
            return await _InvoiceService.CreateInvoiceCore(invoice, store, serverUrl, additionalTags, cancellationToken);
        }
    }
}
