using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO;
using UnityEngine;
using UnityEngine.Events;
using System;
using BestHTTP.WebSocket;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DG.Tweening;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;

public class SocketIOManager : MonoBehaviour
{
    [SerializeField]
    private SlotBehaviour slotManager;

    internal GameData initialData = null;
    internal UIData initUIData = null;
    internal GameData resultData = null;
    internal PlayerData playerdata = null;
    [SerializeField]
    internal List<string> bonusdata = null;
    WebSocket currentSocket = null;
    internal bool isResultdone = false;

    protected string gameID = "SL-VIK";

    private void Start()
    {
        OpenWebsocket();
    }

    private void InitRequest(WebSocket webSocket)
    {
        InitData message = new InitData();
        message.Data = new AuthData();
        message.Data.GameID = gameID;
        message.id = "Auth";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        Debug.Log(json);
        // Send the message
        webSocket.Send(json);
    }

    private void OpenWebsocket()
    {
        var webSocket = new WebSocket(new Uri("wss://slotwebsocket-new.onrender.com"));
        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnError += OnWebSocketError;
        webSocket.Open();
    }

    private void OnWebSocketError(WebSocket webSocket, string message)
    {
        Debug.Log(message);
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("WebSocket is now Open!");
        currentSocket = webSocket;
        InitRequest(webSocket);
    }

    private void OnMessageReceived(WebSocket webSocket, string message)
    {
        ParseResponse(message);
    }

    private void ParseResponse(string jsonObject)
    {
        Debug.Log(jsonObject);
        Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

        string id = myData.id;

        switch(id)
        {
            case "InitData":
                {
                    Debug.Log(jsonObject);
                    initialData = myData.message.GameData;
                    initUIData = myData.message.UIData;
                    playerdata = myData.message.PlayerData;
                    bonusdata = myData.message.BonusData;
                    List<string> LinesString = ConvertListListIntToListString(initialData.Lines);
                    List<string> InitialReels = ConvertListOfListsToStrings(initialData.Reel);
                    InitialReels = RemoveQuotes(InitialReels);
                    PopulateSlotSocket(InitialReels, LinesString);
                    break;
                }
            case "ResultData":
                {
                    Debug.Log(jsonObject);
                    myData.message.GameData.FinalResultReel = ConvertListOfListsToStrings(myData.message.GameData.ResultReel);
                    myData.message.GameData.FinalsymbolsToEmit = TransformAndRemoveRecurring(myData.message.GameData.symbolsToEmit);
                    resultData = myData.message.GameData;
                    playerdata = myData.message.PlayerData;
                    isResultdone = true;
                    break;
                }
        }
    }

