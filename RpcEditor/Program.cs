using DiscordRPC;
using System;
using Terminal.Gui;

namespace RpcEditor
{
    public class Program
    {
        // Discord
        private DiscordRpcClient _client;

        // Views
        private readonly View _mainView;
        private readonly View _currentPresenceView;
        private readonly View _currentStateView;
        private readonly View _connectionView;
        private readonly View _editPresenceView;

        // Main view components
        private readonly Label _copyright;

        // Connection view
        private readonly TextField _conn_ApplicationId;
        private readonly Label _conn_ApplicationId_ErrorText;
        private bool _conn_ApplicationId_Errored;
        private readonly Button _conn_Connect;

        // State view
        private readonly Label _state;

        // Current presence view
        private readonly Label _currentPresence_Name;
        private readonly Label _currentPresence_State;
        private readonly Label _currentPresence_ArtworkLarge;
        private readonly Label _currentPresence_ArtworkSmall;
        private readonly Label _currentPresence_Timestamps;

        // Edit presence view
        private readonly TextField _editPresence_Name;
        private readonly TextField _editPresence_State;
        private readonly TextField _editPresence_ArtworkLarge;

        public Program()
        {
            Application.Init();

            // copyright
            _copyright = new Label("Copyright (c) 2019 Abyssal - https://github.com/abyssal/RpcEditor")
            {
                TextColor = Terminal.Gui.Attribute.Make(Color.BrighCyan, Color.Black)
            };

            #region Main view

            _mainView = new FrameView("Discord Rich Presence Editor")
            {
                X = 0,
                Y = 1, // Leave one row for the copyright label

                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            #endregion

            #region Connection view

            _connectionView = new FrameView("Connection")
            {
                Height = 5
            };

            var idLabel = new Label("Discord application ID: ");

            _conn_ApplicationId = new TextField("")
            {
                X = Pos.Right(idLabel),
                Width = 40
            };

            _conn_ApplicationId.Changed += ApplicationIdInput_Changed;

            _conn_ApplicationId_ErrorText = new Label("Invalid application ID")
            {
                X = Pos.Right(_conn_ApplicationId),
                Y = Pos.Top(_conn_ApplicationId),
                Width = 25,
                TextColor = Terminal.Gui.Attribute.Make(Color.Red, Color.Black)
            };

            _conn_Connect = new Button("Connect")
            {
                X = 0,
                Y = Pos.Bottom(idLabel),

                Width = 10
            };
            _conn_Connect.Clicked += Connect_Clicked;

            _connectionView.Add(idLabel, _conn_ApplicationId, _conn_ApplicationId_ErrorText, _conn_Connect);

            #endregion
            #region State view

            _currentStateView = new FrameView("State")
            {
                X = 0,
                Y = Pos.Bottom(_connectionView),
                Height = 4
            };

            _state = new Label("Not connected")
            {
                X = 2
            };

            _currentStateView.Add(_state);

            #endregion
            #region Edit presence view

            _editPresenceView = new FrameView("Edit Presence")
            {
                X = 0,
                Y = Pos.Bottom(_currentStateView),
                Width = 50,
                Height = Dim.Fill()
            };

            var editPresenceNameLabel = new Label("Name: ");

            _editPresence_Name = new TextField("")
            {
                X = Pos.Right(editPresenceNameLabel),
                Y = 0,
                Width = 30
            };

            var editPresenceStateLabel = new Label("State: ")
            {
                X = 0,
                Y = Pos.Bottom(editPresenceNameLabel)
            };

            _editPresence_State = new TextField("")
            {
                X = Pos.Right(editPresenceStateLabel),
                Y = Pos.Bottom(editPresenceNameLabel),
                Width = 30
            };

            var editPresenceLargeKey = new Label("Artwork large key: ")
            {
                X = 0,
                Y = Pos.Bottom(editPresenceStateLabel)
            };

            _editPresence_ArtworkLarge = new TextField("")
            {
                X = Pos.Right(editPresenceLargeKey),
                Y = Pos.Bottom(editPresenceStateLabel),
                Width = 10
            };

            var updatePresence = new Button("Update")
            {
                X = 0,
                Y = Pos.Bottom(editPresenceLargeKey)
            };

            var clearPresence = new Button("Clear")
            {
                X = Pos.Right(updatePresence),
                Y = Pos.Bottom(editPresenceLargeKey)
            };

            updatePresence.Clicked += UpdatePresence_Clicked;

            clearPresence.Clicked += ClearPresence_Clicked;

            _editPresenceView.Add(editPresenceNameLabel, _editPresence_Name, editPresenceStateLabel, _editPresence_State, editPresenceLargeKey, _editPresence_ArtworkLarge, updatePresence, clearPresence);

            #endregion
            #region Current presence view

            _currentPresenceView = new FrameView("Current Presence")
            {
                X = Pos.Right(_editPresenceView),
                Y = Pos.Bottom(_currentStateView),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var presenceLabel = new Label("Presence: ");
            var stateLabel = new Label("State: ")
            {
                Y = Pos.Bottom(presenceLabel)
            };
            var artworkLargeLabel = new Label("Large artwork ID: ")
            {
                Y = Pos.Bottom(stateLabel)
            };
            var artworkSmallLabel = new Label("Small artwork ID: ")
            {
                Y = Pos.Bottom(artworkLargeLabel)
            };
            var timestampLabel = new Label("Timestamps: ")
            {
                Y = Pos.Bottom(artworkSmallLabel)
            };

            _currentPresence_Name = new Label("No presence set.")
            {
                X = Pos.Right(presenceLabel)
            };
            _currentPresence_State = new Label("No state set.") 
            { 
                Y = Pos.Bottom(_currentPresence_Name), 
                X = Pos.Right(stateLabel)
            };
            _currentPresence_ArtworkLarge = new Label("No large artwork set.") 
            { 
                Y = Pos.Bottom(_currentPresence_State),
                X = Pos.Right(artworkLargeLabel)
            };
            _currentPresence_ArtworkSmall = new Label("No small artwork set.")
            {
                Y = Pos.Bottom(_currentPresence_ArtworkLarge),
                X = Pos.Right(artworkSmallLabel)
            };
            _currentPresence_Timestamps = new Label("No timestamps set.")
            { 
                Y = Pos.Bottom(_currentPresence_ArtworkSmall),
                X = Pos.Right(timestampLabel)
            };

            _currentPresenceView.Add(_currentPresence_Name, _currentPresence_State, _currentPresence_ArtworkLarge, _currentPresence_ArtworkSmall, _currentPresence_Timestamps, artworkLargeLabel, artworkSmallLabel, presenceLabel, stateLabel, timestampLabel);

            #endregion

            // add all views
            _mainView.Add(_connectionView, _editPresenceView, _currentPresenceView, _currentStateView);

            var top = Application.Top;
            top.Add(_copyright);
            top.Add(_mainView);
        }

        public static void Main(string[] _0)
        {
            _ = new Program();
            Application.Run();
        }

        private void ApplicationIdInput_Changed(object sender, EventArgs e)
        {
            _conn_ApplicationId_Errored = !ulong.TryParse(_conn_ApplicationId.Text.ToString(), out _);
            if (_conn_ApplicationId_Errored)
            {
                _conn_ApplicationId_ErrorText.Text = "Invalid application ID";
            } else
            {
                _conn_ApplicationId_ErrorText.Text = "";
            }

        }

        private void Connect_Clicked()
        {
            if (_conn_ApplicationId_Errored) return;

            UpdateLabel(_state, "Connecting...");

            if (_client != null)
            {
                _client.OnReady -= Client_OnReady;
                _client.OnConnectionFailed -= Client_OnConnectionFailed;
                _client.OnConnectionEstablished -= Client_OnConnectionEstablished;
                _client.OnError -= Client_OnError;
                _client.OnClose -= Client_OnClose;
                _client.OnPresenceUpdate -= Client_OnPresenceUpdate;
                _client = null;
            }

            _client = new DiscordRpcClient(_conn_ApplicationId.Text.ToString());
            _client.Initialize();

            _client.OnReady += Client_OnReady;
            _client.OnConnectionFailed += Client_OnConnectionFailed;
            _client.OnConnectionEstablished += Client_OnConnectionEstablished;
            _client.OnError += Client_OnError;
            _client.OnClose += Client_OnClose;
            _client.OnPresenceUpdate += Client_OnPresenceUpdate;
        }

        private void Client_OnClose(object sender, DiscordRPC.Message.CloseMessage args)
        {
            UpdateLabel(_state, $"Connection closed: {args.Code} {args.Reason}");
        }

        private void Client_OnError(object sender, DiscordRPC.Message.ErrorMessage args)
        {
            UpdateLabel(_state, $"Error: {args.Code} {args.Message}");
        }

        private void Client_OnConnectionEstablished(object sender, DiscordRPC.Message.ConnectionEstablishedMessage args)
        {
            UpdateLabel(_state, "Established connection...");
        }

        private void Client_OnConnectionFailed(object sender, DiscordRPC.Message.ConnectionFailedMessage args)
        {
            UpdateLabel(_state, $"Failed to connect to pipe {args.FailedPipe}. Is Discord open?");
        }

        private void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
        {
            // don't call updatelabel because we're updating many labels at once
            _currentPresence_Name.Text = _client.CurrentPresence?.Details ?? "No presence set.";
            _currentPresence_State.Text = _client.CurrentPresence?.State ?? "No state set.";
            _currentPresence_ArtworkLarge.Text = _client.CurrentPresence?.Assets?.LargeImageKey ?? "No large artwork set.";
            _currentPresence_ArtworkSmall.Text = _client.CurrentPresence?.Assets?.SmallImageKey ?? "No small artwork set.";
            _currentPresence_Timestamps.Text = _client.CurrentPresence?.HasTimestamps() == true ? $"{_client.CurrentPresence.Timestamps.Start:F} - {_client.CurrentPresence.Timestamps.End:F}" : "No timestamps set.";

            Application.Refresh();
        }

        private void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            UpdateLabel(_state, $"Ready. Connected as {args.User}.");
        }

        private void UpdatePresence_Clicked()
        {
            var presence = new RichPresence
            {
                Details = _editPresence_Name.Text.ToString(),
                State = _editPresence_State.Text.ToString()
            };

            if (!_editPresence_ArtworkLarge.Text.IsEmpty)
            {
                presence.Assets = new Assets
                {
                    LargeImageKey = _editPresence_ArtworkLarge.Text.ToString()
                };
            };

            _client.SetPresence(presence);
        }

        private void ClearPresence_Clicked()
        {
            _client.ClearPresence();
        }

        private void UpdateLabel(Label label, string newText)
        {
            label.Clear();
            label.Text = newText;
            Application.Refresh();
        }
    }
}
