using CommandLine;
using System;

namespace AOLicensing.KeyManager
{
    internal enum Action
    {
        Query,
        Create,
        Validate
    }

    internal class Options
    {
        [Option(shortName: 'a', longName: "action", Required = true)]
        public string ActionName { get; set; }

        [Option(shortName: 'p', longName: "product", Required = true)]
        public string Product { get; set; }

        [Option(shortName: 'e', longName: "email", Required = true)]
        public string Email { get; set; }

        [Option(shortName: 'k', longName: "key")]
        public string Key { get; set; }

        public Action Action
        {
            get =>
                (ActionName.Equals("query")) ? Action.Query :
                (ActionName.Equals("create")) ? Action.Create :
                (ActionName.Equals("validate")) ? Action.Validate :
                throw new ArgumentException(nameof(ActionName));
        }
    }
}
