namespace Scrape.NET.Tests;

[TestFixture]
public class UriNormalizerTests
{
    [Test]
    [TestCase("http://example", "http://example/")]
    [TestCase("http://EXAMPLE", "http://example/")]
    [TestCase("http://example/path", "http://example/path")]
    [TestCase("http://example/path/", "http://example/path/")]
    [TestCase("http://example/path?b=c&b=a&a=f", "http://example/path?a=f&b=a&b=c")]
    [TestCase("http://example/path?b=c&b=a&a=f#fragment-is-ignored", "http://example/path?a=f&b=a&b=c#fragment-is-ignored")]
    [Parallelizable(ParallelScope.All)]
    public void ShouldWork(string uri, string result)
    {
        Assert.That(UriNormalizer.NormalizeUriAsString(new Uri(uri, UriKind.Absolute)), Is.EqualTo(result));
    }
}
