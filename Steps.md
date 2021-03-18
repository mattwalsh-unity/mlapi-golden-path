## 1. Setting up the project

1. `git clone` this project so a place of your choosing.  I'll use `~/dev/mlapi-golden-path`
1. Open the Unity Hub.
1. Click New 
1. Select type ‘3D’
1. Name the project "GoldenPath".
1. Use the location you cloned the starter project in (`~/dev/mlapi-golden-path`)
1. Click Create
2. Copy the .gitignore into your new folder (`cp ~/dev/mlapi-golden-path/.gitignore ~/dev/mlapi-golden-path/GoldenPath`)
3. This will take you to the `created starter project` git commit for the project

## 2. Loading the MLAPI library into your project

1. Open your newly created project in the Unity editor
1. Open Window > Package Manager
1. Click the *+* in the upper left to add
1. Select `Add package from git URL...`
1. Enter the Git URL like this:  
 https://github.com/Unity-Technologies/com.unity.multiplayer.mlapi.git?path=/com.unity.multiplayer.mlapi#release/0.1.0   
 _(note I have selected `release/0.1.0`; use whatever version of MLAPI you want to test against here)_
1. You should now see "MLAPI Networking Library" in your package manager.  If you enter the URL in incorrectly it will silently fail; make sure to get the URL perfect - no leading or trailing spaces
2. This will take you to the `added MLAPI library` git commit for the project

## 3. Creating a Network Manager
1. Right click in the Hierarchy tab of the Main Unity Window.
1. Select **Create Empty**.
1. Rename the GameObject **NetworkManager**.
1. Select **NetworkManager**.
1. Click **Add Component** in the Inspector Tab.
1. Types `NetworkManager` and add that component from the list displayed
1. Inside the `NetworkManager` component tab, locate the  `NetworkTransport` field. 
1. Click "Select Transport".
1. Select `UnetTransport`
1. Save your scene
2. This will take you to the "added Network Manager" commit for the project

## 4. Creating an object to spawn for each connected player
1. Right click in the Hierarchy tab of the Main Unity Window.
1. Select **Create 3D Object->Capsule** 
2. Name it **Player**
1. Click **Add Component** in the Inspector Tab.
4. Add a `NetworkObject` component 
5. Click the Assets folder.
6. Create a new Folder and call it **Prefabs**.
7. Make **Player** a prefab by dragging it to **Prefabs** folder you just created.
1. Delete **Player** from scene.  We must do this because the library will automatically spawn this prefab for each player that connects; we don't want it to show up automatically in the scene
1. Select `NetworkManager`
2. Find the `NetworkPrefabs` list section
3. Click `+` to create a slot
4. Drag this player prefab from above into the new empty slot
5. Click the **Default Player Prefab.** checkbox
6. Right click in the Hierarchy tab of the Main Unity Window
7. Select **Create 3D Object->Plane** (defaults are fine)
8. Save your scene
9. Optional: To do a quick test, click play.  Editor will start, and you will just see the plane.  Now, without stopping the editor's play mode, navigate to the `NetworkManager` componet in the Hierarchy tab (it will be underneath `DontDestroyOnLoad`).  In the `NetworkManager` inspector scroll down and find the `Start Host` button.  If you click it, you will see the player capsule spawn.  Stop the player
10. This will take you to the "added per-player spawn object" commit

## 5. Creating a command line helper
This command line helper will help us launch our project from a command line.   Eventually we will include this in the MLAPI project in some form

1. Right click on the `NetworkManager` in the hierarchy view
2. Create an empty GameObject underneath it.  Name it `NetworkCommandLine`
3. In the inspector for this object, **Add Component > New Script** and name it `NetworkCommandLine`
4. An empty script will be created.  Open it, and paste this code in instead
```
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
   private NetworkManager netManager;

   void Start()
   {
       netManager = GetComponentInParent<NetworkManager>();

       if (Application.isEditor) return;

       var args = GetCommandlineArgs();

       if (args.TryGetValue("-mlapi", out string mlapiValue))
       {
           switch (mlapiValue)
           {
               case "server":
                   netManager.StartServer();
                   break;
               case "host":
                   netManager.StartHost();
                   break;
               case "client":
                   netManager.StartClient();
                   break;
           }
       }
   }

   private Dictionary<string, string> GetCommandlineArgs()
   {
       Dictionary<string, string> argDictionary = new Dictionary<string, string>();

       var args = System.Environment.GetCommandLineArgs();

       for (int i = 0; i < args.Length; ++i)
       {
           var arg = args[i].ToLower();
           if (arg.StartsWith("-"))
           {
               var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
               value = (value?.StartsWith("-") ?? false) ? null : value;

               argDictionary.Add(arg, value);
           }
       }
       return argDictionary;
   }
}
```