    private void PopulateSlotSocket(List<string> slotPop, List<string> LineIds)
    {
        for (int i = 0; i < slotPop.Count; i++)
        {
            List<int> points = slotPop[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
            slotManager.PopulateInitalSlots(i, points);
        }

        for (int i = 0; i < slotPop.Count; i++)
        {
            slotManager.LayoutReset(i);
        }

        for (int i = 0; i < LineIds.Count; i++)
        {
            slotManager.FetchLines(LineIds[i], i);
        }

        slotManager.SetInitialUI();

        Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
    }

    internal void AccumulateResult(double currBet)
    {
        if (currentSocket != null)
        {
            isResultdone = false;
            SendDataWithNamespace("Spin", currBet, currentSocket);
        }
    }

    private void SendDataWithNamespace(string namespaceName, double bet, WebSocket webSocket)
    {
        // Construct message data

        MessageData message = new MessageData();
        message.Data = new BetData();
        message.Data.CurrentBet = bet;
        message.id = namespaceName;
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        Debug.Log(json);
        // Send the message
        webSocket.Send(json);
    }

    private List<string> RemoveQuotes(List<string> stringList)
    {
        for (int i = 0; i < stringList.Count; i++)
        {
            stringList[i] = stringList[i].Replace("\"", ""); // Remove inverted commas
        }
        return stringList;
    }

    private List<string> ConvertListListIntToListString(List<List<int>> listOfLists)
    {
        List<string> resultList = new List<string>();

        foreach (List<int> innerList in listOfLists)
        {
            // Convert each integer in the inner list to string
            List<string> stringList = new List<string>();
            foreach (int number in innerList)
            {
                stringList.Add(number.ToString());
            }

            // Join the string representation of integers with ","
            string joinedString = string.Join(",", stringList.ToArray()).Trim();
            resultList.Add(joinedString);
        }

        return resultList;
    }

    private List<string> ConvertListOfListsToStrings(List<List<string>> inputList)
    {
        List<string> outputList = new List<string>();

        foreach (List<string> row in inputList)
        {
            string concatenatedString = string.Join(",", row);
            outputList.Add(concatenatedString);
        }

        return outputList;
    }

    private List<string> TransformAndRemoveRecurring(List<List<string>> originalList)
    {
        // Flattened list
        List<string> flattenedList = new List<string>();
        foreach (List<string> sublist in originalList)
        {
            flattenedList.AddRange(sublist);
        }

        // Remove recurring elements
        HashSet<string> uniqueElements = new HashSet<string>(flattenedList);

        // Transformed list
        List<string> transformedList = new List<string>();
        foreach (string element in uniqueElements)
        {
            transformedList.Add(element.Replace(",", ""));
        }

        return transformedList;
    }
}

[Serializable]
public class BetData
{
    public double CurrentBet;
    //public double TotalLines;
}

[Serializable]
public class AuthData
{
    public string GameID;
    //public double TotalLines;
}

[Serializable]
public class MessageData
{
    public BetData Data;
    public string id;
}

[Serializable]
public class InitData
{
    public AuthData Data;
    public string id;
}

[Serializable]
public class AbtLogo
{
    public string logoSprite { get; set; }
    public string link { get; set; }
}

[Serializable]
public class GameData
{
    public List<List<string>> Reel { get; set; }
    public List<List<int>> Lines { get; set; }
    public List<int> Bets { get; set; }
    public bool canSwitchLines { get; set; }
    public List<int> LinesCount { get; set; }
    public List<int> autoSpin { get; set; }
    public List<List<string>> ResultReel { get; set; }
    public List<int> linesToEmit { get; set; }
    public List<List<string>> symbolsToEmit { get; set; }
    public double WinAmout { get; set; }
    public double freeSpins { get; set; }
    public List<string> FinalsymbolsToEmit { get; set; }
    public List<string> FinalResultReel { get; set; }
    public double jackpot { get; set; }
    public bool isBonus { get; set; }
    public double BonusStopIndex { get; set; }
}

[Serializable]
public class Message
{
    public GameData GameData { get; set; }
    public UIData UIData { get; set; }
    public PlayerData PlayerData { get; set; }
    public List<string> BonusData { get; set; }
}

[Serializable]
public class Root
{
    public string id { get; set; }
    public Message message { get; set; }
}

[Serializable]
public class UIData
{
    public Paylines paylines { get; set; }
    public List<string> spclSymbolTxt { get; set; }
    public AbtLogo AbtLogo { get; set; }
    public string ToULink { get; set; }
    public string PopLink { get; set; }
}

[Serializable]
public class Paylines
{
    public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Symbol
{
    public Multiplier multiplier { get; set; }
}

[Serializable]
public class Multiplier
{
    [JsonProperty("5x")]
    public double _5x { get; set; }

    [JsonProperty("4x")]
    public double _4x { get; set; }

    [JsonProperty("3x")]
    public double _3x { get; set; }

    [JsonProperty("2x")]
    public double _2x { get; set; }
}

[Serializable]
public class PlayerData
{
    public double Balance { get; set; }
    public double haveWon { get; set; }
}



