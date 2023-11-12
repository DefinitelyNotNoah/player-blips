using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Server;
using CitizenFX.Server.Native;
using Newtonsoft.Json;

namespace Server;

public class ServerScript : BaseScript
{
    private void SendPlayerData()
    {
        IDictionary<string, object> playerData = new Dictionary<string, object>();

        // Loop through all the players known to the server, where their ped entity exists.
        foreach (Player player in new PlayerList().ToList().Where(player => player.Character != null))
        {
            // Check if player is in vehicle.
            int vehicle = Natives.GetVehiclePedIsIn(player.Character.Handle, false);

            playerData[player.Handle.ToString()] = new object[]
            {
                player.Character.Position,
                player.Character.Heading,
                player.Name,
                Natives.GetEntityModel(vehicle),
                Natives.GetEntityRoutingBucket(player.Character.Handle)
            };

            // Update player state bags to make routing buckets known client-side.
            player.State["bucket"] = Natives.GetEntityRoutingBucket(player.Character.Handle);
        }

        Events.TriggerAllClientsEvent("PlayerBlips-Client:ReceivePlayerData", playerData);
    }

    [EventHandler("playerEnteredScope")]
    public void PlayerEnteredScopeServerEvent(IDictionary<string, object> data)
    {
        Events.TriggerAllClientsEvent("PlayerBlips-Client:PlayerEnteredScope", data["for"], data["player"]);
    }

    [EventHandler("playerLeftScope")]
    public void PlayerLeftScopeServerEvent(IDictionary<string, object> data)
    {
        Events.TriggerAllClientsEvent("PlayerBlips-Client:PlayerLeftScope", data["for"], data["player"]);
    }

    [EventHandler("playerDropped")]
    public void PlayerDroppedServerEvent([Source] Player source, string reason)
    {
        Events.TriggerAllClientsEvent("PlayerBlips-Client:PlayerDropped", source.Handle);
    }

    [Tick]
    public async Coroutine OnTick()
    {
        SendPlayerData();
        string config = File.ReadAllText(Natives.GetResourcePath("playerblips") + "/config.json");
        dynamic json = JsonConvert.DeserializeObject<dynamic>(config);
        
        await Wait((uint)json["Delay"]);
    }
}