5. Go into **File > Build Settings > Player Settings...**, and change `Full Screen` mode into `Windowed` mode.  If on a Pro Unity, you probably want to also disable the splash screen
6. Let's do a build and test.  Select **File > Build and Run**.  Create a folder called `Build`.  You want to use `Build` because the `.gitignore` file knows about it.  Name the binary `GoldenPath`.  Your project will build, and it will launch, and you should see the plane.  Quit your app.
7. Let's launch from the command line.  From a Mac terminal do: `~/dev/mlapi-golden-path/GoldenPath/Build/GoldenPath.app/Contents/MacOS/GoldenPath -mlapi server -logfile - & ; ~/dev/mlapi-golden-path/GoldenPath/Build/GoldenPath.app/Contents/MacOS/GoldenPath -mlapi client -logfile -` 
You will see 2 players spawn.  Both should show a plane and a capsule (the capsule being the single player that was spawned)  
(todo, Windows instructions for this step)
8.  This will take you to the "added command line" commit

## 6. Introducing Server- and Client- controlled Network Variables
1. Right click on the player prefab
2. Add a new script component, name it `NetworkVariableTest`
3. Open the file, change it to the following:

```
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class NetworkVariableTest : NetworkBehaviour
{
    private NetworkVariable<float> ServerNetworkVariable = new NetworkVariable<float>();
    private NetworkVariable<float> ClientNetworkVariable = new NetworkVariable<float>();
    private float last_t = 0.0f;

    void Start()
    {
        ClientNetworkVariable.Settings.WritePermission = NetworkVariablePermission.OwnerOnly;
        ClientNetworkVariable.Settings.ReadPermission = NetworkVariablePermission.ServerOnly;

        if (IsServer)
        {
            ServerNetworkVariable.Value = 0.0f;
            Debug.Log("Server's var initialized to: " + ServerNetworkVariable.Value);
        }
        else if (IsClient)
        {
            ClientNetworkVariable.Value = 0.0f;
            Debug.Log("Client's var initialized to: " + ClientNetworkVariable.Value);
        }
    }

    void Update()
    {
        var t_now = Time.time;
        if (IsServer)
        {
            ServerNetworkVariable.Value = ServerNetworkVariable.Value + 0.1f;
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Server set its var to: " + ServerNetworkVariable.Value + ", has client var at: "  + 
                    ClientNetworkVariable.Value);
            }
        }
        else if (IsClient)
        {
            ClientNetworkVariable.Value = ClientNetworkVariable.Value + 0.1f;
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Client set its var to: " + ClientNetworkVariable.Value + ", has server var at: "  + 
                    ServerNetworkVariable.Value);
            }
        }
    }
}
```
4. Choose "Build and run", kill the player and launch the client & server together in a terminal as shown in the previous step.  After a brief delay, the client and server will spawn.  You should expect to see this in the console, showing that the server and client are sharing the variable.  Since the printing doesn't happen on every tick, the numbers won't match up perfectly.

```
Server's var initialized to: 0
Client's var initialized to: 0
Server set its var to: 0.1, has client var at: 0
Client set its var to: 0.1, has server var at: 0.2
Server set its var to: 3.099999, has client var at: 2.6
Client set its var to: 3.099999, has server var at: 3.199999
Server set its var to: 6.099997, has client var at: 5.599997
```
5.  This will take you to the "network variables" added commit

## 7. Introducing a Network Transform
1. Select Player prefab.  `Add Component > Network Transform`
2. Select Player prefab.  `Add Component > New Script`.  Name it `NetworkTransformTest`
3. Edit `NetworkTransformTest` and change the code to be:
```
using System;
using MLAPI;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsClient)
        {
            float theta = Time.frameCount / 10.0f;
            transform.position = new Vector3((float) Math.Cos(theta), 0.0f, (float) Math.Sin(theta));
        }
    }
}
```
4. Choose "Build and run", kill the player and launch the client & server together in a terminal as shown in the previous steps.  Expect to see the player capsule moving in a circle on both the client and the server
5. This will take you to the `network transform added` commit in the project

## 8. Introducing RPCs
1. Select Player prefab.  `Add Component > New Script`.  Name it `RpcTest`
2. Open the file and replace it with this:
```
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{
    private bool firstTime = true;

    [ClientRpc]
    void TestClientRpc(int value)
    {
        if (IsClient)
        {
            Debug.Log("Client Received the RPC #" + value);
            TestServerRpc(value + 1);
        }
    }

    [ServerRpc]
    void TestServerRpc(int value)
    {
        if (IsServer)
        {
            Debug.Log("Server Received the RPC #" + value);
            TestClientRpc(value);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (IsClient && firstTime)
        {
            firstTime = false;
            TestServerRpc(0);
        }
    }
}
```

