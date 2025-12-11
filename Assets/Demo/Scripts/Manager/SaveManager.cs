using CreatorKitCode;
using CreatorKitCodeInternal;
using System;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(GameDataManager))]
public class SaveManager : MonoBehaviour
{
    GameDataManager m_GameDataManager;
    [SerializeField] string m_SaveFilename = "savegame.dat";
    [SerializeField] string m_SaveFilenameInventory = "savegameInventory.dat";
    [SerializeField] string m_SaveFilenameEquipment = "savegameEquipment.dat";
    [Tooltip("Show Debug messages.")]
    [SerializeField] bool m_DebugValues;

    private static string inventoryFilePath => Application.persistentDataPath + "/inventory.json";


    public static event Action<GameData> GameDataLoaded;
    public static event Action<InventorySystem> GameInventoryDataLoaded;
    public static event Action<EquipmentSystem> GameEquipmentDataLoaded;

    void Awake()
    {
        m_GameDataManager = GetComponent<GameDataManager>();
    }
    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnEnable()
    {
        //SettingsScreen.SettingsShown += OnSettingsShown;
        //CharScreen.InventoryShown += OnInventoryShow;

        //SettingsScreen.SettingsUpdated += OnSettingsUpdated;
        //InventoryUI.InventoryUpdated += OnInventoryUpdated;
        //InventoryUI.EquipmentUpdated += OnEquipmentUpdated;

        //GameScreenController.SettingsUpdated += OnSettingsUpdated;

    }
    void OnDisable()
    {
        //SettingsScreen.SettingsShown -= OnSettingsShown;
        //CharScreen.InventoryShown -= OnInventoryShow;

        //SettingsScreen.SettingsUpdated -= OnSettingsUpdated;
        //InventoryUI.InventoryUpdated -= OnInventoryUpdated;
        //InventoryUI.EquipmentUpdated -= OnEquipmentUpdated;

        //GameScreenController.SettingsUpdated -= OnSettingsUpdated;

    }
    public GameData NewGame()
    {
        return new GameData();
    }

    public void LoadGame()
    {
        Debug.LogError("LoadGame");
        // load saved data from FileDataHandler

        if (m_GameDataManager.GameData == null)
        {
            if (m_DebugValues)
            {
                Debug.Log("GAME DATA MANAGER LoadGame: Initializing game data.");
            }

            m_GameDataManager.GameData = NewGame();
            return;
        }

        if (FileManager.LoadFromFile(m_SaveFilename, out var jsonString))
        {
            m_GameDataManager.GameData.LoadJson(jsonString);

            if (m_DebugValues)
            {
                Debug.Log("SaveManager.LoadGame: " + m_SaveFilename + " json string: " + jsonString);
            }
        }
        // notify other game objects 
        if (m_GameDataManager.GameData != null)
        {
            GameDataLoaded?.Invoke(m_GameDataManager.GameData);
        }

        /// Load Inventory
        if (FileManager.LoadFromFile(m_SaveFilenameInventory, out var jsonStringInventory))
        {
            m_GameDataManager.InventoryData.LoadJson(jsonStringInventory);

            if (m_DebugValues)
            {
                Debug.Log("SaveManager.LoadGame: " + m_SaveFilenameInventory + " json string: " + jsonStringInventory);
            }
        }

        // notify other game objects 
        if (m_GameDataManager.InventoryData != null)
        {
            GameInventoryDataLoaded?.Invoke(m_GameDataManager.InventoryData);
        }

        /// Load Equipment
        if (FileManager.LoadFromFile(m_SaveFilenameEquipment, out var jsonStringEquipment))
        {
            m_GameDataManager.EquipmentData.LoadJson(jsonStringEquipment);

            if (m_DebugValues)
            {
                Debug.Log("SaveManager.LoadGame: " + m_SaveFilenameEquipment + " json string: " + jsonStringInventory);
            }
        }

        // notify other game objects 
        if (m_GameDataManager.EquipmentData != null)
        {
            GameEquipmentDataLoaded?.Invoke(m_GameDataManager.EquipmentData);
        }
    }

    public void SaveGame()
    {
        Debug.LogError("SaveGame");
        string jsonFile = m_GameDataManager.GameData.ToJson();

        // save to disk with FileDataHandler
        if (FileManager.WriteToFile(m_SaveFilename, jsonFile) && m_DebugValues)
        {
            Debug.Log("SaveManager.SaveGame: " + m_SaveFilename + " json string: " + jsonFile);
        }

        string jsonFileInventory = m_GameDataManager.InventoryData.ToJson();
        if (FileManager.WriteToFile(m_SaveFilenameInventory, jsonFileInventory) && m_DebugValues)
        {
            Debug.Log("SaveFilenameInventory.SaveGame: " + m_SaveFilenameInventory + " json string: " + jsonFileInventory);
        }

        string jsonFileEquipment = m_GameDataManager.EquipmentData.ToJson();

        if (FileManager.WriteToFile(m_SaveFilenameEquipment, jsonFileEquipment) && m_DebugValues)
        {
            Debug.Log("SaveFilenameEquipment.SaveGame: " + m_SaveFilenameEquipment + " json string: " + jsonFileEquipment);
        }
    }
}
