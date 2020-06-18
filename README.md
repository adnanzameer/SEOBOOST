[![Platform](https://img.shields.io/badge/Episerver-%2011.0.+-orange.svg?style=flat)](http://world.episerver.com/cms/)

SEOBOOST for EPiServer
=====================

SEOBOOST for Episerver 11

The package provides the following helper methods & features:
* Canonical Link 
* hreflang attributes (Alternate Links)
* Breadcrumbs Items
* robots.txt

## Usage:

Include the follow **@ using SeoBoost.Helper** at top of the Mater page.
 	
### For Canonical Link
Use the following extension **@Html.GetCanonicalLink()** within **<head></head>** section.
 	
### For Alternate Links
Use the following extension **@Html.GetAlternateLinks()** within **<head></head>** section.

### For Breadcrumbs Items
Use the following extension **@Html.GetBreadcrumbItemList()** where required.

### For robots.txt

The idea behind this feature is simple, provide editors with the flexibility to change robots.txt file on the go. The Robots.txt page (backed by SBRobotsTxt PageType) will automatically be created for the site under Start Page. 

![robots.txt PageType](assets/docsimages/image001.png)

The physical rebots.txt file content (if any) will be replaced with the CMS robobts.txt page content.

There are restrictions in place to move Rebots.txt page (other than under Start Page to change order) or delete the page. If editors want to disable robot.txt feature use the "Disable Robots.txt feature" property in the Robotx.Txt page.

If the editors set the "Disable Robots.txt feature" to true the physical rebots.txt file content (if any) will be shown otherwise the default 404 error page will be shown.


