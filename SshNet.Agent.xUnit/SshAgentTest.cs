using System;
using System.Diagnostics;

using Xunit;

using Renci.SshNet;

namespace SshNet.Agent.xUnit
{
    public class SshAgentTest
    {
        [Fact]
        public void SshAgentTest01()
        {
            try 
            {
                var agent = new SshAgent();
                var keys = agent.RequestIdentities();
                foreach (var key in keys) 
                {
                    var hostKey = key.HostKey as Renci.SshNet.Security.KeyHostAlgorithm;
                    Debug.WriteLine($"{hostKey.Name} {hostKey.Key.KeyLength} {key.HostKey.GetHashCode()}  {hostKey.Key.Comment}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
