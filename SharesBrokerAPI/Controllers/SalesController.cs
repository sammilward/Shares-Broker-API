using CurrencyConverterService;
using Microsoft.AspNetCore.Mvc;
using SharesBrokerAPI;
using SharesBrokerAPI.Contracts.V1;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Contracts.V1.Responces;
using SharesBrokerAPI.DatabaseAccess;
using SharesBrokerAPI.Domain.HTTP;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharesBroker.Controllers
{
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IShareRepository _shareRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserShareRepository _userShareRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly RateConverter _rateConverter;
        private readonly SharesUpdateHandler _sharesUpdateHandler;

        public SalesController(IUserRepository userRepository, IShareRepository shareRepository, ISaleRepository saleRepository, IUserShareRepository userShareRepository, IHTTPClientFactory hTTPClientFactory)
        {
            _shareRepository = shareRepository;
            _userRepository = userRepository;
            _userShareRepository = userShareRepository;
            _saleRepository = saleRepository;
            _rateConverter = new RateConverter(new CurrencyConversionWSClient());
            _sharesUpdateHandler = new SharesUpdateHandler(shareRepository, hTTPClientFactory);
        }

        [HttpPut(Routes.SalesRoutes.Create)]
        public async System.Threading.Tasks.Task<IActionResult> CreateAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromBody] CreateSaleRequest createSaleRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            var user = _userRepository.Get(username);
            if (user.IsAdmin) return Unauthorized();
            if (user == null) return BadRequest($"No user exists with username: {username}");

            var shareToSell = _shareRepository.Get(createSaleRequest.CompanySymbol);
            if (shareToSell == null) return BadRequest($"No share exists with symbol {createSaleRequest.CompanySymbol}");
            await _sharesUpdateHandler.HandleShareUpdateAsync(createSaleRequest.CompanySymbol);
            shareToSell = _shareRepository.Get(createSaleRequest.CompanySymbol);

            var userShare = _userShareRepository.GetWithUsernameAndCompanySymbol(username, createSaleRequest.CompanySymbol);
            if (userShare == null || userShare.Quantity == 0) return BadRequest($"User does not have any shares of: {createSaleRequest.CompanySymbol}");
            if (userShare.Quantity < createSaleRequest.Quantity) return BadRequest($"{username} only has {userShare.Quantity} of {createSaleRequest.CompanySymbol} shares.");

            var sale = new Sale
            {
                Username = user.Username,
                Quantity = createSaleRequest.Quantity,
                CompanySymbol = shareToSell.CompanySymbol,
                TotalValue = shareToSell.Value * createSaleRequest.Quantity
            };

            _saleRepository.Add(sale);
            _userShareRepository.Update(userShare.Id, -sale.Quantity);
            _userRepository.Update(username, new UpdateUserRequest { ChangeWalletValueAmount = sale.TotalValue });

            return Created(Routes.SalesRoutes.Create.Replace("{id}", sale.Id.ToString()), sale);
        }

        [HttpGet(Routes.SalesRoutes.GetAll)]
        public async System.Threading.Tasks.Task<IActionResult> GetAllAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromQuery] SalesFilters salesFilters)
        {
            var userHandleResult = ValidateUser(username, password, out var user);
            if (userHandleResult != null) return userHandleResult;

            IEnumerable<Sale> sales;
            double rate = 1;
            if (!user.PrefferedCurrencyIsDefault())
            {
                rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    if (user.IsAdmin)
                    {
                        sales = _saleRepository.GetAll(salesFilters, rate);
                    }
                    else
                    {
                        sales = _saleRepository.GetAll(username, salesFilters, rate);
                    }
                }
                else
                {
                    if (user.IsAdmin)
                    {
                        sales = _saleRepository.GetAll(salesFilters, 1);
                    }
                    else
                    {
                        sales = _saleRepository.GetAll(username, salesFilters, 1);
                    }
                }
            }
            else
            {
                if (user.IsAdmin)
                {
                    sales = _saleRepository.GetAll(salesFilters, 1);
                }
                else
                {
                    sales = _saleRepository.GetAll(username, salesFilters, 1);
                }
            }

            if (!sales.Any()) return NoContent();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                if (rate != 0)
                {
                    foreach (var sale in sales)
                    {
                        sale.TotalValue *= rate;
                    }
                    response.currency = user.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = sales.OrderByDescending(x => x.SaleTime);
            return Ok(response);
        }

        [HttpGet(Routes.SalesRoutes.Get)]
        public async System.Threading.Tasks.Task<IActionResult> GetAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromRoute] Guid id)
        {
            var userHandleResult = ValidateUser(username, password, out var user);
            if (userHandleResult != null) return userHandleResult;
            var sale = _saleRepository.Get(id);
            if (sale == null) return NotFound();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    sale.TotalValue *= rate;
                    response.currency = user.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = sale;
            return Ok(sale);
        }

        private IActionResult ValidateUser(string username, string password, out User user)
        {
            user = _userRepository.Get(username);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            if (user == null) return BadRequest($"No user exists with username: {username}");
            return null;
        }
    }
}