﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TimberNet
{
    public class TimberClient : TimberNetBase
    {

        private readonly TcpClient client;

        public readonly string address;
        public readonly int port;

        public override bool ShouldTick => base.ShouldTick && receivedEvents.Count > 0;

        public TimberClient(string address, int port) : base()
        {
            client = new TcpClient();
            this.address = address;
            this.port = port;
        }

        public override bool TryUserInitiatedEvent(JObject message)
        {
            SendEvent(client, message);
            return false;
            // Don't actually do the event - wait for the server to confirm
            // w/ adjusted Tick
        }

        public override void Start()
        {
            base.Start();
            client.Connect(address, port);
            // Connect a TCP socket at the address
            Task.Run(() => StartListening(client, true));
        }


        public override void Close()
        {
            client.Close();
        }
    }
}