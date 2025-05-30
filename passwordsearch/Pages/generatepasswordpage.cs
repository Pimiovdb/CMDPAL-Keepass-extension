﻿using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace GeneratePasswordExtension.Pages
{
    public sealed partial class GeneratePasswordPage : ContentPage
    {
        private readonly GeneratePasswordForm _form = new();

        public GeneratePasswordPage()
        {
            Name = "GeneratePassword";
            Title = "Generate Password";
            Icon = new IconInfo("\uE8C8");
        }

        public override IContent[] GetContent()
            => new IContent[] { _form };
    }

    internal sealed partial class GeneratePasswordForm : FormContent
    {
        public GeneratePasswordForm()
        {
            TemplateJson = $$"""
            {
                "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
                "type": "AdaptiveCard",
                "version": "1.6",
                "body": [
                    {
                        "type": "Input.Number",
                        "id": "passwordLength",
                        "label": "Password Length",
                        "style": "text",
                        "isRequired": true,
                        "errorMessage": "Enter a Number Between 6 and 64",
                        "value": 12,
                        "max": 64
                    },
                    {
                        "type": "Input.ChoiceSet",
                        "id": "characterTypes",
                        "style": "expanded",
                        "isMultiSelect": true,
                        "wrap": false,
                        "value": "upper,lower,digits,symbold",
                        "choices": [
                            {
                                "title": "Hoofdletters (A-Z)",
                                "value": "upper"
                            },
                            {
                                "title": "Kleine letters (a-z)",
                                "value": "lower"
                            },
                            {
                                "title": "Cijfers (0-9)",
                                "value": "digits"
                            },
                            {
                                "title": "Symbolen (!@#$...)",
                                "value": "symbols"
                            }
                        ]
                    }
                ],
                "actions": [
                    {
                        "type": "Action.Submit",
                        "title": "Generate"
                    }
                ]
            }
            """;
        DataJson = $$"""
                {}
                """;
        }

        public override CommandResult SubmitForm(string payload)
        {
            var inputs = JsonNode.Parse(payload)?.AsObject();
            if (inputs == null)
                return CommandResult.GoHome();

            if (!int.TryParse(inputs["passwordLength"]?.GetValue<string>(), out int length) || length <= 6)
            {
                var errorMessage = new StatusMessage
                {
                    Message = "Invalid Length, Enter a Number Between 6 and 64",
                    State = MessageState.Error
                };

                ExtensionHost.ShowStatus(errorMessage, StatusContext.Page);
                return CommandResult.KeepOpen();
            }

            string? selected = inputs["characterTypes"]?.GetValue<string>();
            var types = selected?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            bool useUpper = Array.Exists(types, t => t == "upper");
            bool useLower = Array.Exists(types, t => t == "lower");
            bool useDigits = Array.Exists(types, t => t == "digits");
            bool useSymbols = Array.Exists(types, t => t == "symbols");

            if (!useUpper && !useLower && !useDigits && !useSymbols)
            {
                var errorMessage = new StatusMessage
                {
                    Message = "Select at least one character type.",
                    State = MessageState.Error
                };

                ExtensionHost.ShowStatus(errorMessage, StatusContext.Page);
                return CommandResult.KeepOpen();
            }

            var password = Generate(length, useUpper, useLower, useDigits, useSymbols);
            ClipboardHelper.SetText(password);

            var status = new StatusMessage
            {
                Message = $"🔒 Password generated: {password} (Copied to Clipboard)",
                State = MessageState.Success
            };
            ExtensionHost.ShowStatus(status, StatusContext.Page);

            return CommandResult.KeepOpen();
        }

        private static string Generate(int length, bool useUpper, bool useLower, bool useDigits, bool useSymbols)
        {
            var charSets = new List<string>();
            if (useUpper) charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (useLower) charSets.Add("abcdefghijklmnopqrstuvwxyz");
            if (useDigits) charSets.Add("0123456789");
            if (useSymbols) charSets.Add("!@#$%^&*()-_=+[]{};:,.<>?");

            var allChars = string.Concat(charSets);
            var data = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);

            var result = new char[length];
            for (int i = 0; i < length; i++)
                result[i] = allChars[data[i] % allChars.Length];

            return new string(result);
        }
    }
}
