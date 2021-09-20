# Abot 2.0.70

## Main execution flow

### 1. Start with root Uri

*WebCrawler.CrawlAsync(Uri, CancellationTokenSource)*

- CrawlContext.RootUri = uri

- rootPage = new PageToCrawl( uri )

- if **ShouldSchedulePageLink**(rootPage)

- **scheduler.Add**( rootPage )

### 2. Main loop

*WebCrawler.CrawlSite()*

while (!crawlComplete)
  
- CheckForCancellationRequest() 
  - if crawlContext.CancellationTokenSource.IsCancellationRequested
  - rawlResult.ErrorException = new OperationCanceledException()
  - crawlContext.IsCrawlHardStopRequested = true
- CheckForHardStopRequest() 
  - if crawlContext.IsCrawlHardStopRequested
  - after an unknown exception occured in WebCrawler.ProcessPage
  - after CheckMemoryUsage failed
  - after a cancellation request
  - after a crawl timeout
  - after CrawlDecision.ShouldHardStopCrawl in ShouldCrawlPageLinks / ShouldCrawlPage / ShouldRecrawlPage / ShouldDownloadPageContent
  - **scheduler.Clear**()
  - **threadManager.AbortAll**()
  - set all events to null so no more events are fired
- CheckForStopRequest()
  - if crawlContext.IsCrawlStopRequested
  - after CrawlDecision.ShouldStopCrawl in ShouldCrawlPageLinks / ShouldCrawlPage / ShouldRecrawlPage / ShouldDownloadPageContent
  - **scheduler.Clear**()

- if scheduler.Count > 0
- pageToCrawl = **scheduler.GetNext**()
- **threadManager.DoWork**(   =>  ProcessPage(pageToCrawl) )

### 3. Process one page

*WebCrawler.ProcessPage(PageToCrawl)*

*WebCrawler.CrawlThePage(PageToCrawl)*

*WebCrawler.SchedulePageLinks(CrawledPage)*

- crawledPage = await **pageRequester.MakeRequestAsync**(Uri, Func **shouldDownloadPageContent**)

*-- 2 ways to handle http redirects --*

- if **CrawlConfiguration.IsHttpRequestAutoRedirectsEnabled** && (crawledPage.HttpResponseMessage.RequestMessage.RequestUri.AbsoluteUri != crawledPage.Uri.AbsoluteUri)	
  - do nothing special, redirect was silently managed before

- if **not CrawlConfiguration.IsHttpRequestAutoRedirectsEnabled** && crawledPage.HttpResponseMessage.StatusCode >= 300 && crawledPage.HttpResponseMessage.StatusCode <= 399	
    - uri = ExtractRedirectUri(crawledPage);
    - page = new PageToCrawl(uri);
    - if **ShouldSchedulePageLink**(page)
    - **scheduler.Add**(page)

*-- Extract page links --*

- if **ShouldCrawlPageLinks**(crawledPage) || CrawlConfiguration.IsForcedLinkParsingEnabled
- crawledPage.ParsedLinks = **htmlParser.GetLinks**(crawledPage);

foreach (uri in crawledPage.ParsedLinks)

- if not **scheduler.IsUriKnown(hyperLink.HrefValue)** &&
- if  **ShouldScheduleLinkDecisionMaker**.Invoke(uri, crawledPage, crawlContext)		
- page = new PageToCrawl(uri);	
- page.IsInternal = **IsInternalUri**(uri)

- if **ShouldSchedulePageLink**(page)
   - **scheduler.Add**(page)
  
- **scheduler.AddKnownUri**(uri);		

return

- if **ShouldRecrawlPage**(crawledPage)	
- crawledPage.RetryAfter = seconds;
- crawledPage.IsRetry = true;
- **scheduler.Add**(crawledPage);

### 4. Decisions to limit the crawl scope

#### ShouldDownloadPageContent



#### ShouldCrawlPageLinks

#### IsInternalUri

**IsInternalUriDecisionMaker**(uri, crawlContext.RootUri) ||
**IsInternalUriDecisionMaker**(uri, crawlContext.OriginalRootUri)

#### ShouldScheduleLink

#### ShouldSchedulePageLink

#### ShouldCrawlPage

#### ShouldRecrawlPage

### 5. Events activated during the crawl

PageCrawlStarting

- Event that is fired before a page is crawled.
- PageToCrawl PageToCrawl
- CrawlContext CrawlContext

 PageCrawlCompleted

- Event that is fired when an individual page has been crawled.
- CrawledPage CrawledPage
- CrawlContext CrawlContext

PageCrawlDisallowed

- Event that is fired when the ICrawlDecisionMaker.ShouldCrawl impl returned false. This means the page or its links were not crawled.
- string DisallowedReason
- PageToCrawl PageToCrawl
- CrawlContext CrawlContext

PageLinksCrawlDisallowed

- Event that is fired when the ICrawlDecisionMaker.ShouldCrawlLinks impl returned false. This means the page's links were not crawled.
- string DisallowedReason
- CrawledPage CrawledPage
- CrawlContext CrawlContext

