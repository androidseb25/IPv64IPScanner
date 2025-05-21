namespace IPv64IPScanner;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ResultDS
    {
        public List<object> Error { get; set; }
        public List<SubAction> SubActions { get; set; }
        public List<Transcriptt> Transcript { get; set; }
        public List<object> Information { get; set; }
    }

    public class MXIPResponse
    {
        public string Command { get; set; }
        public bool SupportTransitions { get; set; }
        public List<int> ListedBlacklists { get; set; }
        public string IgnoredSubactions { get; set; }
        public string CommandArgument { get; set; }
        public bool IsKeeper { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool HasDbAccess { get; set; }
        public string AllSubActionTested { get; set; }
        public object DnsAnswer { get; set; }
        public object DnsAnswerPrevious { get; set; }
        public bool DnsChanged { get; set; }
        public bool HasAlertBeenSent { get; set; }
        public DateTime TimeRecorded { get; set; }
        public object LookupActionResultID { get; set; }
        public object LookupActionResultUID { get; set; }
        public bool HasSubscriptions { get; set; }
        public SubActionsState SubActionsState { get; set; }
        public string RaisedSubactions { get; set; }
        public object RelatedDomainName { get; set; }
        public object RelatedIP { get; set; }
        public object ReportingNameServer { get; set; }
        public ResultDS ResultDS { get; set; }
        public List<object> AllAnswers { get; set; }
        public int TimeToComplete { get; set; }
        public int MxRep { get; set; }
    }

    public class SubAction
    {
        public string SubActionID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public string TTL { get; set; }
        public string ResponseTime { get; set; }
        public string ReasonForListing { get; set; }
        public string IsRHSBL { get; set; }
        public string DelistUrl { get; set; }
    }

    public class SubActionsState
    {
        public string _priorSubActions { get; set; }
        public string _currentSubActionState { get; set; }
        public bool IsFirstTimeEverRun { get; set; }
        public string IgnoredSubActions { get; set; }
    }

    public class Transcriptt
    {
        public string Transcript { get; set; }
    }