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
using System.Linq;

namespace SharesBroker.Controllers
{
    [ApiController]
    public class UserShareController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserShareRepository _userShareRepository;
        private readonly RateConverter _rateConverter;

        public UserShareController(IUserRepository userRepository, IShareRepository shareRepository, IUserShareRepository userShareRepository)
        {
            _userRepository = userRepository;
            _userShareRepository = userShareRepository;
            _rateConverter = new RateConverter(new CurrencyConversionWSClient());
        }

        [HttpGet(Routes.UserStockRoutes.Get)]
        public async System.Threading.Tasks.Task<IActionResult> GetAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromRoute] Guid id)
        {
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            var user = _userRepository.Get(username);

            var userShare = _userShareRepository.Get(id);

            if (userShare == null) return NotFound();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    userShare.Share.Currency = user.PrefferedCurrency;
                    userShare.Share.Value *= rate;
                    response.currency = user.PrefferedCurrency;
                }       
                else response.currencyServiceDown = true;
            }

            response.result = userShare;
            return Ok(response);
        }

        [HttpGet(Routes.UserStockRoutes.GetAll)]
        public async System.Threading.Tasks.Task<IActionResult> GetAllAsync([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password, [FromQuery] UserShareFilters userShareFilters)
        {
            if (!_userRepository.IsValidLogin(username, password)) return Unauthorized();
            var user = _userRepository.Get(username);

            IEnumerable<UserShare> userShares;
            if (user.IsAdmin) userShares = _userShareRepository.GetAll(userShareFilters);
            else userShares = _userShareRepository.GetAll(username, userShareFilters);

            if (!userShares.Any()) return NotFound();

            var response = new ConvertedResponse();

            if (!user.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(user.PrefferedCurrency);
                if (rate != 0)
                {
                    foreach (var userShare in userShares)
                    {
                        if (userShare.Share.Currency != user.PrefferedCurrency)
                        {
                            userShare.Share.Currency = user.PrefferedCurrency;
                            userShare.Share.Value *= rate;
                        }
                    }
                    response.currency = user.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = userShares.OrderBy(x => x.Username);
            return Ok(response);
        }
    }
}