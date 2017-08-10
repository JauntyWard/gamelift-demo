using System;
using Newtonsoft.Json;
using Aws.GameLift.Server;
using Nancy.ModelBinding;

namespace gameserver
{
	public class PlayerType
	{
		public string session_id { get; set; }
		public string player_id { get; set; }
	}


}

