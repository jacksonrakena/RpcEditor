using DiscordRPC;
using System;
using System.Diagnostics;
using Terminal.Gui;

namespace RpcEditor
{
    public class Program
    {
        private DiscordRpcClient _client;

        private readonly Label _copyright;

        private readonly View _mainView;
        private readonly View _currentPresenceView;
        private readonly View _currentStateView;
        private readonly View _authView;

        private readonly Label _applicationId;
        private readonly TextField _applicationId_Input;
        private readonly Label _applicationId_Error;
        private bool _appId_Errored = false;

        private readonly Label _state;
        private readonly Label _currentPresence_Name;

        private readonly Button _connect;

        private ulong _appId;

        public Program()
        {
            Application.Init();

            // copyright
            _copyright = new Label("Copyright (c) 2019 Abyssal - https://github.com/abyssal/RpcEditor")
            {
                TextColor = Terminal.Gui.Attribute.Make(Color.BrighCyan, Color.Black)
            };

            // main view

            _mainView = new FrameView("Discord Rich Presence Editor")
            {
                X = 0,
                Y = 1, // Leave one row for the copyright label

                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            // authentication window

            _authView = new FrameView("Connection")
            {
                Height = 5
            };

            _applicationId = new Label("Discord application ID: ")
            {
                X = 0,
                Y = 0
            };

            _applicationId_Input = new TextField("")
            {
                X = Pos.Right(_applicationId),
                Y = Pos.Top(_applicationId),
                Width = 40
            };

            _applicationId_Input.Changed += ApplicationIdInput_Changed;

            _applicationId_Error = new Label("Invalid application ID")
            {
                X = Pos.Right(_applicationId_Input),
                Y = Pos.Top(_applicationId_Input),
                Width = 25,
                TextColor = Terminal.Gui.Attribute.Make(Color.Red, Color.Black)
            };

            _connect = new Button("Connect")
            {
                X = 0,
                Y = Pos.Bottom(_applicationId),

                Width = 10
            };
            _connect.Clicked += Connect_Clicked;

            _authView.Add(_applicationId, _applicationId_Input, _applicationId_Error, _connect);

            // state window

            _currentStateView = new FrameView("State")
            {
                X = 0,
                Y = Pos.Bottom(_authView),
                Height = 4
            };

            _state = new Label("Not connected")
            {
                X = 2
            };

            _currentStateView.Add(_state);

            // edit presence window

            _editPresenceView = new FrameView("Edit Presence")
            {
                X = 0,
                Y = Pos.Bottom(_currentStateView),
                Width = 50,
                Height = Dim.Fill()
            };

            _enterPresenceName = new Label("Name: ");

            _enterPresenceName_Input = new TextField("")
            {
                X = Pos.Right(_enterPresenceName),
                Y = 0,
                Width = 30
            };

            _enterPresenceState = new Label("State: ")
            {
                X = 0,
                Y = Pos.Bottom(_enterPresenceName)
            };

            _enterPresenceState_Input = new TextField("")
            {
                X = Pos.Right(_enterPresenceState),
                Y = Pos.Bottom(_enterPresenceName),
                Width = 30
            };

            _enterPresenceLargeKey = new Label("Artwork large key: ")
            {
                X = 0,
                Y = Pos.Bottom(_enterPresenceState)
            };

            _enterPresenceLargeKey_Input = new TextField("")
            {
                X = Pos.Right(_enterPresenceLargeKey),
                Y = Pos.Bottom(_enterPresenceState),
                Width = 10
            };

            _sendPresence = new Button("Update")
            {
                X = 0,
                Y = Pos.Bottom(_enterPresenceLargeKey)
            };

            _clearPresence = new Button("Clear")
            {
                X = Pos.Right(_sendPresence),
                Y = Pos.Bottom(_enterPresenceLargeKey)
            };

            _sendPresence.Clicked += UpdatePresence_Clicked;

            _clearPresence.Clicked += ClearPresence_Clicked;

            _editPresenceView.Add(_enterPresenceName, _enterPresenceName_Input, _enterPresenceState, _enterPresenceState_Input, _enterPresenceLargeKey, _enterPresenceLargeKey_Input, _sendPresence, _clearPresence);

            // current presence window

            _currentPresenceView = new FrameView("Current Presence")
            {
                X = Pos.Right(_editPresenceView),
                Y = Pos.Bottom(_currentStateView),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            _currentPresence_Name = new Label("No presence set.");

            _currentPresenceView.Add(_currentPresence_Name);

            // add all views
            _mainView.Add(_authView, _editPresenceView, _currentPresenceView, _currentStateView);

            var top = Application.Top;
            top.Add(_copyright);
            top.Add(_mainView);
        }

        public static void Main(string[] args)
        {
            var p = new Program();
            Application.Run();
        }

        private void ApplicationIdInput_Changed(object sender, EventArgs e)
        {
            _appId_Errored = !ulong.TryParse(_applicationId_Input.Text.ToString(), out _appId);
            if (_appId_Errored)
            {
                _applicationId_Error.Text = "Invalid application ID";
            } else
            {
                _applicationId_Error.Text = "";
            }

        }

        private void Connect_Clicked()
        {
            if (_appId_Errored) return;
            _client = new DiscordRpcClient(_appId.ToString());
            _client.Initialize();

            _client.OnReady += Client_OnReady;
            _client.OnPresenceUpdate += Client_OnPresenceUpdate;
        }

        private void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
        {
            _currentPresence_Name.Text = _client.CurrentPresence?.Details ?? "No presence set.";
            Application.Refresh();
        }

        private readonly View _editPresenceView;
        private readonly Label _enterPresenceName = new Label("Name: ");
        private readonly TextField _enterPresenceName_Input;

        private readonly Label _enterPresenceState;
        private readonly TextField _enterPresenceState_Input;

        private readonly Label _enterPresenceLargeKey;
        private readonly TextField _enterPresenceLargeKey_Input;

        private readonly Button _sendPresence;
        private readonly Button _clearPresence;

        private void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            _state.Text = $"Ready. Connected as {args.User}.";
            Application.Refresh();
        }

        private void UpdatePresence_Clicked()
        {
            var presence = new RichPresence
            {
                Details = _enterPresenceName_Input.Text.ToString(),
                State = _enterPresenceState_Input.Text.ToString()
            };

            if (!_enterPresenceLargeKey_Input.Text.IsEmpty)
            {
                presence.Assets = new Assets
                {
                    LargeImageKey = _enterPresenceLargeKey_Input.Text.ToString()
                };
            };

            _client.SetPresence(presence);
        }

        private void ClearPresence_Clicked()
        {
            _client.ClearPresence();
        }
    }
}
