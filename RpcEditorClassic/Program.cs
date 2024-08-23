﻿using DiscordRPC;
using System;
using RpcEditorClassic.Views;
using Terminal.Gui;

namespace RpcEditorClassic
{
    public class Program
    {
        // Discord
        private DiscordRpcClient _client;

        // Views
        private readonly View _currentPresenceView;
        private readonly View _currentStateView;
        private readonly View _connectionView;
        private readonly View _editPresenceView;


        // Connection view
        private readonly InputSetWithValidation _conn_ApplicationId;
        private readonly Button _conn_Connect;

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

            #region Connection view

            _connectionView = new FrameView("Connection")
            {
                Height = 5
            };

            _conn_ApplicationId = new InputSetWithValidation("Discord application ID: ", "Invalid application ID", "Valid", Utilities.IsDiscordId, 18);

            _conn_Connect = new Button("Connect")
            {
                X = 0,
                Y = Pos.Bottom(_conn_ApplicationId),

                Width = 10
            };
            _conn_Connect.Clicked += Connect_Clicked;

            _connectionView.Add(_conn_ApplicationId, _conn_Connect);

            #endregion
            #region State view

            _currentStateView = new FrameView("State")
            {
                X = 0,
                Y = Pos.Bottom(_connectionView),
                Height = 4
            }.WithSubviews(new Label("Not connected")
            {
                X = 2
            });

            #endregion
            #region Edit presence view

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

            var exit = new Button("Exit")
            {
                X = Pos.Right(clearPresence),
                Y = Pos.Bottom(editPresenceLargeKey)
            };

            updatePresence.Clicked += UpdatePresence_Clicked;

            clearPresence.Clicked += () => _client.ClearPresence();

            exit.Clicked += () => Environment.Exit(0);

            _editPresenceView = new FrameView("Edit Presence")
            {
                X = 0,
                Y = Pos.Bottom(_currentStateView),
                Width = 50,
                Height = Dim.Fill()
            }.WithSubviews(editPresenceNameLabel, _editPresence_Name, editPresenceStateLabel, _editPresence_State,
                _editPresence_State, editPresenceLargeKey, _editPresence_ArtworkLarge, updatePresence, clearPresence, exit);

            #endregion
            #region Current presence view

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
            _currentPresenceView = new FrameView("Current Presence")
            {
                X = Pos.Right(_editPresenceView),
                Y = Pos.Bottom(_currentStateView),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            }.WithSubviews(_currentPresence_Name, _currentPresence_State, _currentPresence_ArtworkLarge, _currentPresence_ArtworkSmall, _currentPresence_Timestamps, artworkLargeLabel, artworkSmallLabel, presenceLabel, stateLabel, timestampLabel);
            #endregion

            // add all views

            var top = Application.Top;

            top.Add(new Label("Copyright (c) 2019 Abyssal - https://github.com/abyssal/RpcEditor")
            {
                TextColor = Terminal.Gui.Attribute.Make(Color.BrighCyan, Color.Black)
            }, new FrameView("Discord Rich Presence Editor")
            {
                X = 0,
                Y = 1, // Leave one row for the copyright label

                Width = Dim.Fill(),
                Height = Dim.Fill()
            }.WithSubviews(_connectionView, _editPresenceView, _currentPresenceView, _currentStateView));
        }

        public static void Main(string[] _0)
        {
            _ = new Program();
            Application.Run();
        }

        private void SetStateText(string stateText)
        {
            ((Label) _currentStateView.Subviews[0].Subviews[0]).SetText(stateText);
        }

        private void Connect_Clicked()
        {
            if (!_conn_ApplicationId.IsValid()) return;
            SetStateText("Connecting...");

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

            _client = new DiscordRpcClient(_conn_ApplicationId.Value.Text.ToString());
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
            SetStateText($"Connection closed: {args.Code} {args.Reason}");
        }

        private void Client_OnError(object sender, DiscordRPC.Message.ErrorMessage args)
        {
            SetStateText($"Error: {args.Code} {args.Message}");
        }

        private void Client_OnConnectionEstablished(object sender, DiscordRPC.Message.ConnectionEstablishedMessage args)
        {
            SetStateText("Established connection...");
        }

        private void Client_OnConnectionFailed(object sender, DiscordRPC.Message.ConnectionFailedMessage args)
        {
            SetStateText($"Failed to connect to pipe {args.FailedPipe}. Is Discord open?");
        }

        private void Client_OnPresenceUpdate(object sender, DiscordRPC.Message.PresenceMessage args)
        {
            // call settext with false because we're setting many at once
            _currentPresence_Name.SetText(_client.CurrentPresence?.Details ?? "No presence set.", false);
            _currentPresence_State.SetText(_client.CurrentPresence?.State ?? "No state set.", false);
            _currentPresence_ArtworkLarge.SetText(_client.CurrentPresence?.Assets?.LargeImageKey ?? "No large artwork set.", false);
            _currentPresence_ArtworkSmall.SetText(_client.CurrentPresence?.Assets?.SmallImageKey ?? "No small artwork set.", false);
            _currentPresence_Timestamps.SetText(_client.CurrentPresence?.HasTimestamps() == true ? $"{_client.CurrentPresence.Timestamps.Start:F} - {_client.CurrentPresence.Timestamps.End:F}" : "No timestamps set.", false);

            Application.Refresh();
        }

        private void Client_OnReady(object sender, DiscordRPC.Message.ReadyMessage args)
        {
            SetStateText($"Ready. Connected as {args.User}.");
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
    }
}
