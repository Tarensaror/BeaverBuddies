﻿using System;
using TimberNet;
using static TimberNet.TimberNetBase;

namespace BeaverBuddies
{
    public class ClientEventIO : NetIOBase<TimberClient>
    {
        // If the client receives an event to replay, no matter where it
        // originated, it shouldn't send it *back* to the server, since the
        // server is what sent the event.
        public override bool RecordReplayedEvents => false;

        // Clients don't need to send heartbeats
        public override bool ShouldSendHeartbeat => false;

        // The client doesn't get to do anything from the user directly.
        // The client should send user-initiated events to the server.
        // It has to wait until an event is received from the server.
        public override UserEventBehavior UserEventBehavior => UserEventBehavior.Send;

        private MapReceived mapReceivedCallback;
        private bool FailedToConnect = false;

        private ClientEventIO(string address, int port, MapReceived mapReceivedCallback)
        {
            this.mapReceivedCallback = mapReceivedCallback;

            netBase = new TimberClient(address, port);
            netBase.OnMapReceived += mapReceivedCallback;
            netBase.OnLog += Plugin.Log;
            try
            {
                netBase.Start();
            } catch (Exception ex)
            {
                Plugin.LogError(ex.ToString());
                CleanUp();
                FailedToConnect = true;
            }
        }

        private void CleanUp()
        {
            if (netBase == null) return;
            netBase.OnMapReceived -= mapReceivedCallback;
            netBase.OnLog -= Plugin.Log;
            netBase = null;
        }

        public static ClientEventIO Create(string address, int port, MapReceived mapReceivedCallback)
        {
            ClientEventIO eventIO = new ClientEventIO(address, port, mapReceivedCallback);
            if (eventIO.FailedToConnect) return null;
            return eventIO;
        }
    }
}
