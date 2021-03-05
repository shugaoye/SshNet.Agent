﻿using System;
using System.Collections.Generic;
using Renci.SshNet;
using Renci.SshNet.Security;
using SshNet.Agent.AgentMessage;
using SshNet.Agent.Keys;

namespace SshNet.Agent
{
    public class SshAgent
    {
        private readonly string _socketPath;

        public SshAgent() : this(AgentSocketPath.GetPath())
        {
        }

        public SshAgent(string socketPath)
        {
            _socketPath = socketPath;
        }

        public IEnumerable<AgentIdentity> RequestIdentities()
        {
            var list = Send(new RequestIdentities(this));
            if (list is null)
                return new List<AgentIdentity>();
            return (IEnumerable<AgentIdentity>)list;
        }

        public void RemoveAllIdentities()
        {
            _ = Send(new RemoveIdentity());
        }

        public void RemoveIdentities(IEnumerable<PrivateKeyFile> privateKeyFiles)
        {
            foreach (var privateKeyFile in privateKeyFiles)
            {
                RemoveIdentity(privateKeyFile);
            }
        }

        public void RemoveIdentity(PrivateKeyFile privateKeyFile)
        {
            if (!(((KeyHostAlgorithm)privateKeyFile.HostKey).Key is IAgentKey agentKey))
                throw new ArgumentException("Just AgentKeys can be removed");

            _ = Send(new RemoveIdentity(agentKey));
        }

        public void AddIdentity(PrivateKeyFile keyFile)
        {
            _ = Send(new AddIdentity(keyFile));
        }

        internal byte[] Sign(IAgentKey key, byte[] data)
        {
            var signature = Send(new RequestSign(key, data));
            if (signature is null)
                return new byte[0];
            return (byte[])signature;
        }

        internal virtual object? Send(IAgentMessage message)
        {
            using var socketStream = new AgentSocketStream(_socketPath);
            using var writer = new AgentWriter(socketStream);
            using var reader = new AgentReader(socketStream);

            message.To(writer);
            return message.From(reader);
        }
    }
}