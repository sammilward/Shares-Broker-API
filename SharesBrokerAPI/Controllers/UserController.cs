using CurrencyConverterService;
using Microsoft.AspNetCore.Mvc;
using SharesBrokerAPI;
using SharesBrokerAPI.Contracts.V1;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Contracts.V1.Responces;
using SharesBrokerAPI.DatabaseAccess;
using SharesBrokerAPI.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SharesBroker.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly RateConverter _rateConverter;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _rateConverter = new RateConverter(new CurrencyConversionWSClient());
        }

        [HttpGet(Routes.UserRoutes.GetAll)]
        public async Task<IActionResult> GetAllAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromQuery] UserFilters userFilters)
        {
            var requestorUser = _userRepository.Get(username);
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            if (!requestorUser.IsAdmin) return Unauthorized();

            var users = _userRepository.GetAll();

            var response = new ConvertedResponse();

            if (!requestorUser.PrefferedCurrencyIsDefault())
            {
                var rate = _rateConverter.GetRateDollarsToCurrency(requestorUser.PrefferedCurrency);
                if (await rate != 0)
                {
                    foreach (var user in users)
                    {
                        user.WalletValue *= await rate;
                    }
                    response.currency = requestorUser.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            if (userFilters.admins.HasValue && userFilters.admins.Value) users = users.Where(x => x.IsAdmin);
            else if (userFilters.admins.HasValue && !userFilters.admins.Value) users = users.Where(x => !x.IsAdmin);
            if (!string.IsNullOrEmpty(userFilters.Username)) users = users.Where(x => x.Username.ToLower().Contains(userFilters.Username.ToLower()));
            if (!string.IsNullOrEmpty(userFilters.PrefferedCurrency)) users = users.Where(x => x.PrefferedCurrency.ToLower().Contains(userFilters.PrefferedCurrency.ToLower()));
            if (userFilters.MaxWalletValue.HasValue) users = users.Where(x => x.WalletValue <= userFilters.MaxWalletValue.Value);
            if (userFilters.MinWalletValue.HasValue) users = users.Where(x => x.WalletValue >= userFilters.MinWalletValue.Value);

            response.result = users;
            return Ok(response);
        }

        [HttpGet(Routes.UserRoutes.Get)]
        public async Task<IActionResult> GetAsync([Required] [FromHeader(Name = "username")] string requestorUsername, [Required] [FromHeader(Name = "password")] string requestorPassword, [FromRoute] string username)
        {
            if (!_userRepository.IsValidLogin(requestorUsername, requestorPassword)) return Unauthorized("Username or password is inccorect");
            var requestorUser = _userRepository.Get(requestorUsername);
            if (requestorUser == null) return BadRequest($"No user exists with username: {requestorUsername}");
            if (!requestorUser.IsAdmin && requestorUser.Username != username) return Unauthorized("You need to be an admin, to get details of other peoples accounts");

            var user = _userRepository.Get(username);
            if (user == null) return BadRequest($"No user exists with username: {username}");

            var response = new ConvertedResponse();

            if (!requestorUser.PrefferedCurrencyIsDefault())
            {
                var rate = _rateConverter.GetRateDollarsToCurrency(requestorUser.PrefferedCurrency);
                if (await rate != 0)
                {
                    user.WalletValue *= await rate;
                    response.currency = requestorUser.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = user;

            return Ok(response);
        }

        [HttpPost(Routes.UserRoutes.Create)]
        public IActionResult Create([FromBody] CreateUserRequest createUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userRepository.Get(createUserRequest.Username);
            if (user != null)
            {
                return BadRequest($"User already exists with username: {createUserRequest.Username}");
            }

            if (!isValidCurrency(createUserRequest.PreferredCurrency)) return BadRequest($"The currency {createUserRequest.PreferredCurrency} is not supported");

            var newUser = new User
            {
                Username = createUserRequest.Username,
                Password = createUserRequest.Password,
                IsAdmin = createUserRequest.IsAdmin,
                PrefferedCurrency = createUserRequest.PreferredCurrency
            };

            _userRepository.Add(newUser);

            return Created(Routes.UserRoutes.Create.Replace("{username}", newUser.Username), newUser);
        }

        [HttpPut(Routes.UserRoutes.Update)]
        public async Task<IActionResult> UpdateAsync([FromHeader(Name = "username")] string requestorUsername, [FromHeader(Name = "password")] string requestorPassword, [FromRoute] string username, [FromBody] UpdateUserRequest updateUserRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestorUser = _userRepository.Get(requestorUsername);
            if (requestorUser == null) return BadRequest($"No user exists with username: {requestorUsername}");
            if (!_userRepository.IsValidLogin(requestorUsername, requestorPassword)) return Unauthorized("Username or password is inccorect");
            if (requestorUser.Username != username) return Unauthorized("You can only update your own account");

            var user = _userRepository.Get(username);
            if (user == null) return BadRequest($"No user exists with username: {username}");

            if (user.PrefferedCurrency != updateUserRequest.PrefferedCurrency && !string.IsNullOrEmpty(updateUserRequest.PrefferedCurrency))
            {
                if (!isValidCurrency(updateUserRequest.PrefferedCurrency)) return BadRequest($"The currency {updateUserRequest.PrefferedCurrency} is not supported");
                _userRepository.UpdatePrefferedCurrency(username, updateUserRequest.PrefferedCurrency);
                user = _userRepository.Get(username);
            }

            var response = new ConvertedResponse();

            if (updateUserRequest.ChangeWalletValueAmount.HasValue)
            {            
                if (!user.PrefferedCurrencyIsDefault())
                {
                    var rateToDollars = await _rateConverter.GetRateCurrencyToDollars(user.PrefferedCurrency);
                    var rateFromDollars = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);

                    if (rateToDollars == 0 || rateFromDollars == 0)
                    {
                        if (updateUserRequest.ChangeWalletValueAmount < 0 && user.WalletValue < Math.Abs(updateUserRequest.ChangeWalletValueAmount.Value)) return BadRequest($"User does not have enough in their wallet");
                        user = _userRepository.Update(username, updateUserRequest);
                        response.currencyServiceDown = true;
                    }
                    else
                    {
                        if (updateUserRequest.ChangeWalletValueAmount < 0 && (user.WalletValue * rateFromDollars) < Math.Abs(updateUserRequest.ChangeWalletValueAmount.Value)) return BadRequest($"User does not have enough in their wallet");
                        updateUserRequest.ChangeWalletValueAmount *= rateToDollars;
                        user = _userRepository.Update(username, updateUserRequest);
                        user.WalletValue *= rateFromDollars;
                        response.currency = user.PrefferedCurrency;
                    }
                }
                else
                {
                    if (updateUserRequest.ChangeWalletValueAmount < 0 && user.WalletValue < Math.Abs(updateUserRequest.ChangeWalletValueAmount.Value)) return BadRequest($"User does not have enough in their wallet");
                    user = _userRepository.Update(username, updateUserRequest);
                }
            }

            response.result = user;

            return Ok(response);
        }

        [HttpDelete(Routes.UserRoutes.Delete)]
        public IActionResult Delete([FromHeader(Name = "username")] string requestorUsername, [FromHeader(Name = "password")] string requestorPassword, [FromRoute] string username)
        {
            var requestorUser = _userRepository.Get(requestorUsername);
            if (requestorUser == null) return BadRequest($"No user with username: {username}");
            if (!_userRepository.IsValidLogin(requestorUsername, requestorPassword)) return Unauthorized();
            if (!requestorUser.IsAdmin) return Unauthorized("You need to be an admin to remove other users");

            var user = _userRepository.Get(username);
            if (user == null) return BadRequest($"No user with username: {username}");
            if (!user.IsAdmin) return BadRequest($"Can not delete: {username} as they are not an admin, and may have still have shares");
            else _userRepository.Delete(username);
            return Ok();
        }

        private bool isValidCurrency(string currency)
        {
            List<string> availableCurrencies = new List<string> { "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTC", "BTN", "BWP", "BYN", "BYR", "BZD", "CAD", "CDF", "CHF", "CLF", "CLP", "CNY", "COP", "CRC", "CUC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL", "GGP", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LTL", "LVL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRO", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SRD", "STD", "SVC", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VEF", "VND", "VUV", "WST", "XAF", "XAG", "XAU", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR", "ZMK", "ZMW", "ZWL" };
            return availableCurrencies.Contains(currency);
        }
    }
}