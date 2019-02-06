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
namespace ImperialStudio.Core.UnityEngine.Steam
{
    public class SteamClientComponent : MonoBehaviour
    {
        public uint AppId;

        public static SteamClientComponent Instance { get; private set; }

        public Facepunch.Steamworks.Client Client { get; private set; }

        private void Awake()
        {
            Instance = this;

            // keep us around until the game closes
            DontDestroyOnLoad(gameObject);

            if (AppId == 0)
                throw new System.Exception("You need to set the AppId to your game");

            //
            // Configure us for this unity platform
            //
            Facepunch.Steamworks.Config.ForUnity( Application.platform.ToString() );

            // Create the client
            Client = new Facepunch.Steamworks.Client( AppId );

            if ( !Client.IsValid )
            {
                Client = null;
                return;
            }

            Debug.Log( "Steam Initialized: " + Client.Username + " / " + Client.SteamId ); 
        }
	
        private void Update()
        {
            if (Client == null)
                return;

            try
            {
                UnityEngine.Profiling.Profiler.BeginSample("Steam Client Update");
                Client.Update();
            }
            finally
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private void OnDestroy()
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }

            Destroy(gameObject);
        }
    }
}
