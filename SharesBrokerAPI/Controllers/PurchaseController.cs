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
using System.Threading.Tasks;

namespace SharesBroker.Controllers
{
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IShareRepository _shareRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserShareRepository _userShareRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly RateConverter _rateConverter;
        private readonly SharesUpdateHandler _sharesUpdateHandler;

        public PurchaseController(IUserRepository userRepository, IShareRepository shareRepository, IPurchaseRepository purchaseRepository, IUserShareRepository userShareRepository, IHTTPClientFactory hTTPClientFactory)
        {
            _shareRepository = shareRepository;
            _userRepository = userRepository;
            _userShareRepository = userShareRepository;
            _purchaseRepository = purchaseRepository;
            _rateConverter = new RateConverter(new CurrencyConversionWSClient());
            _sharesUpdateHandler = new SharesUpdateHandler(shareRepository, hTTPClientFactory);
        }

        [HttpPut(Routes.PurchaseRoutes.Create)]
        public async Task<IActionResult> CreateAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromBody] CreatePurchaseRequest createPurchaseRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            var user = _userRepository.Get(username);
            if (user.IsAdmin) return Unauthorized();
            if (user == null) return BadRequest($"No user exists with username: {username}");

            var shareToBuy = _shareRepository.Get(createPurchaseRequest.CompanySymbol);
            if (shareToBuy == null) return BadRequest($"No share exists with symbol {createPurchaseRequest.CompanySymbol}");
            await _sharesUpdateHandler.HandleShareUpdateAsync(createPurchaseRequest.CompanySymbol);
            shareToBuy = _shareRepository.Get(createPurchaseRequest.CompanySymbol);
            if (shareToBuy.NumberOfShares < createPurchaseRequest.Quantity) return BadRequest($"{createPurchaseRequest.CompanySymbol} only had {shareToBuy.NumberOfShares} remaining");

            var purchase = new Purchase
            {
                Username = user.Username,
                Quantity = createPurchaseRequest.Quantity,
                CompanySymbol = shareToBuy.CompanySymbol,
                TotalValue = shareToBuy.Value * createPurchaseRequest.Quantity
            };

            if (user.WalletValue < purchase.TotalValue) return BadRequest($"User does not have enough credit to purchase");

            _purchaseRepository.Add(purchase);

            var userShare = _userShareRepository.GetWithUsernameAndCompanySymbol(username, createPurchaseRequest.CompanySymbol);
            if (userShare == null)
            {
                userShare = new UserShare()
                {
                    Quantity = purchase.Quantity,
                    Share = shareToBuy,
                    Username = user.Username
                };
                _userShareRepository.Add(userShare);
            }
            else
            {
                _userShareRepository.Update(userShare.Id, purchase.Quantity);
            }

            _userRepository.Update(username, new UpdateUserRequest { ChangeWalletValueAmount = -purchase.TotalValue });

            return Created(Routes.PurchaseRoutes.Create.Replace("{id}", purchase.Id.ToString()), purchase);
        }

        [HttpGet(Routes.PurchaseRoutes.GetAll)]
        public async Task<IActionResult> GetAllAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromQuery] PurchaseFilters purchaseFilters)
        {
            var userHandleResult = ValidateUser(username, password, out var user);
            if (userHandleResult != null) return userHandleResult;

            IEnumerable<Purchase> purchases = new List<Purchase>();
            double rate = 1;
            if (!user.PrefferedCurrencyIsDefault())
            {
                rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    if (user.IsAdmin)
                    {
                        purchases = _purchaseRepository.GetAll(purchaseFilters, rate);
                    }
                    else
                    {
                        purchases = _purchaseRepository.GetAll(username, purchaseFilters, rate);
                    }
                }
                else
                {
                    if (user.IsAdmin)
                    {
                        purchases = _purchaseRepository.GetAll(purchaseFilters, 1);
                    }
                    else
                    {
                        purchases = _purchaseRepository.GetAll(username, purchaseFilters, 1);
                    }
                }
            }
            else
            {
                if (user.IsAdmin)
                {
                    purchases = _purchaseRepository.GetAll(purchaseFilters, 1);
                }
                else
                {
                    purchases = _purchaseRepository.GetAll(username, purchaseFilters, 1);
                }
            }
            
            if (!purchases.Any()) return NoContent();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                if (rate != 0)
                {
                    foreach (var purchase in purchases)
                    {
                        purchase.TotalValue *= rate;
                    };
                    response.currency = user.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = purchases.OrderByDescending(x => x.PurchaseTime);
            return Ok(response);
        }

        [HttpGet(Routes.PurchaseRoutes.Get)]
        public async Task<IActionResult> GetAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromRoute] Guid id, [FromQuery] string currency)
        {
            var userHandleResult = ValidateUser(username, password, out var user);
            if (userHandleResult != null) return userHandleResult;
            var purchase = _purchaseRepository.Get(id);
            if (purchase == null) return NotFound();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    purchase.TotalValue *= rate;
                    response.currency = user.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = purchase;
            return Ok(response);
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