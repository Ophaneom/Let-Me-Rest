using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace LetMeRest
{
    class ModEntry : Mod
    {
        //DATA VARIABLES
        public Data data;
        public Dictionary<string, float> ItemDataBase;

        //CONFIG VARIABLES
        public ModConfig config;
        public static ModEntry instance;

        //CONTROL VARIABLES
        public int resetMovingTimer = 2;
        private int movingTimer;
        public static bool playingSound;

        //GENERAL VARIABLES
        public int radius = 6;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = this.Helper.ReadConfig<ModConfig>() ?? new ModConfig();
            ItemDataBase = DataBase.GetDataBase();

            helper.ConsoleCommands.Add("rest_change_multiplier", "Changes the stamina multiplier\nUsage: rest_change_riding <multiplier_value>", cm_OnChangeMultiplier);
            helper.ConsoleCommands.Add("rest_enable_sitting", "Enable/disable the sitting verification\nUsage: rest_enable_sitting <true/false>", cm_OnChangeSitting);
            helper.ConsoleCommands.Add("rest_enable_riding", "Enable/disable the riding verification\nUsage: rest_enable_riding <true/false>", cm_OnChangeRiding);
            helper.ConsoleCommands.Add("rest_enable_standing", "Enable/disable the standing verification\nUsage: rest_enable_standing <true/false>", cm_OnChangeStanding);
            helper.ConsoleCommands.Add("rest_enable_secrets", "Enable/disable the secrets verification\nUsage: rest_enable_secrets <true/false>", cm_OnChangeSecrets);
            helper.ConsoleCommands.Add("rest_reset_data", "Resets the datafile", cm_ResetData);

            helper.Events.Multiplayer.PeerConnected += this.OnPeerConnected;
            helper.Events.Multiplayer.ModMessageReceived += this.OnModMessageReceived;
            helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.onUpdate;
        }

        private void onUpdate(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady && !Game1.game1.IsActive) return;
            //Reset moving timer if using tool
            if (Game1.player.UsingTool)
            {
                if (Context.IsMultiplayer)
                {
                    if (this.data.StandingVerification)
                    {
                        movingTimer = resetMovingTimer * 60;
                        playingSound = false;
                    }
                }
                else
                {
                    if (config.StandingVerification)
                    {
                        movingTimer = resetMovingTimer * 60;
                        playingSound = false;
                    }
                }
            }

            // Singleplayer Pause Check
            if (!Context.IsMultiplayer && !Context.IsPlayerFree) return;
            // Multiplayer Pause Check
            if (Context.IsMultiplayer && Game1.player.requestingTimePause) return;

            // Check sitting
            if (Game1.player.IsSitting())
            {
                if (Context.IsMultiplayer)
                {
                    if (this.data.SittingVerification)
                    {
                        float secretMultiplier = Secrets.CheckForSecrets();
                        if (this.data.EnableSecrets) increaseStamina(1f, secretMultiplier);
                        else increaseStamina(1f, 1);
                    }
                }
                else
                {
                    if (config.SittingVerification)
                    {
                        float secretMultiplier = Secrets.CheckForSecrets();
                        if (config.EnableSecrets) increaseStamina(1f, secretMultiplier);
                        else increaseStamina(1f, 1);
                    }
                }
            }
            // Check riding
            if (Game1.player.isRidingHorse())
            {
                if (Context.IsMultiplayer)
                {
                    if (this.data.RidingVerification)
                    {
                        increaseStamina(0.25f, 1);
                    }
                }
                else
                {
                    if (config.RidingVerification)
                    {
                        increaseStamina(0.25f, 1);
                    }
                }
            }
            // Check movement
            if (!Game1.player.isMoving() &&
                !Game1.player.IsSitting() &&
                !Game1.player.isRidingHorse() &&
                !Game1.player.UsingTool)
            {
                if (Context.IsMultiplayer)
                {
                    if (this.data.StandingVerification)
                    {
                        if (movingTimer > 0) movingTimer--;
                        else increaseStamina(0.25f, 1);
                    }
                }
                else
                {
                    if (config.StandingVerification)
                    {
                        if (movingTimer > 0) movingTimer--;
                        else increaseStamina(0.25f, 1);
                    }
                }
            }
            else
            {
                if (Context.IsMultiplayer)
                {
                    if (!this.data.StandingVerification) return;
                    movingTimer = resetMovingTimer * 60;
                    playingSound = false;
                }
                else
                {
                    if (!config.StandingVerification) return;
                    movingTimer = resetMovingTimer * 60;
                    playingSound = false;
                }
            }
        }

        private void increaseStamina(float value, float secretMultiplier)
        {
            if (Game1.player.Stamina < Game1.player.MaxStamina)
            {
                // Convert PerSecond multiplier value to PerTick
                value /= 60;

                // Get multipliers
                float decorationMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[0];
                float waterMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[1];
                float paisageMultiplier = AmbientInformation.Infos(radius, ItemDataBase)[2];

                // Increases stamina in multiplayer
                if (Context.IsMultiplayer)
                {
                    Game1.player.Stamina += (value * data.Multiplier) * 
                        ((decorationMultiplier * 1.25f) * data.Multiplier) * 
                        (waterMultiplier * data.Multiplier) *
                        (paisageMultiplier * data.Multiplier) *
                        (secretMultiplier * data.Multiplier);
                }
                // Increases stamina in singleplayer
                else
                {
                    Game1.player.Stamina += (value * config.Multiplier) *
                        ((decorationMultiplier * 1.25f) * config.Multiplier) *
                        (waterMultiplier * config.Multiplier) *
                        (paisageMultiplier * config.Multiplier) *
                        (secretMultiplier * config.Multiplier);
                }
            }
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Save host information
            if (Context.IsMainPlayer && Context.IsMultiplayer)
            {
                data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();
                data.Multiplier = config.Multiplier;
                data.SittingVerification = config.SittingVerification;
                data.RidingVerification = config.RidingVerification;
                data.StandingVerification = config.StandingVerification;
                data.EnableSecrets = config.EnableSecrets;
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
            }
        }

        private void OnPeerConnected(object sender, PeerConnectedEventArgs e)
        {
            // Send data to a specific farmhand when connecting
            SendMessageToSpecificPlayer(e.Peer.PlayerID);
        }

        private void SendMessageToSpecificPlayer(long player_id)
        {
            // Send data to a specific farmhand
            if (Context.IsMainPlayer)
            {
                Data _data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                this.Monitor.Log($"Sending important data to farmhand {player_id}.", LogLevel.Trace);
                this.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { this.ModManifest.UniqueID },
                    playerIDs: new[] { player_id }
                );
            }
        }

        private void SendMessageToAllPlayers()
        {
            // Send data to all farmhands
            if (Context.IsMainPlayer)
            {
                Data _data = this.Helper.Data.ReadJsonFile<Data>($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json") ?? new Data();

                this.Monitor.Log($"Sending important data to all farmhands.", LogLevel.Trace);
                this.Helper.Multiplayer.SendMessage(
                    message: _data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { this.ModManifest.UniqueID }
                );
            }
        }

        public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // Receive data from host
            if (!Context.IsMainPlayer && e.FromModID == this.ModManifest.UniqueID && e.Type == "SaveDataFromHost")
            {
                data = e.ReadAs<Data>();
                this.Monitor.Log("Received important data from host.", LogLevel.Trace);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
            }
        }

        private void cm_OnChangeMultiplier(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.Multiplier = config.Multiplier = float.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Stamina multiplier changed to: {data.Multiplier}x", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.Multiplier = float.Parse(args[0]);
                this.Monitor.Log($"Stamina multiplier changed to: {config.Multiplier}x", LogLevel.Info);
            }
        }
        private void cm_OnChangeSitting(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.SittingVerification = config.SittingVerification = bool.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Sitting verification changed to: {args[0]}", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.SittingVerification = bool.Parse(args[0]);
                this.Monitor.Log($"Sitting verification changed to: {args[0]}", LogLevel.Info);
            }
        }
        private void cm_OnChangeRiding(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.RidingVerification = config.RidingVerification = bool.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Riding verification changed to: {args[0]}", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.RidingVerification = bool.Parse(args[0]);
                this.Monitor.Log($"Riding verification changed to: {args[0]}", LogLevel.Info);
            }
        }
        private void cm_OnChangeStanding(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.StandingVerification = config.StandingVerification = bool.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Standing verification changed to: {args[0]}", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.StandingVerification = bool.Parse(args[0]);
                this.Monitor.Log($"Standing verification changed to: {args[0]}", LogLevel.Info);
            }
        }
        private void cm_OnChangeSecrets(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data.EnableSecrets = config.EnableSecrets = bool.Parse(args[0]);
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);
                SendMessageToAllPlayers();
                this.Monitor.Log($"Secrets verification changed to: {args[0]}", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config.EnableSecrets = bool.Parse(args[0]);
                this.Monitor.Log($"Secrets verification changed to: {args[0]}", LogLevel.Info);
            }
        }
        private void cm_ResetData(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;

            // Verify if is multiplayer and if is host
            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {
                data = new Data();
                this.Helper.Data.WriteJsonFile($"MultiplayerData/{Game1.player.farmName}_Farm_Data.json", data);

                this.Monitor.Log($"Reseting all farmhands data", LogLevel.Trace);
                this.Helper.Multiplayer.SendMessage(
                    message: data,
                    messageType: "SaveDataFromHost",
                    modIDs: new[] { this.ModManifest.UniqueID }
                );
                this.Monitor.Log($"Data reseted", LogLevel.Info);
            }
            // Verify if is singleplayer
            if (!Context.IsMultiplayer)
            {
                config = new ModConfig();
                this.Monitor.Log($"Config.json reseted", LogLevel.Info);
            }
        }
    }
}
