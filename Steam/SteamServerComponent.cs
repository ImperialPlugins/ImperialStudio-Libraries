using Facepunch.Steamworks;
using UnityEngine;

//
// This class takes care of a lot of stuff for you.
//
//  1. It initializes steam on startup.
//  2. It calls Update so you don't have to.
//  3. It disposes and shuts down Steam on close.
//
// You don't need to reference this class anywhere or access the client via it.
// To access the client use Facepunch.Steamworks.Client.Instance, see SteamAvatar
// for an example of doing this in a nice way.
//
namespace ImperialStudio.Core.Steam
{
    public class SteamServerComponent : MonoBehaviour
    {
        public uint AppId;

        public static SteamServerComponent Instance { get; private set; }

        public Server Server { get; private set; }

        private void Awake ()
        {
            Instance = this;

            // keep us around until the game closes
            DontDestroyOnLoad(gameObject);

            if (AppId == 0)
                throw new System.Exception("You need to set the AppId to your game");

            //
            // Configure us for this unity platform
            //
            Config.ForUnity( Application.platform.ToString() );

            //
            // Create a steam_appid.txt (this seems greasy as fuck, but this is exactly
            // what UE's Steamworks plugin does, so fuck it.
            //
            try
            {
                System.IO.File.WriteAllText("steam_appid.txt", AppId.ToString());
            }
            catch ( System.Exception e )
            {
                Debug.LogWarning("Couldn't write steam_appid.txt: " + e.Message );
            }

            // Create the client
            Server = new Server( AppId, new ServerInit("U2019", "U2019 Game Server"));

            if ( !Server.IsValid )
            {
                Server = null;
                Debug.LogWarning("Couldn't initialize Steam");
                return;
            }

#if UNITY_SERVER
            Server.DedicatedServer = true;
#endif
            Server.LogOnAnonymous();

            Debug.Log( "Steam Server Initialized: " + Server); 
        }
	
        private void Update()
        {
            if (Server == null)
                return;

            try
            {
                UnityEngine.Profiling.Profiler.BeginSample("Steam Update Server");
                Server.Update();
            }
            finally
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private void OnDestroy()
        {
            if (Server != null)
            {
                Server.Dispose();
                Server = null;
            }

            Destroy(gameObject);
        }
    }
}
