﻿using System;
using System.Collections.Generic;
using TimberNet;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using static TimberNet.TimberNetBase;

namespace TimberModTest
{
    public class ClientEventIO : EventIO
    {

        public readonly TimberClient client;

        public ClientEventIO(string address, int port, MapReceived mapReceivedCallback)
        {
            client = new TimberClient(address, port);
            client.OnMapReceived += mapReceivedCallback;
            client.OnLog += Plugin.Log;
            client.Start();
        }

        public void Close()
        {
            client.Close();
        }

        private ReplayEvent ToEvent(JObject obj)
        {
            return JsonSettings.Deserialize<ReplayEvent>(obj.ToString());
        }

        public void Update()
        {
            client.Update();
        }

        public List<ReplayEvent> ReadEvents(int ticksSinceLoad)
        {
            return client.ReadEvents(ticksSinceLoad)
                .Select(ToEvent).ToList();
        }

        public void WriteEvents(params ReplayEvent[] events)
        {
            foreach (ReplayEvent e in events)
            {
                // TODO: It is silly to convert to JObject here, but not sure if there's
                // a better way to do it.
                client.TryUserInitiatedEvent(JObject.Parse(JsonSettings.Serialize(e)));
            }
        }
    }
}