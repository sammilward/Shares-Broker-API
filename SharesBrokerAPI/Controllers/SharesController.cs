using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyConverterService;
using Microsoft.AspNetCore.Mvc;
using SharesBrokerAPI;
using SharesBrokerAPI.Contracts.V1;
using SharesBrokerAPI.Contracts.V1.Requests;
using SharesBrokerAPI.Contracts.V1.Responces;
using SharesBrokerAPI.DatabaseAccess;
using SharesBrokerAPI.Domain.HTTP;
using SharesBrokerAPI.Domain.Models;

namespace SharesBroker.Controllers
{
    [ApiController]
    public class SharesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IShareRepository _shareRepository;
        private readonly ShareConverter _shareConverter;
        private readonly RateConverter _rateConverter;
        private readonly SharesUpdateHandler _sharesUpdateHandler;

        public SharesController(IUserRepository userRepository, IShareRepository shareRepository, IHTTPClientFactory hTTPClientFactory)
        {
            _userRepository = userRepository;
            _shareRepository = shareRepository;
            _shareConverter = new ShareConverter();
            _rateConverter = new RateConverter(new CurrencyConversionWSClient());
            _sharesUpdateHandler = new SharesUpdateHandler(shareRepository, hTTPClientFactory);
        }

        [HttpGet(Routes.SharesRoutes.GetAll)]
        public async Task<ActionResult<IEnumerable<Share>>> GetAsync([FromHeader(Name = "username")] string requestorUsername, [FromHeader(Name = "password")] string requestorPassword, [FromQuery] ShareFilters shareFilters)
        {
            var requestorUser = _userRepository.Get(requestorUsername);
            if (!_userRepository.IsValidLogin(requestorUsername, requestorPassword)) return Unauthorized();

            List<Share> shares;
            var response = new ConvertedResponse();

            if (!requestorUser.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(requestorUser.PrefferedCurrency);
                if (rate == 0)
                {
                    shares = _shareRepository.GetAll(shareFilters, 1).ToList();
                    response.currencyServiceDown = true;
                }
                else
                {
                    shares = _shareRepository.GetAll(shareFilters, rate).ToList();
                    shares = _shareConverter.ConvertShares(shares, requestorUser.PrefferedCurrency, rate).ToList();
                    response.currency = requestorUser.PrefferedCurrency;
                }
            }
            else
            {
                shares = _shareRepository.GetAll(shareFilters, 1).ToList();
            }
            
            if (shares.ToList().Count == 0) return NotFound();
            response.result = shares;

            return Ok(response);
        }
       
        [HttpGet(Routes.SharesRoutes.Get)]
        public async Task<ActionResult<string>> GetAsync([FromHeader(Name = "username")] string requestorUsername, [FromHeader(Name = "password")] string requestorPassword, [FromRoute] string companySymbol)
        {
            var requestorUser = _userRepository.Get(requestorUsername);
            if (!_userRepository.IsValidLogin(requestorUsername, requestorPassword)) return Unauthorized();

            var share = _shareRepository.Get(companySymbol);
            if (share == null) return NotFound();
            await _sharesUpdateHandler.HandleShareUpdateAsync(companySymbol);

            var response = new ConvertedResponse();

            share = _shareRepository.Get(companySymbol);
            if (!requestorUser.PrefferedCurrencyIsDefault())
            {
                var rate = await _rateConverter.GetRateDollarsToCurrency(requestorUser.PrefferedCurrency);
                if (rate != 0)
                {
                    share = _shareConverter.ConvertShares(share, requestorUser.PrefferedCurrency, rate);
                    response.currency = requestorUser.PrefferedCurrency;
                }
                else response.currencyServiceDown = true;
            }

            response.result = share;
            return Ok(response);
        }
    }
}