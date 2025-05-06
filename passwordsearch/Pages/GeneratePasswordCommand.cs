// File: GeneratePasswordCommand.cs
using System;
using System.Security.Cryptography;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace PasswordSearch.Commands
{
    public class GeneratePasswordCommand : InvokableCommand
    {
        private readonly int _length;

        public GeneratePasswordCommand(int length = 16)
        {
            _length = length;
            Name = "Generate Password";
            // Segoe MDL2 “Key” glyph als icoon (of kies een andere)
            Icon = new IconInfo("\uE72E");
        }

        public override ICommandResult Invoke()
        {
            string pwd = Generate(_length);
            // CopyTextCommand zet direct in clipboard en returned Hide()
            return new CopyTextCommand(pwd).Invoke();
        }

        private static string Generate(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                 "abcdefghijklmnopqrstuvwxyz" +
                                 "0123456789" +
                                 "!@#$%^&*()-_=+[]{};:,.<>?";
            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = chars[data[i] % chars.Length];

            return new string(result);
        }
    }
}
