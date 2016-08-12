using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassian
{
    public class Watchers
    {
        public string href { get; set; }
    }

    public class Branches
    {
        public string href { get; set; }
    }

    public class Tags
    {
        public string href { get; set; }
    }

    public class Commits
    {
        public string href { get; set; }
    }

    public class Clone
    {
        public string href { get; set; }
        public string name { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Html
    {
        public string href { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Hooks
    {
        public string href { get; set; }
    }

    public class Forks
    {
        public string href { get; set; }
    }

    public class Downloads
    {
        public string href { get; set; }
    }

    public class Pullrequests
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Watchers watchers { get; set; }
        public Branches branches { get; set; }
        public Tags tags { get; set; }
        public Commits commits { get; set; }
        public List<Clone> clone { get; set; }
        public Self self { get; set; }
        public Html html { get; set; }
        public Avatar avatar { get; set; }
        public Hooks hooks { get; set; }
        public Forks forks { get; set; }
        public Downloads downloads { get; set; }
        public Pullrequests pullrequests { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Html2
    {
        public string href { get; set; }
    }

    public class Avatar2
    {
        public string href { get; set; }
    }

    public class Links2
    {
        public Self2 self { get; set; }
        public Html2 html { get; set; }
        public Avatar2 avatar { get; set; }
    }

    public class Project
    {
        public string key { get; set; }
        public string type { get; set; }
        public string uuid { get; set; }
        public Links2 links { get; set; }
        public string name { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Html3
    {
        public string href { get; set; }
    }

    public class Avatar3
    {
        public string href { get; set; }
    }

    public class Links3
    {
        public Self3 self { get; set; }
        public Html3 html { get; set; }
        public Avatar3 avatar { get; set; }
    }

    public class Owner
    {
        public string username { get; set; }
        public string display_name { get; set; }
        public string type { get; set; }
        public string uuid { get; set; }
        public Links3 links { get; set; }
    }

    public class RootObject
    {
        public string scm { get; set; }
        public string website { get; set; }
        public bool has_wiki { get; set; }
        public string name { get; set; }
        public Links links { get; set; }
        public string fork_policy { get; set; }
        public string uuid { get; set; }
        public Project project { get; set; }
        public string language { get; set; }
        public string created_on { get; set; }
        public string full_name { get; set; }
        public bool has_issues { get; set; }
        public Owner owner { get; set; }
        public string updated_on { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public bool is_private { get; set; }
        public string description { get; set; }
    }
}
