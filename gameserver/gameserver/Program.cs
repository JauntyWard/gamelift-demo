using System;

using Aws.GameLift.Server;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using System.Threading;
using Newtonsoft.Json;
using Nancy.ModelBinding;


namespace gameserver
{
	public class API : Nancy.NancyModule

	{
		public API()
		{

			Get["/end"] = _ =>
			{

				GameLiftServerAPI.TerminateGameSession();
				Console.WriteLine("DESTROYING SESSION");

				return 200;
			};

			Get["/shutdown"] = _ =>
			{
				GameLiftServerAPI.Destroy();
				return 200;
			};

			Post["/start"] = _ =>
			{
				try
				{
					var body = this.Request.Body;
					int length = (int)body.Length; // this is a dynamic variable
					byte[] data = new byte[length];
					body.Read(data, 0, length);
					PlayerType json = this.Bind<PlayerType>();

					Console.WriteLine(json.session_id);

					var acceptPlayerSessionOutcome = GameLiftServerAPI.AcceptPlayerSession(json.session_id);

					return 200;

				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
				return 200;


			};
		}
	}

    class Program
    {

        private static int PORT = 3389;
        private bool running;
        //private Thread keepAlive;

        public void start()
        {
            var initSDKOutcome = GameLiftServerAPI.InitSDK();
            if (initSDKOutcome.Success)
            {
                // Set parameters and call ProcessReady
                var processParams = new ProcessParameters(
                   (gameSession) =>
                   {
                       Console.WriteLine("HERE");
                       GameLiftServerAPI.ActivateGameSession();

                   },
                   this.OnProcessTerminate,
                   this.OnHealthCheck,
                   PORT,
                   new LogParameters(new List<string>()          // Examples of log and error files written by the game server
                   {
                  "/local/game/log"
                   })
                );

                var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParams);
                if (processReadyOutcome.Success)
                {
                    Console.WriteLine("ProcessReady success.");
                }
                else
                {
                    Console.WriteLine("ProcessReady failure : " + processReadyOutcome.Error.ToString());
                }
            }
            else
            {
                Console.WriteLine("InitSDK failure : " + initSDKOutcome.Error.ToString());
            }


        }


        void OnProcessTerminate()
        {
            // game-specific tasks required to gracefully shut down a game session,
            // such as notifying players, preserving game state data, and other cleanup
            var ProcessEndingOutcome = GameLiftServerAPI.ProcessEnding();
            GameLiftServerAPI.Destroy();
            this.running = false;
        }

        bool OnHealthCheck()
        {
            bool isHealthy = true;
            // complete health evaluation within 60 seconds and set health
            return isHealthy;
        }

        void OnApplicationQuit()
        {
            //Make sure to call GameLiftServerAPI.Destroy() when the application quits. This resets the local connection with GameLift's agent.
            GameLiftServerAPI.Destroy();
        }


        //void run()
        //{
        //    this.running = true;
        //    this.startThread();
        //    this.keepAlive.Start();





        //}


        //public void startThread()
        //{
        //    this.keepAlive = new Thread(() =>
        //    {
        //        while (running)
        //        {
        //            Console.WriteLine("Alive");
        //            System.Threading.Thread.Sleep(1000);
        //        }
        //    });
        //}


        static void Main(String[] args)
        {
            Program foo = new Program();
            foo.start();

			var url = "http://localhost:" + PORT;

			         //var configuration = new HostConfiguration()
			         //{
			         //    UrlReservations = new UrlReservations() { CreateAutomatically = true }
			         //};

            var server = new Nancy.Hosting.Self.NancyHost(new Uri(url));

			
			server.Start();
			Console.WriteLine("Nancy Server listening on {0}", url);
            //Console.Read();




            while (true)
            {
                Thread.Sleep(10000000);
            }

		}
    }
}