3. Choose "Build and run", kill the player and launch the client & server together in a terminal as shown in the previous steps.  In the console, expect to see the client and server sending RPC messages to each other.  The client kicks off the exchange in its `Update` call the first time with a counter value of 0.  It then makes an RPC call to the server with the next value.  The server receives this and calls the client, etc.  In the console view, you will see:

```
Server Received the RPC #1
Client Received the RPC #1
Server Received the RPC #2
Client Received the RPC #2
Server Received the RPC #3
Client Received the RPC #3
...
```

## 9. Introducing Multiple Scenes
1. In the *project* view (not the Hierarchy) click on the `Assets > Scenes` folder.  You should see your one and only scene `SampleScene` inside
2. Select `SampleScene` and duplicate it  `Edit > Duplicate`.  A new scene will appear in the scene folder
3. Select this new scene and rename it to `SampleScene2`.  You should now have 2 scenes, `SampleScene` and `SampleScene2`.
4. Save your `SampleScene`
5. Double click `SampleScene2` to open it.  It will now appear in the hierarchy
6. Delete the `NetworkManager` component from `SampleScene2`.  We want to have one and only one NetworkManager per project
7. Let's pick a color for the scene so we can tell which one is active.  Select `Main Camera` from the Hierarchy view.  In the inspector, select `Clear Flags` and change it to `Solid Color`.  It will auto-select a blue color.
8. Save `SampleScene2`
9. Open up `File > Build Settings`.  Note the "Scenes in build" area, currently blank.  Click `Add Open Scenes`.  You should hnow see `SampleScene2` in the list.  Close the dialog.
10. Double click on `SampleScene` to go back to our original scene
11. Let's pick a different color for this other scene.  Select `Main Camera` from the Hierarchy view.  In the inspector, select `Clear Flags` and change it to `Solid Color`.  It will auto-select a blue color.  Pick any other color than you chose for `SampleScene2`
12. Save the scene
13.  Open up `File > Build Settings`.  Click `Add Open Scenes` so that now both scenes appear.  However, drag `SampleScene` to be on the top with `SampleScene2` on the bottom. Close the dialog.
14.  Select the `NetworkManager`. In the inspector find the "Registered Scene Names" section.  Set it up so that you have 2 entries, `SampleScene` and `SampleScene2`.  Note, unlike how prefabs are registered, you do not drag a scene into the box; you enter in its name, and the name must match the scene's name exactly
15.  Select the Player Prefab.  In the inspector, `Add Component > New Script` and name it `MultiSceneTest`.  Paste in the code below to replace what's there
```
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.SceneManagement;

public class MultiSceneTest : MonoBehaviour 
{
    [SerializeField]
    private bool m_EnableSceneSwitchTimer;

    [SerializeField]
    private float m_SwitchSceneInterval = 5.0f;
    private float m_SwitchSceneTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_SwitchSceneTimer = Time.realtimeSinceStartup + m_SwitchSceneInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_EnableSceneSwitchTimer && m_SwitchSceneTimer <  Time.realtimeSinceStartup)
        {
            SwitchToNextScene();
        }
        else
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchToNextScene();
        }
    }

    private Scene m_ActiveScene;

    private void SwitchToNextScene()
    {
        m_ActiveScene = SceneManager.GetActiveScene();
        var sceneIndex = NetworkManager.Singleton.NetworkConfig.RegisteredScenes.FindIndex(FindCurrentScene);
        sceneIndex++;
        if(sceneIndex >= NetworkManager.Singleton.NetworkConfig.RegisteredScenes.Count)
        {
            sceneIndex = 0;
        }
        NetworkSceneManager.SwitchScene(NetworkManager.Singleton.NetworkConfig.RegisteredScenes[sceneIndex]);
    }

    private bool FindCurrentScene(string sceneName)
    {
        if(m_ActiveScene != null)
        {
            return m_ActiveScene.name == sceneName;
        }

        return false;
    }
}
```
17.  Optional: To do a quick test, click play.  Editor will start, and you will just see the plane.  Now, without stopping the editor's play mode, navigate to the `NetworkManager` component in the Hierarchy tab (it will be underneath `DontDestroyOnLoad`).  In the `NetworkManager` inspector scroll down and find the `Start Host` button.  If you click it, you will see the player capsule spawn and spin in a circle as before.  However now when you press `space`, the background color should alternate to the color you pick for `SampleScene2` and back to `SampleScene` as you are swithching scenes with every press.  Now stop the player.
18. Choose "Build and run", kill the player and launch the client & server together in a terminal as shown in the previous steps.  Expect to see the capsule spinning in both windows as before.  However, if you select the server window and hit space it will toggle between the active scenes simulating the sever making a scene change.  If you hit space in the client window, nothing will happen.

