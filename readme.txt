---------------------------------------------------------------------
------ SEOBOOST Robots dynamic generator Readme -----------
---------------------------------------------------------------------
 
!!!!!!!! IMPORTANT  !!!!!!!!

Create robots.txt page available under SEOBOOST GroupName under the Start page per domain/site

-- Web.config --

If you are using IIS URL Rewrite rules to add a trailing slash at the end of the URL, add the following in the rule to ignore robots.txt route 

<add input="{URL}" pattern="\robots.txt" negate="true" />

-- Global.asax --

If you are using web API in your project, please add these lines in Global.asax.cs

protected override void RegisterRoutes(System.Web.Routing.RouteCollection routes)
{
    base.RegisterRoutes(routes);
    routes.Ignore("robots.txt");
}

-- FEATURES Robots --

* Multi-site support 
* robots.txt page type under Start page per domain/site
* Web API to intercept calls to robots.txt path per domain/site