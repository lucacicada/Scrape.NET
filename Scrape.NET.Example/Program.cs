namespace ExampleConsoleApp;

using AngleSharp.Html.Dom;
using Scrape.NET;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        HttpClient httpClient = new();

        IHtmlDocument html = await httpClient.GetAsync<IHtmlDocument>("https://google.com");

        Console.WriteLine(html.CssOrFail("title").TextContent());
    }
}
