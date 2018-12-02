﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LtiAdvantage.Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// Dynamically add required scope authorization policy.
    /// See https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/.
    /// </summary>
    /// <example>
    /// To protect an API
    /// 
    /// [Authorize("https://purl.imsglobal.org/spec/lti-ags/scope/lineitem")]
    /// </example>
    /// <example>
    /// To add AuthorizationPolicyProvider to ConfigureServices
    ///
    /// 
    /// </example>
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IOptions<AuthorizationOptions> _options;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder().AddRequirements()
                    .RequireClaim("scope", policyName)
                    .Build();

                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                _options.Value.AddPolicy(policyName, policy);
            }

            return policy;
        }
    }
}