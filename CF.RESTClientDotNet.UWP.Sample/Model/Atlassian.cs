using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlassian
{
    #region Issue
    public class ReportedBy
    {
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string display_name { get; set; }
        public bool is_staff { get; set; }
        public string avatar { get; set; }
        public string resource_uri { get; set; }
        public bool is_team { get; set; }
    }

    public class Metadata
    {
        public string kind { get; set; }
        public object version { get; set; }
        public object component { get; set; }
        public object milestone { get; set; }
    }

    public class Issue
    {
        public string status { get; set; }
        public string priority { get; set; }
        public string title { get; set; }
        public ReportedBy reported_by { get; set; }
        public string utc_last_updated { get; set; }
        public string created_on { get; set; }
        public Metadata metadata { get; set; }
        public string content { get; set; }
        public int comment_count { get; set; }
        public int local_id { get; set; }
        public int follower_count { get; set; }
        public string utc_created_on { get; set; }
        public string resource_uri { get; set; }
        public bool is_spam { get; set; }
    }
    #endregion

    #region Repo

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

    public class Owner
    {
        public string username { get; set; }
        public string display_name { get; set; }
        public string type { get; set; }
        public string uuid { get; set; }
        public Links2 links { get; set; }
    }

    public class Repository
    {
        public string scm { get; set; }
        public string website { get; set; }
        public bool has_wiki { get; set; }
        public string name { get; set; }
        public Links links { get; set; }
        public string fork_policy { get; set; }
        public string uuid { get; set; }
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

    public class RepositoryList
    {
        public int pagelen { get; set; }
        public List<Repository> values { get; set; }
        public int page { get; set; }
        public int size { get; set; }
    }

    #endregion

}
