using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace SeoBoost.Business.Url
{
    public class UrlBuilder
    {
        private const string Pattern = @"^(([^:\/?#]+):)?(\/\/([^\/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?";
        private const string SchemeSeparator = "://";
        private const string PathSeparator = "/";

        private const string QueryPathSeparator = "?";
        private const string QueryKeyValueSeparator = "=";
        private const string QueryParametersSeparator = "&";

        private string _scheme;
        private string _hostname;
        private List<string> _path = new List<string>();
        private Dictionary<string, string> _query = new Dictionary<string, string>();
        private Regex _regex;

        private bool _endWithSeparator;

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
            string url = $"{GetScheme()}{GetHost()}{GetPath()}{GetQuery()}";
            return TransformUrl(url);
        }

        public string GetExternalUrlHost()
        {
            string url = $"{GetScheme()}{GetHost()}";
            return TransformUrl(url);
        }

        public UrlBuilder AddMissingPathPart()
        {
            if (HttpContext.Current == null)
            {
                return this;
            }

            string currentUrl = GetScheme() + GetHost() + GetPath();
            string contextUrl = HttpContext.Current.Request.Url.AbsoluteUri;

            if (!contextUrl.StartsWith(currentUrl))
            {
                return this;
            }

            string missingUrlPart = contextUrl.Remove(0, currentUrl.Length);
            int questionMarkPosition = missingUrlPart.IndexOf(QueryPathSeparator, 0, StringComparison.InvariantCultureIgnoreCase);

            if (questionMarkPosition >= 0)
            {
                missingUrlPart = missingUrlPart.Remove(questionMarkPosition);
            }

            List<string> missingPath = missingUrlPart.Split(PathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            _path.AddRange(missingPath);

            return this;
        }

        private UrlBuilder SetPath(string path)
        {
            List<string> pathParts = path.Split(PathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return SetPath(pathParts);
        }

        private UrlBuilder SetPath(List<string> path)
        {
            _path = path;
            return this;
        }

        private UrlBuilder SetQuery(string query)
        {
            string[] parameters = query.Split(QueryParametersSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var queryDictionary = new Dictionary<string, string>();

            foreach (string parameter in parameters)
            {
                string[] keyValue = parameter.Split(QueryKeyValueSeparator.ToCharArray());
                string key = keyValue[0];
                string value = keyValue.Length == 2
                    ? keyValue[1]
                    : string.Empty;

                queryDictionary.Add(key, value);
            }

            return SetQuery(queryDictionary);
        }

        private UrlBuilder SetQuery(Dictionary<string, string> query)
        {
            _query = query;
            return this;
        }

        private UrlBuilder SetPathAndQuery(string pathAndQuery)
        {
            string[] splitPathAndQuery = pathAndQuery.Split(QueryPathSeparator[0]);

            string path = splitPathAndQuery[0];
            string query = splitPathAndQuery.Length > 1
                ? splitPathAndQuery[1]
                : string.Empty;

            return SetPath(path)
                .SetQuery(query);
        }

        private void ExtractFromContentReference(ContentReference contentReference, string culture= null)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var pageData = repository.Get<PageData>(contentReference);

            string url = !ContentReference.IsNullOrEmpty(contentReference)
                ? UrlResolver.Current.GetUrl(contentReference, culture, new VirtualPathArguments { ContextMode = ContextMode.Default })
                : string.Empty;
            if (!string.IsNullOrEmpty(url) && url.EndsWith("/"))
                _endWithSeparator = true;

            ExtractFromString(url);

            if (!string.IsNullOrEmpty(pageData?.ExternalURL))
            {
                SetPathAndQuery(pageData.ExternalURL);
            }
        }

        private void ExtractFromString(string url)
        {
            if (_regex == null)
            {
                _regex = new Regex(Pattern, RegexOptions.IgnoreCase);
            }

            Match match = url == null
                ? _regex.Match(string.Empty)
                : _regex.Match(url);
            Uri siteUrl = SiteDefinition.Current.SiteUrl;

            _scheme = match.Groups[RegexIndex.Scheme].Success
                ? match.Groups[RegexIndex.Scheme].ToString()
                : siteUrl?.Scheme ?? "http";

            _hostname = match.Groups[RegexIndex.Host].Success
                ? match.Groups[RegexIndex.Host].ToString()
                : siteUrl.Host;

            string pathString = match.Groups[RegexIndex.Path].Success
                ? match.Groups[RegexIndex.Path].ToString()
                : string.Empty;

            SetPath(pathString);

            string queryString = match.Groups[RegexIndex.Query].Success
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

            foreach (string path in _path)
            {
                stringBuilder.Append(PathSeparator)
                    .Append(path);
            }

            if (_endWithSeparator)
            {
                stringBuilder.Append(PathSeparator);
            }

            string url = stringBuilder.ToString();

            return url;
        }

        private string GetQuery()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(QueryPathSeparator);

            foreach (KeyValuePair<string, string> parameter in _query)
            {
                stringBuilder.Append(parameter.Key)
                    .Append(QueryKeyValueSeparator)
                    .Append(parameter.Value)
                    .Append(QueryParametersSeparator);
            }

            string query = stringBuilder.ToString();

            return query.Remove(query.Length - 1);
        }

        private string TransformUrl(string url)
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
