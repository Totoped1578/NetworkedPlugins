using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WinSCP;

namespace FileUploader
{
    class Program
    {
        public class FUConfig
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
        }

        public class FUConfigGlobal
        {
            public string Password { get; set; }
            public List<FUConfig> Configs = new List<FUConfig>()
            {
                new FUConfig(),
                new FUConfig()
            };
        }

        public static FUConfigGlobal config;

        static void Main(string[] args)
        {
            if (!File.Exists("./config.json"))
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(new FUConfigGlobal()));
            config = JsonConvert.DeserializeObject<FUConfigGlobal>(File.ReadAllText("./config.json"));

            foreach (var c in config.Configs)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // Setup session options
                        SessionOptions sessionOptions = new SessionOptions
                        {
                            Protocol = Protocol.Sftp,
                            HostName = c.HostName,
                            UserName = c.UserName,
                            Password = config.Password
                        };

                        using (Session session = new Session())
                        {
                            // Connect
                            session.Open(sessionOptions);

                            // Upload files
                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Binary;

                            TransferOperationResult transferResult;
                            transferResult =
                                session.PutFiles(@"D:\Pliki\VS Projects\NetworkedPlugins\FileUploader\Upload\*", "/home/container/.config/EXILED/Plugins", true, transferOptions);

                            // Throw on any error
                            transferResult.Check();

                            // Print results
                            foreach (TransferEventArgs transfer in transferResult.Transfers)
                            {
                                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e);
                    }
                    try
                    {
                        // Setup session options
                        SessionOptions sessionOptions = new SessionOptions
                        {
                            Protocol = Protocol.Sftp,
                            HostName = c.HostName,
                            UserName = c.UserName,
                            Password = config.Password
                        };

                        using (Session session = new Session())
                        {
                            // Connect
                            session.Open(sessionOptions);

                            // Upload files
                            TransferOptions transferOptions = new TransferOptions();
                            transferOptions.TransferMode = TransferMode.Binary;

                            TransferOperationResult transferResult;
                            transferResult =
                                session.PutFiles(@"D:\Pliki\VS Projects\NetworkedPlugins\FileUploader\Upload\dependencies\*", "/home/container/.config/EXILED/Plugins/dependencies", true, transferOptions);

                            // Throw on any error
                            transferResult.Check();

                            // Print results
                            foreach (TransferEventArgs transfer in transferResult.Transfers)
                            {
                                Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: {0}", e);
                    }
                });
            }
           
        }
    }
}
