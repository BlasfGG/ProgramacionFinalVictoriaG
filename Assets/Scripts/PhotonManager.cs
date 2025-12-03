using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner; //es quien se encarga de enviar y recibir info, es mi medio de comunicacion con mi servidor
    [SerializeField] NetworkSceneManagerDefault sceneManager;
    [SerializeField] private NetworkPrefabRef prefabPlayer; // referencia al player prefab
    [SerializeField] private GameObject canvas; // referencia al canvas del menu principal

    Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

    #region Metodos de Photon
    public void OnConnectedToServer(NetworkRunner runner) // Cuando se conecta al servidor
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) //Cuando la conexion falla

    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) // Cuando se recibe una solicitud de conexion
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) // Cuando se recibe una respuesta de autenticacion personalizada
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) // Cuando se desconecta del servidor
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) // Cuando se migra el host
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input) // Cuando se recibe una entrada
    {
        NetworkInfoData data = new NetworkInfoData()
        {
            move = InputManager.Instance.GetMoveInput(), // Obtenemos la posicion del jugador
            rotation = InputManager.Instance.GetMouseDelta(), // Obtenemos la rotacion del jugador
        };
        input.Set(data); // Enviamos la entrada al runner

        if (InputManager.Instance.WasRunInputPressed()) data.buttons.Set(NetworkInfoData.BotonCorrer, true);

        if (InputManager.Instance.BotonDisparoPresionado()) data.buttons.Set(NetworkInfoData.BotonDisparo, true);

        if (InputManager.Instance.BotonRecargaPresionado()) data.buttons.Set(NetworkInfoData.BotonRecarga, true);

        input.Set(data);

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) // Cuando falta una entrada
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) // Cuando un objeto entra en el area de interes
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) // Cuando un objeto sale del area de interes
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return; // Solo el servidor puede spawnear jugadores

        if (SceneManager.GetActiveScene().name == "Victoria")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!players.ContainsKey(player)) // Si el jugador no esta en la lista
        {
            Debug.Log($"Player Joined: {player.PlayerId}");
            Vector3 spawnPosition = new Vector3(player.RawEncoded % runner.Config.Simulation.PlayerCount * 0f, 12f, -15f);
            NetworkObject networkObject = runner.Spawn(prefabPlayer, spawnPosition, Quaternion.identity, player);
            players.Add(player, networkObject);
        }
        else
        {
            Debug.Log($"Player {player.PlayerId} already exists in playerList.");
        }

    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) // Cuando un jugador se va
    {
        // Eliminar al jugador de la lista cuando se va
        if (players.ContainsKey(player))
        {
            if (players[player] != null)
            {
                runner.Despawn(players[player]);
            }
            players.Remove(player);
        }

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) // Cuando se recibe el progreso de datos confiables
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) // Cuando se recibe datos confiables
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner) // Cuando se carga una escena
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner) // Cuando se empieza a cargar una escena
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) // Cuando se actualiza la lista de sesiones
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) // Cuando se apaga el servidor
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) // Cuando se recibe un mensaje de simulacion de usuario
    {

    }

    private void Update()
    {

    }

    #endregion

    /// <summary>
    /// 
    /// En este metodo vamoa a crear o buscar una partida. Si no existe alguna partida o lobby, entonces nosotros lo creamos y somos el host, 
    /// si ya hay una partida entonces entramos y comos el cliente.
    /// 
    /// Aqui vamos a configurar cuantos jugadores se pueden conectar a la partida como maximo, cual mapa va a ser el que va a cargar, 
    /// tambien si dentro de la partida puede haber cambios de escena
    /// 
    /// 
    /// </summary>
    private async void StartGame(GameMode mode)
    {
        runner.AddCallbacks(this);
        runner.ProvideInput = true;

        //guarda que escena se va a guardar
        var scene = SceneRef.FromIndex(0); //guardar una referencia a la escena 0

        //Guarda como se va a guardar las escenas en el juego, puede guardar la info de hasta 8 escenas
        var sceneInfo = new NetworkSceneInfo(); //creo una variable que me va a guardar las escenas que voy a usar y como las debo cargar

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "#0001", //nombre interno que como yo desarrollador debo de entender
            Scene = scene,
            CustomLobbyName = "Official EA Europe", //nombre que quiero mostrar
            SceneManager = sceneManager
        });

        if (canvas != null) canvas.SetActive(false);

    }

    public void StartGameAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartGameAsClient()
    {
        StartGame(GameMode.Client);
    }
}
