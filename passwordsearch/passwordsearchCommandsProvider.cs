using System.Runtime.InteropServices;
using GeneratePasswordExtension.Pages;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using PasswordSearch.Commands;
using PasswordSearch.Pages;

[ComVisible(true)]
public partial class PasswordSearchCommandsProvider : CommandProvider
{
    private readonly Settings _settings;
    private readonly TextSetting _dbPathSetting;
    private readonly TextSetting _masterPwdSetting;
    private readonly SearchKeePassPage _searchPage;
    private readonly GeneratePasswordPage _generatePage;

    public PasswordSearchCommandsProvider()
    {
        DisplayName = "Keepass Password Search";
        Icon = new IconInfo("\uE721");

        _settings = new Settings();
        _dbPathSetting = new TextSetting("DatabasePath", @"C:\Users\You\Documents\vault.kdbx");
        _masterPwdSetting = new TextSetting("MasterPassword", "Enter Your KeePassXC Masterpassword");
        _settings.Add(_dbPathSetting);
        _settings.Add(_masterPwdSetting);

        _searchPage = new SearchKeePassPage(_dbPathSetting, _masterPwdSetting);
        _generatePage = new GeneratePasswordPage();
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return new ICommandItem[]
        {
            new CommandItem(_settings.SettingsPage) { Title = "Configure KeePass DB" },
            new CommandItem(_searchPage){
                Title = "Search KeePass",
                Icon = new IconInfo("\uE721")
            },
            //new CommandItem(new GeneratePasswordCommand(16)){
            //    Title = "Generate Password Command",
            //    Icon  = new IconInfo("\uEAAE")
            //},
            new CommandItem(_generatePage){ 
                Title = "Generate Password",
                Icon = new IconInfo("\uEAAE")
            }
        };
    }
}
