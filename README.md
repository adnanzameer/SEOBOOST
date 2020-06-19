[![Platform](https://img.shields.io/badge/Episerver-%2011.0.+-orange.svg?style=flat)](http://world.episerver.com/cms/)

# SEOBOOST for EPiServer

## Description
This package acilitates developers and editors to improve the SEO ranking of the website by utalizing helper methods & features provided.

## Features
The package provides the following helper methods & features:
* Canonical Link 
* hreflang attributes (Alternate Links)
* Breadcrumbs Items
* robots.txt

## How to install
Install NuGet package from Episerver NuGet Feed:

	Install-Package SeoBoost

## How to use

Include the follow **@ using SeoBoost.Helper** at top of the Mater page.
     
### For Canonical Link
Use the following extension **@Html.GetCanonicalLink()** within **<head></head>** section.
     
### For Alternate Links
Use the following extension **@Html.GetAlternateLinks()** within **<head></head>** section.

### For Breadcrumbs Items
Use the following extension **@Html.GetBreadcrumbItemList()** where required.

### robots.txt

The idea behind this feature is simple, provide editors with the flexibility to change robots.txt file on the go. 

The Robots.txt page (backed by SBRobotsTxt PageType) will automatically be created for the site under Start Page. 

![robots.txt PageType](assets/docsimages/image001.png)

The physical rebots.txt file content (if any) will be replaced with the CMS robobts.txt page content. The fallback behaviour of /robots.txt URL is the content of physical rebots.txt file (if any) otherwise the default 404 error page will be shown.

There are restrictions in place to move (change parent) or delete the Rebots.txt page. 

![robots.txt restrictions](assets/docsimages/image003.png)

#### How to enable robots.txt feature
The editable robot.txt feature is disabled by default and can be enabled by setting "Disable Robots.txt feature" property to "true" in the Robotx.Txt page. 

![Robots.txt Page properties](assets/docsimages/image002.png)

### Additional helper methods

There are some helper methods in the packageto get external URLs of the the page. Developer can use these methods for thier own implementations 

usage 

      var urlHelper = ServiceLocator.Current.GetInstance<SeoBoost.Business.Url.IUrlService>();


There are three method available to get external url for the content 

       string GetExternalUrl(ContentReference contentReference);
       string GetExternalFriendlyUrl(ContentReference contentReference, string culture);
       string GetExternalFriendlyUrl(ContentReference contentReference);

## Changelog
### Changes in version 1.5.0
1. Added functionality to support editable robots.txt in CMS 
