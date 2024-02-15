using Html_Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Tuple<string, string>> Attributes { get; set; }
    public List<string> Classes { get; set; }
    public string InnerHtml { get; set; }
    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; }

    public HtmlElement(string id = null, string name = null, List<Tuple<string, string>> attributes = null,
                       List<string> classes = null, string innerHtml = null)
    {
        Id = id;
        Name = name;
        Attributes = attributes ?? new List<Tuple<string, string>>();
        Classes = classes ?? new List<string>();
        InnerHtml = innerHtml;
        Parent = null;
        Children = new List<HtmlElement>();
    }

    public void AddChild(HtmlElement childElement)
    {
        childElement.Parent = this;
        Children.Add(childElement);
    }

    public IEnumerable<HtmlElement> Descendants()
    {
        Queue<HtmlElement> queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            HtmlElement element = queue.Dequeue();
            yield return element;

            if (element.Children.Count() > 0)
                foreach (var child in element.Children)
                    queue.Enqueue(child);
        }
    }

    public IEnumerable<HtmlElement> Ancestors()
    {
        HtmlElement temp = this;

        while (temp.Parent != null)
        {
            yield return temp.Parent;
            temp = temp.Parent;
        }
    }

    public static List<HtmlElement> findElementsBySelector(Selector selector, HtmlElement htmlElement)
    {
        HashSet<HtmlElement> answer = new HashSet<HtmlElement>();
        help(selector, htmlElement, answer);
        return answer.ToList();
    }

    private static void help(Selector selector, HtmlElement htmlElement, HashSet<HtmlElement> answer)
    {
        var allDescendants = htmlElement.Descendants();

        foreach (var child in allDescendants)
        {
            if ((selector.TagName == null || selector.TagName == child.Name) && (selector.Id == null || selector.Id == child.Id))
            {
                bool flag = true;
                if (selector.Classes.Count() != 0)
                    foreach (var c in selector.Classes)
                    {
                        bool flag2 = false;
                        foreach (var c2 in child.Classes)
                        {
                            if (c == c2)
                                flag2 = true;
                        }
                        if (flag2 == false)
                        {
                            flag = false;
                            break;
                        }
                    }
                if (flag != false)
                {
                    allDescendants = child.Descendants();
                    if (selector.Child == null)
                    {
                        answer.Add(child);
                        foreach (var descendant in allDescendants)
                        {
                            if (descendant != child)
                                help(selector, descendant, answer);
                        }
                    }
                    else
                        foreach (var descendant in allDescendants)
                        {
                            if (descendant != child)
                                help(selector.Child, descendant, answer);
                        }
                }
            }
        }
    }

    public override string ToString()
    {
        string result = $"<{Name}";

        if (!string.IsNullOrEmpty(Id))
            result += $" id=\"{Id}\"";

        if (Attributes.Count > 0)
            result += $" {string.Join(" ", Attributes.Select(attr => $"{attr.Item1}=\"{attr.Item2}\""))}";

        if (Classes.Count > 0)
            result += $" class=\"{string.Join(" ", Classes)}\"";

        //if the tags is not close
        foreach (var message in HtmlHelper.Instance.SelfClosingTags)
            if (message == Name)
                return result + '>';

        result += ">";

        if (!string.IsNullOrEmpty(InnerHtml))
            result += $"\n  {InnerHtml}\n";

        foreach (var child in Children)
            result += $"  {child.ToString()}\n";

        result += $"</{Name}>";

        return result;
    }

    public static HtmlElement Serialize(List<string> htmlLines)
    {
        var root = new HtmlElement();
        var current = new HtmlElement();
        current = root;

        foreach (var line in htmlLines)
        {
            Regex regex = new Regex(@"(?:^|\s)(\S+)\b");
            var first = regex.Match(line);

            if (first.Value == "!DOCTYPE")
                continue;

            if (first.Value.StartsWith('/'))
                current = current.Parent;

            else
            {
                bool flag2 = false;
                foreach (var message in HtmlHelper.Instance.AllHtmlTags)
                    if (message == first.Value)
                    {
                        flag2 = true;
                        var child = new HtmlElement();
                        child.Name = first.Value;
                        var allWordsInLine = Regex.Split(line, @"\s+");
                        foreach (var word in allWordsInLine)
                        {
                            var classes = new List<string>();
                            var allAttributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line).Select(match => Tuple.Create(match.Groups[1].Value, match.Groups[2].Value))
                                               .ToList();
                            foreach (var attribute in allAttributes)
                            {
                                if (attribute.Item1 == "class")
                                {
                                    var allClasses = Regex.Split(attribute.Item2, @"\s+");
                                    foreach (var c in allClasses)
                                        classes.Add(c);
                                }
                                else
                                    if (attribute.Item1 == "id")
                                    child.Id = attribute.Item2;
                            }
                            allAttributes = allAttributes.Where(attribute => !(attribute.Item1 == "class" || attribute.Item1 == "id")).ToList();
                            child.Classes = classes;
                            child.Attributes = allAttributes;
                        }
                        current.AddChild(child);

                        bool flag = false;
                        if (message.EndsWith('/'))
                            flag = true;
                        foreach (var m in HtmlHelper.Instance.SelfClosingTags)
                            if (m == first.Value)
                            {
                                flag = true;
                                break;
                            }
                        if (flag == false)
                            current = child;
                    }
                if (flag2 == false)
                    current.InnerHtml = first.Value;
            }
            if (first.Value == "/html")
                return root;
        }
        return root;
    }
}

