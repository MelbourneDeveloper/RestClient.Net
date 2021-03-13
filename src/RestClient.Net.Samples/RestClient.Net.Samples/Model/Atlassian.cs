using System.Collections.Generic;
using System.Runtime.Serialization;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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

    [DataContract]
    public class Watchers
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Branches
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Tags
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Commits
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Clone
    {
        [DataMember]
        public string href { get; set; }
        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class Self
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Html
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Avatar
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Hooks
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Forks
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Downloads
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Pullrequests
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Links
    {
        [DataMember]
        public Watchers watchers { get; set; }
        [DataMember]
        public Branches branches { get; set; }
        [DataMember]
        public Tags tags { get; set; }
        [DataMember]
        public Commits commits { get; set; }
        [DataMember]
        public List<Clone> clone { get; set; }
        [DataMember]
        public Self self { get; set; }
        [DataMember]
        public Html html { get; set; }
        [DataMember]
        public Avatar avatar { get; set; }
        [DataMember]
        public Hooks hooks { get; set; }
        [DataMember]
        public Forks forks { get; set; }
        [DataMember]
        public Downloads downloads { get; set; }
        [DataMember]
        public Pullrequests pullrequests { get; set; }
    }

    [DataContract]
    public class Self2
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Html2
    {
        [DataMember]
        public string href { get; set; }
    }

    public class Avatar2
    {
        [DataMember]
        public string href { get; set; }
    }

    [DataContract]
    public class Links2
    {
        [DataMember]
        public Self2 self { get; set; }
        [DataMember]
        public Html2 html { get; set; }
        [DataMember]
        public Avatar2 avatar { get; set; }
    }

    [DataContract]
    public class Owner
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string display_name { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string uuid { get; set; }
        [DataMember]
        public Links2 links { get; set; }
    }

    [DataContract]
    public class Repository
    {
        [DataMember]
        public string scm { get; set; }
        [DataMember]
        public string website { get; set; }
        [DataMember]
        public bool has_wiki { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Links links { get; set; }
        [DataMember]
        public string fork_policy { get; set; }
        [DataMember]
        public string uuid { get; set; }
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public string created_on { get; set; }
        [DataMember]
        public string full_name { get; set; }
        [DataMember]
        public bool has_issues { get; set; }
        [DataMember]
        public Owner owner { get; set; }
        [DataMember]
        public string updated_on { get; set; }
        [DataMember]
        public int size { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public bool is_private { get; set; }
        [DataMember]
        public string description { get; set; }
    }

    [DataContract]
    public class RepositoryList
    {
        [DataMember]
        public int pagelen { get; set; }
        [DataMember]
        public List<Repository> values { get; set; }
        [DataMember]
        public int page { get; set; }
        [DataMember]
        public int size { get; set; }
    }

    #endregion

    #region Errors
    public class Error
    {
        public string message { get; set; }
    }

    public class ErrorModel
    {
        public Error error { get; set; }
    }
    #endregion
}
