using Braintree;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinePizzaDelivery_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _options { get; set; }
        private IBraintreeGateway brainTreeGateWay { get; set; }

        private readonly IConfiguration _configuration;
        public BrainTreeGate(IOptions<BrainTreeSettings> options, IConfiguration configuration)
        {
            _options = options.Value;
            _configuration = configuration;
        }

        public IBraintreeGateway CreateGateway()
        {
            _options = _configuration.GetSection("BrainTree").Get<BrainTreeSettings>();
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (brainTreeGateWay == null)
            {
                brainTreeGateWay = CreateGateway();
            }
            return brainTreeGateWay;
        }
    }
}
