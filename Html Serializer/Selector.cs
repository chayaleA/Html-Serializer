using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Html_Serializer
{
    internal class Selector
    {
        public string TagName { get; set; }

        public string Id { get; set; }

        public List<string> Classes { get; set; }

        public Selector Parent { get; set; }

        public Selector Child { get; set; }

        public Selector(string id = null, string tagName = null, List<string> classes = null)
        {
            Id = id;
            TagName = tagName;
            Classes = classes ?? new List<string>();
            Parent = null;
            Child = null;
        }

        public void AddChild(Selector childSelector)
        {
            childSelector.Parent = this;
            Child = childSelector;
        }

        public static Selector Convert(string qString)
        {
            Selector answer = new Selector();
            Selector current = new Selector();
            current = answer;
            var allWords = Regex.Split(qString, @"\s+");

            foreach (var word in allWords)
            {
                var wordContent = Regex.Split(word, "(?=[#.])");
                foreach (var selector in wordContent)
                {
                    if (selector.StartsWith("."))
                        current.Classes.Add(selector.Substring(1));
                    else
                        if (selector.StartsWith("#"))
                        current.Id = selector.Substring(1);
                    else
                    {
                        foreach (var tag in HtmlHelper.Instance.AllHtmlTags)
                        {
                            if (tag == selector)
                            {
                                current.TagName = tag;
                                break;
                            }
                        }
                    }
                }
                Selector child = new Selector();
                current.AddChild(child);
                current = child;
            }
            current.Parent.Child = null;
            return answer;
        }

        private static void printSelector(Selector selector)
        {
            if (selector == null)
                return;
            if (selector.TagName != null)
                Console.WriteLine("TagName: " + selector.TagName);
            if (selector.Id != null)
                Console.WriteLine("Id: " + selector.Id);
            if (selector.Classes.Count() > 0)
                selector.Classes.ForEach(item => Console.WriteLine("Class: " + item));
            if (selector.Child != null)
            {
                Console.WriteLine("child:");
                printSelector(selector.Child);
            }

        }
    }
}
