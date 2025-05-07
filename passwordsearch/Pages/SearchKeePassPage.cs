using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using PasswordSearch.Commands;

namespace PasswordSearch.Pages
{
    public sealed partial class SearchKeePassPage : DynamicListPage
    {
        private readonly TextSetting _dbPathSetting;
        private readonly TextSetting _masterPwdSetting;
        private PwDatabase _database = null!;
        private List<IListItem> _items = new();

        public SearchKeePassPage(TextSetting dbPathSetting, TextSetting masterPwdSetting)
        {
            _dbPathSetting = dbPathSetting;
            _masterPwdSetting = masterPwdSetting;

            Icon = IconHelpers.FromRelativePath("Assets\\KeyIcon.png");
            Title = "Search in KeePass database";
            Name = "Search in KeePass database";
            Icon = new IconInfo("\uE721");
        }

        public override IListItem[] GetItems() => _items.ToArray();

        public override void UpdateSearchText(string oldSearch, string newSearch)
        {
            if (_database == null)
            {
                string dbPath = _dbPathSetting.Value;
                string masterPwd = _masterPwdSetting.Value;

                try
                {
                    if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
                        throw new FileNotFoundException($"Bestand niet gevonden: {dbPath}");

                    var ioInfo = new IOConnectionInfo { Path = dbPath };
                    var compositeKey = new CompositeKey();
                    if (!string.IsNullOrEmpty(masterPwd))
                        compositeKey.AddUserKey(new KcpPassword(masterPwd));

                    _database = new PwDatabase();
                    _database.Open(ioInfo, compositeKey, new NullStatusLogger());
                }
                catch (Exception ex)
                {
                    _items = new List<IListItem>
                    {
                        new ListItem(new NoOpCommand())
                        {
                            Title    = $"Error opening DB: {ex.GetType().Name}: {ex.Message}",
                            Subtitle = ex.StackTrace
                        }
                    };
                    RaiseItemsChanged();
                    IsLoading = false;
                    return;
                }
            }

            IsLoading = true;
            Task.Run(() =>
            {
                var all = _database.RootGroup.GetEntries(true);
                var matches = string.IsNullOrWhiteSpace(newSearch)
                    ? all
                    : all.Where(e =>
                        e.Strings.ReadSafe("Title")
                         .Contains(newSearch, StringComparison.OrdinalIgnoreCase));

                _items = matches.Select(entry =>
                {
                    var title = entry.Strings.ReadSafe("Title");
                    var user = entry.Strings.ReadSafe("UserName");
                    var pwd = entry.Strings.ReadSafe("Password");
                    var entryUrl = entry.Strings.ReadSafe("URL");

                    var copyPassword = new CopyTextCommand(pwd)
                    {
                        Name = "Copy Password"
                    };

                    var listItem = new ListItem(copyPassword)
                    {
                        Title = title,
                        Subtitle = string.IsNullOrEmpty(user) ? null : $"User: {user}"
                    };

                    listItem.MoreCommands = new IContextItem[]
                    {
                        new CommandContextItem(new CopyTextCommand(user)
                        {
                            Name = "Copy username"
                        })
                        {
                            Title = "Copy username"
                        },

                        new CommandContextItem(new OpenUrlCommand(entryUrl))
                        {
                            Title = "Open URL"
                        },
                        new CommandContextItem(new OpenInKeePass(_dbPathSetting.Value))
                        {
                            Title = "Open in KeePass"
                        }
                    };

                    return (IListItem)listItem;
                }).ToList();

                if (_items.Count == 0)
                {
                    _items.Add(new ListItem(new NoOpCommand())
                    {
                        Title = "No entries found"
                    });
                }

                RaiseItemsChanged();
                IsLoading = false;
            });
        }
    }
}
