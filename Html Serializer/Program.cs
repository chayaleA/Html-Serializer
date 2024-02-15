using Html_Serializer;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;


var html = await Load("https://forum.netfree.link/categories");
var clearHtml = new Regex(@"[\r\n]+").Replace(html, "");
var htmlLines = new Regex("<(.*?)>").Split(clearHtml).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

//search selector in html page
var root = HtmlElement.Serialize(htmlLines);

List<HtmlElement> findElements = new List<HtmlElement>();


Console.WriteLine("First search: main #nav-dropdown #logged-out-menu li");
findElements = HtmlElement.findElementsBySelector(Selector.Convert("main #nav-dropdown #logged-out-menu li"), root);
Console.WriteLine("amount: " + findElements.Count);
if (findElements.Count > 0)
    findElements.ForEach(e => Console.WriteLine(e));
else
    Console.WriteLine("nothing found!");

Console.WriteLine();
Console.WriteLine("Second search: .container .navbar-header button");
findElements = HtmlElement.findElementsBySelector(Selector.Convert(".container .navbar-header button"), root);
Console.WriteLine("amount: " + findElements.Count);
if (findElements.Count > 0)
    findElements.ForEach(e => Console.WriteLine(e));
else
    Console.WriteLine("nothing found!");

Console.WriteLine();
Console.WriteLine("Third search: form#search-form .form-group .form-control");
findElements = HtmlElement.findElementsBySelector(Selector.Convert("form#search-form .form-group .form-control"), root);
Console.WriteLine("amount: " + findElements.Count);
if (findElements.Count > 0)
    findElements.ForEach(e => Console.WriteLine(e));
else
    Console.WriteLine("nothing found!");

Console.ReadLine();

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var hrml = await response.Content.ReadAsStringAsync();
    return hrml;
}
















//my adds:

//print all the elements tree

//Console.WriteLine("Descendants");
//Console.WriteLine();

//var a = root.Descendants();

//foreach (var item in a)
//{
//    Console.WriteLine(item.Name);
//    if (!string.IsNullOrEmpty(item.Id))
//        Console.WriteLine($"    id=\"{item.Id}\"");

//    if (item.Classes.Count > 0)
//        Console.WriteLine($"    class=\"{string.Join(" ", item.Classes)}\"");

//    if (item.Attributes.Count > 0)
//        Console.WriteLine($"    {string.Join(" ", item.Attributes.Select(attr => $"{attr.Item1}=\"{attr.Item2}\""))}");

//    if (!string.IsNullOrEmpty(item.InnerHtml))
//        Console.WriteLine($"    {item.InnerHtml}");
//}


//var htmlLines = new List<string>()
//{
//    { "!DOCTYPE html" },
//    { "html" },
//    { "div id=\"my-id\" class=\"my-class-1 my-class-2\" width=\"100%\" "},
//    { "div" },
//    { "p class=\"my-class-55\""},
//    { "/p" },
//    { "hello" },
//    { "div class=\"my-class-2\""},
//    { "/div" },
//    { "div class=\"my-class-2\"" },
//    { "/div" },
//    { "h1 class=\"my-class-1 my-class-2\"" },
//    { "banana"},
//    { "/h1" },
//    { "/div"},
//    { "/div"},
//    { "br class=\"my-class-1 my-class-2\"" },
//    { "/html"}
//};


//printTree(root);
//static void printTree(HtmlElement root)
//{
//    if (root.Name == null)
//        return;
//    Console.WriteLine(root.Name);
//    if (root.Children.Count() > 0)
//        foreach (var child in root.Children)
//        {
//            printTree(child);
//        }
//    else
//        return;
//}


//var z = a.ToList()[a.Count() - 1];
//Console.WriteLine("Ancestors of: " + z.Name);
//var zz = z.Ancestors();

//foreach (var item in zz)
//{
//    Console.WriteLine(item.Name);
//    if (!string.IsNullOrEmpty(item.Id))
//        Console.WriteLine($"    id=\"{item.Id}\"");

//    if (item.Classes.Count > 0)
//        Console.WriteLine($"    class=\"{string.Join(" ", item.Classes)}\"");

//    if (item.Attributes.Count > 0)
//        Console.WriteLine($"    {string.Join(" ", item.Attributes.Select(attr => $"{attr.Item1}=\"{attr.Item2}\""))}");

//    if (!string.IsNullOrEmpty(item.InnerHtml))
//        Console.WriteLine($"  {item.InnerHtml}");
//}


//example1
//var htmlElement = "<div id=\"my-id\" class=\"my-class-1 my-class-2\" width=\"100%\"> text </div>";

//var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(htmlElement).Select(match => Tuple.Create(match.Groups[1].Value, match.Groups[2].Value))
//                                       .ToList();

//example2

//var h = new HtmlElement("my-id", "div", attributes, new List<string>() { "bb", "bb" }, "text");

//var h2 = new HtmlElement("my-id", "br", attributes, new List<string>() { "bb", "bb" }, "text");
//h.AddChild(h2);

//Console.WriteLine(h.ToString());