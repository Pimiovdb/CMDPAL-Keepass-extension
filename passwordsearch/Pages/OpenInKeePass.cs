using System.Diagnostics;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace PasswordSearch.Commands
{
    public class OpenInKeePass : InvokableCommand
    {
        private readonly string _dbPath;

        public OpenInKeePass(string dbPath)
        {
            _dbPath = dbPath;
            Name = "Open in KeePass";
            Icon = new IconInfo("\uE8F6");
        }

        public override ICommandResult Invoke()
        {
            Process.Start(new ProcessStartInfo(_dbPath) { UseShellExecute = true });

            return CommandResult.Hide();
        }
    }
}
