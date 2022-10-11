using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;

namespace SeoBoost.Business.Url
{
    public class UrlBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string Pattern = @"^(([^:\/?#]+):)?(\/\/([^\/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?";
        private const string SchemeSeparator = "://";
        private const string PathSeparator = "/";

        private const string QueryPathSeparator = "?";
        private const string QueryKeyValueSeparator = "=";
        private const string QueryParametersSeparator = "&";

        private bool _endWithSeparator;
        private string _hostname;
        private List<string> _path = new List<string>();
        private Dictionary<string, string> _query = new Dictionary<string, string>();
        private Regex _regex;

        private string _scheme;

        public UrlBuilder(ContentReference contentReference)
        {
            ExtractFromContentReference(contentReference);
        }

        public UrlBuilder(ContentReference contentReference, string culture)
        {
            ExtractFromContentReference(contentReference, culture);
        }

        public string GetExternalUrl()
        {

            var url = $"{GetScheme()}{GetHost()}{GetPath()}{GetQuery()}";
            return TransformUrl(url);
        }

        public string GetAbsoluteUrl()
        {
            var httpContextAccessor = ServiceLocator.Current.GetInstance<IHttpContextAccessor>();

            if (httpContextAccessor.HttpContext != null)
            {
                var request = httpContextAccessor.HttpContext.Request;

                return $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
            }

            return "";
        }


        public void AddMissingPathPart()
        {
            var currentUrl = GetScheme() + GetHost() + GetPath();
            var contextUrl = GetAbsoluteUrl();

            if (!contextUrl.StartsWith(currentUrl)) return;

            var missingUrlPart = contextUrl.Remove(0, currentUrl.Length);
            var questionMarkPosition =
                missingUrlPart.IndexOf(QueryPathSeparator, 0, StringComparison.InvariantCultureIgnoreCase);

            if (questionMarkPosition >= 0)
                missingUrlPart = missingUrlPart.Remove(questionMarkPosition);

            var missingPath = missingUrlPart.Split(PathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            _path.AddRange(missingPath);
        }

        private void SetPath(string path)
        {
            var pathParts = path.Split(PathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            SetPath(pathParts);
        }

        private void SetPath(List<string> path)
        {
            _path = path;
        }

        private void SetQuery(string query)
        {
            var parameters = query.Split(QueryParametersSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var queryDictionary = new Dictionary<string, string>();

            foreach (var parameter in parameters)
            {
                var keyValue = parameter.Split(QueryKeyValueSeparator.ToCharArray());
                var key = keyValue[0];
                var value = keyValue.Length == 2
                    ? keyValue[1]
                    : string.Empty;

                queryDictionary.Add(key, value);
            }

            SetQuery(queryDictionary);
        }

        private void SetQuery(Dictionary<string, string> query)
        {
            _query = query;
        }

        private void ExtractFromContentReference(ContentReference contentReference, string culture = null)
        {
            var url = !ContentReference.IsNullOrEmpty(contentReference)
                ? UrlResolver.Current.GetUrl(contentReference, culture,
                    new VirtualPathArguments { ContextMode = ContextMode.Default })
                : string.Empty;
            if (!string.IsNullOrEmpty(url) && url.EndsWith("/"))
                _endWithSeparator = true;

            ExtractFromString(url);
        }

        private void ExtractFromString(string url)
        {
            _regex ??= new Regex(Pattern, RegexOptions.IgnoreCase);

            var match = url == null
                ? _regex.Match(string.Empty)
                : _regex.Match(url);
            var siteUrl = SiteDefinition.Current.SiteUrl;

            _scheme = match.Groups[RegexIndex.Scheme].Success
                ? match.Groups[RegexIndex.Scheme].ToString()
                : siteUrl?.Scheme ?? "http";

            _hostname = match.Groups[RegexIndex.Host].Success
                ? match.Groups[RegexIndex.Host].ToString()
                : siteUrl?.Host;

            var pathString = match.Groups[RegexIndex.Path].Success
                ? match.Groups[RegexIndex.Path].ToString()
                : string.Empty;

            SetPath(pathString);

            var queryString = match.Groups[RegexIndex.Query].Success
                ? match.Groups[RegexIndex.Query].ToString()
                : string.Empty;

            SetQuery(queryString);
        }

        private string GetScheme(bool includeSeparator = true)
        {
            return includeSeparator
                ? _scheme + SchemeSeparator
                : _scheme;
        }

        private string GetHost(bool endWithSeparator = false)
        {
            return endWithSeparator
                ? _hostname + PathSeparator
                : _hostname;
        }

        private string GetPath()
        {
            var stringBuilder = new StringBuilder();

            foreach (var path in _path)
                stringBuilder.Append(PathSeparator)
                    .Append(path);

            if (_endWithSeparator)
                stringBuilder.Append(PathSeparator);

            var url = stringBuilder.ToString();

            return url;
        }

        private string GetQuery()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(QueryPathSeparator);

            foreach (var parameter in _query)
                stringBuilder.Append(parameter.Key)
                    .Append(QueryKeyValueSeparator)
                    .Append(parameter.Value)
                    .Append(QueryParametersSeparator);

            var query = stringBuilder.ToString();

            return query.Remove(query.Length - 1);
        }

        private static string TransformUrl(string url)
        {
            return url != string.Empty
                ? url.ToLower()
                : PathSeparator;
        }

        private static class RegexIndex
        {
            public const int Scheme = 2;
            public const int Host = 4;
            public const int Path = 5;
            public const int Query = 7;
        }
    }
}