#### Event properties

**CrawlContext**

(1) Crawl config

- Uri RootUri : *The root of the crawl*
- Uri OriginalRootUri : *The root of the crawl specified in the configuration. If the root URI was redirected to another URI, it will be set in RootUri.*
- CrawlConfiguration CrawlConfiguration : *Configuration values used to determine crawl settings*

(2) Crawl stats

- DateTime CrawlStartDate : *Crawl starting date*
- int CrawledCount : *Total number of pages that have been crawled*
- ConcurrentDictionary<string, int> CrawlCountByDomain : *Threadsafe dictionary of domains and how many pages were crawled in that domain*
- int MemoryUsageBeforeCrawlInMb : *The memory usage in mb at the start of the crawl*
- int MemoryUsageAfterCrawlInMb : *The memory usage in mb at the end of the crawl*

(3) Scheduler state

- IScheduler Scheduler : *The scheduler that is being used*
- IsCrawlStopRequested : *Whether a request to stop the crawl has happened. Will clear all scheduled pages but will allow any threads that are currently crawling to complete.*
- IsCrawlHardStopRequested : * Whether a request to hard stop the crawl has happened. Will clear all scheduled pages and cancel any threads that are currently crawling.*
- CancellationTokenSource CancellationTokenSource : *Cancellation token used to hard stop the crawl. Will clear all scheduled pages and abort any threads that are currently crawling.*

(4) Custom data

- dynamic CrawlBag : *Random dynamic values - used to share user data during the crawl*

**PageToCrawl**

(1) Uri

- Uri Uri : *The uri of the page*
- bool IsRoot : *Whether the page is the root uri of the crawl*
- bool IsInternal : *Whether the page is internal to the root uri of the crawl*

(2) Crawl hierarchy

- Uri ParentUri : *The parent uri of the page*
- int CrawlDepth : *The depth from the root of the crawl. If this page is the homepage this value will be zero, if this page was found on the homepage this value will be 1 and so on.*

(3) Http Redirect (before)

- CrawledPage RedirectedFrom : *The uri that this page was redirected from. If null then it was not part of the redirect chain*
- int RedirectPosition : *The position in the redirect chain. The first redirect is position 1, the next one is 2 and so on.*

(4) Http Retries

- bool IsRetry : *Whether http requests had to be retried more than once. This could be due to throttling or politeness.*
- double? RetryAfter : *The time in seconds that the server sent to wait before retrying.*
- int RetryCount : *The number of times the http request was be retried.*
- DateTime? LastRequest : *The datetime that the last http request was made. Will be null unless retries are enabled.*

(5) Custom data

- dynamic PageBag : *an store values of any type. Useful for adding custom values to the CrawledPage dynamically from event subscriber code*

**CrawledPage**

(0) PageToCrawl properties, and in addition :

(1) Http Request -> Response

- HttpRequestMessage HttpRequestMessage : *Web request sent to the server.*
- HttpResponseMessage HttpResponseMessage : *Web response from the server.*
- HttpRequestException HttpRequestException : *The request exception that occurred during the request*
- HttpClientHandler HttpClientHandler : *The HttpClientHandler that was used to make the request to server*

(2) Request and download duration

- DateTime RequestStarted : *A datetime of when the http request started*
- DateTime RequestCompleted : *A datetime of when the http request completed*
- double Elapsed : *Time it took from RequestStarted to RequestCompleted in milliseconds*
- DateTime? DownloadContentStarted : *A datetime of when the page content download started, this may be null if downloading the content was disallowed by the CrawlDecisionMaker or the inline delegate ShouldDownloadPageContent*
- DateTime? DownloadContentCompleted : *A datetime of when the page content download completed, this may be null if downloading the content was disallowed by the CrawlDecisionMaker or the inline delegate ShouldDownloadPageContent*

(3) Page content : text, HTML, links

- PageContent Content : *The content of page request*
- IHtmlDocument AngleSharpHtmlDocument : *Lazy loaded AngleSharp IHtmlDocument (https://github.com/AngleSharp/AngleSharp) that can be used to retrieve/modify html elements on the crawled page.*
- IEnumerable<HyperLink> ParsedLinks : *Links parsed from page. This value is set by the WebCrawler.SchedulePageLinks() method only If the "ShouldCrawlPageLinks" rules return true or if the IsForcedLinkParsingEnabled config value is set to true.*

InitializeAngleSharpHtmlParser()
- angleSharpHtmlParser = new AngleSharp.Html.Parser.HtmlParser()
- angleSharpHtmlParser.ParseDocument(Content.Text)

(4) Http Redirect (after)

- PageToCrawl RedirectedTo : *The page that this pagee was redirected to*

**PageContent**

- byte[] Bytes : *The raw data bytes taken from the web response*
- string Charset : *String representation of the charset/encoding*
- Encoding Encoding : *The encoding of the web response*
- string Text : *The raw text taken from the web response*

### 6. IPageRequester / PageRequester

### 7. IHtmlParser / AngleSharpHyperlinkParser


### 8. PoliteWebCrawler

### 9. Configuration properties

