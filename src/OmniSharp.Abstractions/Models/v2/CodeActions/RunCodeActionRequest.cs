using OmniSharp.Mef;
using System.Collections.Generic;

namespace OmniSharp.Models.V2.CodeActions
{
    [OmniSharpEndpoint(OmniSharpEndpoints.V2.RunCodeAction, typeof(RunCodeActionRequest), typeof(RunCodeActionResponse))]
    public class RunCodeActionRequest : Request, ICodeActionRequest
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public Range Selection { get; set; }
        public bool WantsTextChanges { get; set; }
        public bool ApplyTextChanges { get; set; } = true;
        public bool WantsAllCodeActionOperations { get; set; }
        public IDictionary<string, object> Params { get; set; }
    }
}
