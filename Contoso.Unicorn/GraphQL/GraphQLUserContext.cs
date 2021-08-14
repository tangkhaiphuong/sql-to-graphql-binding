using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using Fluid;
using Microsoft.AspNetCore.Http;

namespace Contoso.Unicorn.GraphQL
{
    /// <inheritdoc cref="System.Collections.DictionaryBase" />
    public sealed class GraphQLUserContext : Dictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        /// <inheritdoc />
        public GraphQLUserContext(
            ClaimsPrincipal user,
            HttpRequest request,
            HttpResponse response,
            IServiceProvider serviceProvider)
        {
            User = user;
            Request = request;
            Response = response;
            ServiceProvider = serviceProvider;

            var templateContext = new TemplateContext { Model = new Dictionary<string, object> { { "global", _dictionary } } };
            this["templateContext"] = templateContext;
            this["global"] = _dictionary;
        }

        /// <summary>
        /// Gets user.
        /// </summary>
        public ClaimsPrincipal User
        {
            get => (ClaimsPrincipal)this[nameof(User)];
            set => this[nameof(User)] = value;
        }

        /// <summary>
        /// Gets http requests.
        /// </summary>
        public HttpRequest Request
        {
            get => (HttpRequest)this[nameof(Request)];
            set => this[nameof(Request)] = value;
        }

        /// <summary>
        /// Gets http response 
        /// </summary>
        public HttpResponse Response
        {
            get => (HttpResponse)this[nameof(Response)];
            set => this[nameof(Response)] = value;
        }

        /// <summary>
        /// Gets service provider.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get => (IServiceProvider)this[nameof(ServiceProvider)];
            set => this[nameof(ServiceProvider)] = value;
        }
    }
}