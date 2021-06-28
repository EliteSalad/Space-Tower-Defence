using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public enum ReturnCode
{
    True,
    False,
    Invalid,
    Correct,
    Incorrect,
    Incorrect_Format,
    Incorrect_Syntax
}

public class Database_Control : MonoBehaviour {

    public Game_State GameState = new Game_State();
    public Leaderboard leaderboard = new Leaderboard();
    public Store store = new Store();

    internal Debug_Log d;

    [Header("In-Game Debugging")]
    private Text text;

    #region General Initialisation
    void Awake()
    {
        string Save_Path = Application.persistentDataPath;
        text = gameObject.transform.parent.GetComponent<Text>();
        d = new Debug_Log(text, Save_Path);
        //Initialising the local cache in awake due to constructor
        d.DUI = SystemInfo.deviceUniqueIdentifier;
        d.Log(true, "Unique Client ID: " + d.DUI, true);
        store.d.Clone(d);
        leaderboard.d.Clone(d);
        GameState.d.Clone(d);
        FileStream file;
        if (!File.Exists(Save_Path + "/" + store.File_Name))
        {
            file = new FileStream(Save_Path + "/" + store.File_Name, FileMode.OpenOrCreate);
            file.Close();
        }
        if (!File.Exists(Save_Path + "/" + leaderboard.File_Name))
        {
            file = new FileStream(Save_Path + "/" + leaderboard.File_Name, FileMode.OpenOrCreate);
            file.Close();
        }
        if (!File.Exists(Save_Path + "/" + GameState.File_Name))
        {
            file = new FileStream(Save_Path + "/" + GameState.File_Name, FileMode.OpenOrCreate);
            file.Close();
        }
        leaderboard.Load();
        store.Load();
        GameState.Load();
    }
    #endregion
}

public class Database_Interaction
{
    #region Database Variables
    private MySqlConnection connection = null;
    private MySqlCommand command = null;
    private string connectionString = "Server=den1.mysql5.gear.host;Port=3306;Database=stddb;Uid=stdclient;Pwd=8ch8J5PPRRCFKp6!;";
    #endregion

    #region Query Database
    /// <summary>
    /// Returns the data set related to the query submitted.
    /// </summary>
    /// <param name="query">The query being submitted.</param>
    /// <returns></returns>
    public MySqlDataReader queryDatabase(string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            return command.ExecuteReader();
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database query failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database query failed (System Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return null;
        }
    }
    /// <summary>
    /// Returns a ReturnCode enum value with the response from the database.
    /// </summary>
    /// <param name="query">Query to be checked</param>
    /// <returns>ReturnCode enum value</returns>
    public ReturnCode checkDatabase(string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            if (command.ExecuteReader().Read())
                return ReturnCode.True;
            else
                return ReturnCode.False;
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database check failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);
            return ReturnCode.Incorrect_Syntax;
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database check failed (System Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return ReturnCode.Invalid;
        }
    }
    /// <summary>
    /// Queries the database for type T and returns it.
    /// </summary>
    /// <typeparam name="T">The type of the value you want to return.</typeparam>
    /// <param name="value">The name of the field you would like returned.</param>
    /// <param name="query">The query to submit to the database.</param>
    /// <returns>The value with the name that you request, converted to the type you requested.</returns>
    public T getDatabaseValue<T>(string value, string query)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (T)reader[value];
            }
            else
                return default(T);
        }
        catch (MySqlException SQLex)
        {
            Debug.Log("Database get failed (SQL Exception), query: " + query);
            Debug.Log("Exception Message: " + SQLex.Message);   
            return default(T);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Database get failed (System Exception), query: " + query);
            Debug.Log("Exception Message: " + ex.Message);
            return default(T);
        }
    }
    #endregion
}
public class Debug_Log
{
    static bool Allow_Logs = true;//Change this to false if you no longer want anything to be logged at all
    private bool Override = false;//Change this to true if you want to print all logs to the debug_log text file
    private bool Reset_On_Start = true;//Change this to true if you want the debug log file to clear each time it's run (recommended for ease of reading)

    internal string path = "";
    internal string DUI = "";
    private string Debug_File_Name = "debug_log";
    private string Debug_File_Ext = ".txt";

    public Text TextOutput;

    public Debug_Log()
    {
    }
    public Debug_Log(Text text_, string Path_)
    {
        TextOutput = text_;
        path = Path_;
        if (Reset_On_Start)
            File.WriteAllText(Path.Combine(path, Debug_File_Name + Debug_File_Ext), String.Empty);
    }

    //public void Log(bool success, string output, bool toConsole, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string Caller = null)
    public void Log(bool success, string output, bool toConsole, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
    {
        if (!Allow_Logs) return;
        TextOutput.text = TextOutput.text + "\n" + ((success) ? " " : "!") + DateTime.Now + " Caller: " + caller + " | Line Number: " + lineNumber.ToString() + ": " + output;
        if (toConsole || Override) Debug.Log(output);
        try
        {
            StreamWriter writer = new StreamWriter(path + "/" + Debug_File_Name + Debug_File_Ext, true);
            writer.WriteLine(((success) ? " " : "!") + DateTime.Now + " Caller: " + caller + " | Line Number: " + lineNumber.ToString() + ": " + output);
            writer.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Debug logging failed > " + e.Message);
        }
    }
    public List<string> Retrieve()
    {
        if (!Allow_Logs) return null;
        List<string> temp = new List<string>();
        try
        {
            StreamReader reader = new StreamReader(path + "/" + Debug_File_Name + Debug_File_Ext);
            temp.Add(reader.ReadLine());
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Reading from debug log file failed! > " + e.Message);
        }
        return temp;
    }
    public void Clone(Debug_Log dl)
    {
        path = dl.path;
        DUI = dl.DUI;
        TextOutput = dl.TextOutput;
    }
}
public class Local_Cache : Database_Interaction
{
    internal Debug_Log d = new Debug_Log();
    
    internal string File_Name = "";

    internal FileStream file;

    // This is the time of the last game state update currently held in the games memory.
    internal DateTime Modified = new DateTime();

    public virtual void Save()
    {
        // Save data to binary file.
        // ( Overidden by derived class )
    }
    /// <summary>
    /// Load data from local cache unless it's older than the database.
    /// </summary>
    public virtual void Load()
    {
        // Load from either the database or binary file (whichever is the most up to date).
        // ( Overidden by derived class )
    }
    public virtual bool Update(string Key, float Value)
    {
        // Update the database with current data.
        // ( Overidden by derived class )
        return false;
    }
    internal void Get_Modified_Date()
    {
        if (!File.Exists(d.path + "/" + File_Name))
        {
            try
            {
                File.Create(d.path + "/" + File_Name);
            }
            catch (Exception e)
            {
                d.Log(false, "Creating new file for path: " + d.path + "/" + File_Name + " failed! > " + e.Message, true);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            
        }
        else
        {
            d.Log(true, "Attempting to get Modified date from local cache file.", false);
            try
            {
                file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(file);
                Modified = Convert.ToDateTime(br.ReadString());
                br.Close();
                file.Close();
            }
            catch (Exception e)
            {
                d.Log(false, "Reading Modified date from .dat file failed > " + e.Message, true);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            d.Log(true, "Successfully read Modified date from local cache file.", false);
        }
    }
}

public class Store : Local_Cache
{
    public Store()
    {
        File_Name = "Store_Save.dat";
    }

    public Dictionary<string, float> d_Store = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Store contents to binary file: " + d.path + "/" + File_Name, false);
        // Save Store to binary file
        try
        {
            file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(Modified.ToString());
            bw.Write(d_Store.Count);
            foreach (var pair in d_Store)
            {
                bw.Write(pair.Key);
                bw.Write(pair.Value);
            }
            bw.Close();
            file.Close();
            d.Log(true, "Save Store contents to binary file successful: " + Path.Combine(d.path, File_Name), false);
        }
        catch (SerializationException sEx)
        {
            d.Log(false, "Save Store contents to binary file failed: " + Path.Combine(d.path, File_Name) + " > " + sEx.Message, true);
        }
        catch (Exception e)
        {
            d.Log(false, "Writing to the store .dat file failed! > " + e.Message, true);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data 
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        int Record_Count = 0;
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Store;");
        d.Log(true, "Loading Store : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ? 
            " | Attempting to restore Store from local cache." : " | Attempting to restore Store from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use local cache data
            try
            {
                file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(file);
                Modified = Convert.ToDateTime(br.ReadString());
                Record_Count = br.ReadInt32();
                if (Record_Count > 0)
                    d_Store.Clear();
                for (int i = 0; i < Record_Count; i++)
                {
                    d_Store.Add(br.ReadString(), (float)br.ReadSingle());
                }
                br.Close();
                file.Close();
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
                    if (reader == null)
                    {
                        d.Log(false,"Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD STORE!", true);
                        return;
                    }
                    d_Store.Clear();
                    while (reader.Read())
                    {
                        d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                    }
                    d.Log(true,"Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false,"Loading from database failed: " + ex.Message + " | UNABLE TO LOAD STORE!", true);
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            d.Log(true,"Loading from local cache successful: " + Path.Combine(d.path, File_Name), false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Store WHERE In_Use = 1;");
                if (reader == null)
                {
                    d.Log(false,"Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " + 
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                        BinaryReader br = new BinaryReader(file);
                        Modified = Convert.ToDateTime(br.ReadString());
                        Record_Count = br.ReadInt32();
                        if (Record_Count > 0)
                            d_Store.Clear();
                        for (int i = 0; i < Record_Count; i++)
                        {
                            d_Store.Add(br.ReadString(), (float)br.ReadSingle());
                        }
                        br.Close();
                        file.Close();
                        d.Log(true, "Loading from local cache successful.", false);
                    }
                    catch (Exception e)
                    {
                        d.Log(false,"Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD STORE!", true);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Close();
                        }
                    }
                    return;
                }
                d_Store.Clear();
                while (reader.Read())
                {
                    d_Store.Add(reader["Item_Name"].ToString(), (float)reader["Item_Price"]);
                }
                // Set the Modified date to reflect the new data set and save it to binary
                Modified = DB_Date;
                Save();
                d.Log(true,"Loading from database successful.", false);
            }
            catch (Exception e)
            {
                d.Log(false,"Loading from database failed: " + e.Message, true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Store Contents: ", false);
        foreach (KeyValuePair<string,float> entry in d_Store)
        {
            d.Log(true, "Name: " + entry.Key + " Value: " + entry.Value, false);
        }
    }
    // Update function not necessary for this class.
    
    // Access functions
    public Dictionary<string,float> store()
    {
        return (d_Store);
    }
}
public class Leaderboard : Local_Cache
{
    public Leaderboard()
    {
        File_Name = "Leaderboard_Save.dat";
    }

    internal Dictionary<string, float> d_Leaderboard = new Dictionary<string, float>();

    MySqlDataReader reader = null;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Leaderboad contents to binary file: " + d.path + "/" + File_Name, false);
        // Save Leaderboard to binary file
        try
        {
            file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(Modified.ToString());
            bw.Write(d_Leaderboard.Count);
            foreach (var pair in d_Leaderboard)
            {
                bw.Write(pair.Key);
                bw.Write(pair.Value);
            }
            bw.Close();
            file.Close();
            d.Log(true, "Save Leaderboard contents to binary file successful: " + d.path + "/" + File_Name, false);
        }
        catch (Exception e)
        {
            d.Log(false, "Save Leaderboard contents to binary file failed: " + d.path + "/" + File_Name + " > " + e.Message, true);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }
    /// <summary>
    /// Load from database unless the local cache has the data
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        int Record_Count = 0;
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Highscores;");
        d.Log(true, "Loading Leaderboard : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ?
            " | Attempting to restore Leaderboard from local cache." : " | Attempting to restore Leaderboard from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use Local cache data
            try
            {
                file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(file);
                Modified = Convert.ToDateTime(br.ReadString());
                Record_Count = br.ReadInt32();
                if (Record_Count > 0)
                    d_Leaderboard.Clear();
                for (int i = 0; i < Record_Count; i++)
                {
                    d_Leaderboard.Add(br.ReadString(), (float)br.ReadSingle());
                }
                br.Close();
                file.Close();
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score DESC LIMIT 100;");
                    if (reader == null)
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD LEADERBOARD!", true);
                        return;
                    }
                    d_Leaderboard.Clear();
                    while (reader.Read())
                    {
                        d_Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                    }
                    Modified = DB_Date;
                    d.Log(true, "Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD LEADERBOARD!", true);
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            d.Log(true, "Loading from local cache successful: " + Path.Combine(d.path, File_Name), false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Highscores ORDER BY Score DESC LIMIT 100;");
                if (reader == null)
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                        BinaryReader br = new BinaryReader(file);
                        Modified = Convert.ToDateTime(br.ReadString());
                        Record_Count = br.ReadInt32();
                        if (Record_Count > 0)
                            d_Leaderboard.Clear();
                        for (int i = 0; i < Record_Count; i++)
                        {
                            d_Leaderboard.Add(br.ReadString(), (float)br.ReadSingle());
                        }
                        br.Close();
                        file.Close();
                        d.Log(true, "Loading from local cache successful.", false);
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD LEADERBOARD!", true);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Close();
                        }
                    }
                    return;
                }
                d_Leaderboard.Clear();
                while (reader.Read())
                {
                    d_Leaderboard.Add(reader["Player_Name"].ToString(), (float)reader["Score"]);
                }
                // Set the Modified date to reflect the new data set and save it to binary
                Modified = DB_Date;
                Save();
                d.Log(true, "Loading from database successful.", false);
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from database failed: " + e.Message, true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Leaderboard Contents: ", false);
        foreach (KeyValuePair<string, float> entry in d_Leaderboard)
        {
            d.Log(true, "Name: " + entry.Key + " Score: " + entry.Value, false);
        }
    }
    /// <summary>
    /// Update database records
    /// </summary>
    /// <param name="Key">The name of the player.</param>
    /// <param name="Value">The score achieved.</param>
    /// <returns>True if updated successfully, false if key was taken.</returns>
    public override bool Update(string Key, float Value)
    {
        try
        {
            //ReturnCode check = checkDatabase("SELECT Player_Name FROM tbl_Highscores WHERE Unique_Identifier = '" + d.DUI + "';");
            //if (check == ReturnCode.True)
            //{
                d.Log(true, "Attempting to update the database with the following data > Player_Name: '" + Key + "', Score: " + Value, false);
                // Check database to see if we already hold records with this players information
                reader = queryDatabase("SELECT * FROM tbl_Highscores WHERE Unique_Identifier = '" + d.DUI + "';");
                if (reader == null)
                {
                    d.Log(false, "The MySqlDataReader returned null, please ensure you have data in the database and you are connected to the internet! | UNABLE TO UPDATE DATABASE!", true);
                    return false;
                }
                if (reader.HasRows)
                {
                    // Data was found, update it
                    d.Log(true, "Leaderboard data for user: " + Key + " was found, updating record with a new Score of " + Value, false);
                    reader = queryDatabase("UPDATE tbl_Highscores " +
                                            "SET Player_Name = '" + Key + "', " +
                                                "Score = " + Value + ", " +
                                                "DT_Stamp = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                                "WHERE Unique_Identifier = '" + d.DUI + "';");
                    d.Log(true, "Leaderboard data successfully updated > Player Name: " + Key + ", Score: " + Value + ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Unique Identifier: " + d.DUI, false);
                    return true;
                }
                else
                {
                    // No data was found, insert into
                    d.Log(true, "Leaderboard data for user: " + Key + " was not found, inserting a new record with a new Score of " + Value, false);
                    reader = queryDatabase("INSERT INTO tbl_Highscores " +
                                                "(Player_Name, Score, DT_Stamp, Unique_Identifier) " +
                                            "SELECT '" + Key + "', " +
                                                Value + ", " +
                                                "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', " +
                                                "'" + d.DUI + "';");
                    d.Log(true, "Leaderboard data successfull inserted > Player Name: " + Key + ", Score: " + Value + ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Unique Identifier: " + d.DUI, false);
                    return true;
                }
            //}
            //else
            //{
            //    // The check showed that there is already someone with that name on the Leaderboard.
            //    return false;
            //}
        }
        catch (Exception e)
        {
            d.Log(false,"Update Leaderboard data failed: " + e.Message, true);
            return false;
        }
    }

    // Access functions
    /// <summary>
    /// Submit Score and get the updated Leaderboard.
    /// </summary>
    /// <param name="Score">Score the Player achieved. (float)</param>
    /// <param name="Player_Name">Player's name. (string)</param>
    public Dictionary<string, float> leaderboard(float Score, string Player_Name)
    {
        Update(Player_Name, Score);
        Load();

        return (d_Leaderboard);
    }
    /// <summary>
    /// Updates and returns the Leaderboard from the local cache, this can be updated using LoadData and selecting true for the Leaderboard bool.
    /// </summary>
    /// <returns>The local Leaderboard cache.</returns>
    public Dictionary<string, float> leaderboard()
    {
        Load();

        return (d_Leaderboard);
    }
}
public class Game_State : Local_Cache
{
    public Game_State()
    {
        File_Name = "Game_State_Save.dat";
    }

    MySqlDataReader reader = null;

    internal int Save_ID = 0;

    /// <summary>
    /// Save to binary file
    /// </summary>
    public override void Save()
    {
        d.Log(true, "Attempting to save Game State contents to binary file: " + d.path + "/" + File_Name, false);
        // Save Game State to binary file
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(file);
            bw.Write(Modified.ToString());
            bw.Write(Current_Medals);
            bw.Write(Total_Medals_Earned);
            bw.Write(Current_Gems);
            bw.Write(Total_Gems_Earned);
            bw.Write(Level_Data.Count);
            d.Log(true, "Successfully saved Game State of > [Total_Medals_Earned: " + Total_Medals_Earned + " | Current_Medals: " + Current_Medals +
                " | Total_Gems_Earned: " + Total_Gems_Earned + " | Current_Gems: " + Current_Gems + "] to binary file: " + Path.Combine(d.path, File_Name), false);
            foreach (Level_Info level in Level_Data)
            {
                if (level.Name == "" || level.Number == 0 || level.Medals == 0)
                {
                    d.Log(false, "Level #" + level.Number + " Serialization failure, a value was null > Name: " + level.Name + ", Number: " + level.Number + ", Medals: " + level.Medals + ". Skipping this level.", true);
                    continue;
                }
                bf.Serialize(bw.BaseStream, level);
                d.Log(true, "Level #" + level.Number + " - " + level.Name + " (" + level.Medals + " Medals Earned) saved to binary file: " + Path.Combine(d.path, File_Name), true);
            }
            bw.Close();
            file.Close();
            d.Log(true, "Save Game State contents to binary file successful: " + Path.Combine(d.path, File_Name), false);
        }
        catch (SerializationException e)
        {
            d.Log(false, "Save Game State contents to binary file failed: " + Path.Combine(d.path, File_Name) + " > " + e.Message, true);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
        // Now update the database to reflect this
        Update();
    }
    /// <summary>
    /// Load from database unless the local cache has the data
    /// </summary>
    public override void Load()
    {
        Get_Modified_Date();
        DateTime DB_Date = getDatabaseValue<DateTime>("MAX(DT_Stamp)", "SELECT MAX(DT_Stamp) FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
        d.Log(true, "Loading Game State : Local Cache > " + Modified + ", Database > " + DB_Date + ((Modified >= DB_Date) ?
            " | Attempting to restore Game State from local cache." : " | Attempting to restore Game State from database."), false);
        if (Modified >= DB_Date)
        {
            #region Use Local cache data
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(file);
                Modified = Convert.ToDateTime(br.ReadString());
                Current_Medals = br.ReadByte();
                Total_Medals_Earned = br.ReadByte();
                Current_Gems = br.ReadInt32();
                Total_Gems_Earned = br.ReadInt32();
                Level_Data.Clear();
                int Level_Count = br.ReadInt32();
                for (int i = 0; i < Level_Count; i++)
                {
                    Level_Data.Add((Level_Info)bf.Deserialize(br.BaseStream));
                }
                br.Close();
                file.Close();
            }
            catch (Exception e)
            {
                d.Log(false, "Loading from local cache failed: " + e.Message + " Attempting to load from database instead.", true);
                try
                {
                    reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
                    if (reader == null)
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                            "| UNABLE TO LOAD GAME STATE!", true);
                        return;
                    }
                    if (reader.Read())
                    {
                        Current_Medals = reader.GetByte("Current_Medals");
                        Total_Medals_Earned = reader.GetByte("Total_Medals_Earned");
                        Current_Gems = reader.GetInt32("Current_Gems");
                        Total_Gems_Earned = reader.GetInt32("Total_Gems_Earned");
                        int Save_ID = reader.GetInt32("Save_ID");
                        Level_Data.Clear();
                        reader = queryDatabase("SELECT " +
                                                "tbl_Level_Save_Data.Medals_Earned AS ME, " +
                                                "tbl_Map_Data.Level_Name AS LN, " +
                                                "tbl_Map_Data.Level_Number AS MI " +
                                            "FROM tbl_Level_Save_Data " +
                                            "LEFT JOIN tbl_Map_Data " +
                                                "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Level_Number " +
                                            "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                                Level_Data.Add(new Level_Info(reader["LN"].ToString(),reader.GetByte("MI"),reader.GetByte("ME")));
                        }
                    }
                    else
                    {
                        d.Log(false, "Loading from database failed: The MySqlDataReader returned no records | UNABLE TO LOAD GAME STATE!", true);
                        return;
                    }
                    Modified = DB_Date;
                    d.Log(true, "Loading from database successful.", false);
                    return;
                }
                catch (Exception ex)
                {
                    d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD GAME STATE!", true);
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            d.Log(true, "Loading from local cache successful: " + Path.Combine(d.path, File_Name), false);
            #endregion
        }
        else
        {
            #region Use Database data
            try
            {
                reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
                if (reader == null)
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned null, please check the SQL Syntax is correct and ensure there is data to pull! " +
                        "Attempting to load from local cache instead.", true);
                    try
                    {
                        // Use local cache data instead
                        BinaryFormatter bf = new BinaryFormatter();
                        file = new FileStream(d.path + "/" + File_Name, FileMode.OpenOrCreate);
                        BinaryReader br = new BinaryReader(file);
                        Modified = Convert.ToDateTime(br.ReadString());
                        Current_Medals = br.ReadByte();
                        Total_Medals_Earned = br.ReadByte();
                        Current_Gems = br.ReadInt32();
                        Total_Gems_Earned = br.ReadInt32();
                        Level_Data.Clear();
                        int Level_Count = br.ReadInt32();
                        for (int i = 0; i < Level_Count; i++)
                        {
                            Level_Data.Add((Level_Info)bf.Deserialize(br.BaseStream));
                        }
                        d.Log(true, "Loading from local cache successful.", false);
                        br.Close();
                        file.Close();
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Loading from local cache failed: " + e.Message + " | UNABLE TO LOAD GAME STATE!", true);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Close();
                        }
                    }
                    return;
                }
                if (reader.Read())
                {
                    Current_Medals = reader.GetByte("Current_Medals");
                    Total_Medals_Earned = reader.GetByte("Total_Medals_Earned");
                    Current_Gems = reader.GetInt32("Current_Gems");
                    Total_Gems_Earned = reader.GetInt32("Total_Gems_Earned");
                    int Save_ID = reader.GetInt32("Save_ID");
                    Level_Data.Clear();
                    reader = queryDatabase("SELECT " +
                                            "tbl_Level_Save_Data.Medals_Earned AS ME, " +
                                            "tbl_Map_Data.Level_Name AS LN, " +
                                            "tbl_Map_Data.Level_Number AS MI " +
                                        "FROM tbl_Level_Save_Data " +
                                        "LEFT JOIN tbl_Map_Data " +
                                            "ON tbl_Level_Save_Data.Map_ID = tbl_Map_Data.Level_Number " +
                                        "WHERE tbl_Level_Save_Data.Save_ID = " + Save_ID + ";");
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            Level_Data.Add(new Level_Info(reader["LN"].ToString(), reader.GetByte("MI"), reader.GetByte("ME")));
                    }
                }
                else
                {
                    d.Log(false, "Loading from database failed: The MySqlDataReader returned no records | UNABLE TO LOAD GAME STATE!", true);
                    return;
                }
                Modified = DB_Date;
                d.Log(true, "Loading from database successful.", false);
                return;
            }
            catch (Exception ex)
            {
                d.Log(false, "Loading from database failed: " + ex.Message + " | UNABLE TO LOAD GAME STATE!", true);
            }
            #endregion
        }
        // For testing purposes:
        d.Log(true, "Game State Contents: ", false);
        d.Log(true, "Current Medals: " + Current_Medals + ", Total Medals Earned: " + total_Medals_Earned +
            ", Current Gems: " + Current_Gems + ", Total Gems Earned: " + Total_Gems_Earned + 
            ", Modified DateTime: " + Modified, false);
        foreach (Level_Info level in Level_Data)
        {
            d.Log(true, "Level #" + level.Number + ", '" + level.Name + "', Medals: " + level.Medals, false);
        }
    }
    /// <summary>
    /// Update database records
    /// </summary>
    /// <param name="Key">Unused</param>
    /// <param name="Value">Unused</param>
    public override bool Update(string Key = "", float Value = 0.0f)
    {
        try
        {
            d.Log(true, "Attempting to update the database with the Game State data.", false);
            // Check database to see if we already hold records with this players information
            reader = queryDatabase("SELECT * FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
            int Save_ID = 0;
            reader.Read();
            if (reader == null)
            {
                d.Log(false, "The MySqlDataReader returned null, please ensure you have data in the database and you are connected to the internet! | UNABLE TO UPDATE DATABASE!", true);
                return false;
            }
            if (reader.HasRows)
            {
                Save_ID = reader.GetInt32("Save_ID");
                // Data was found, update it
                d.Log(true, "Game State data for Unique ID: " + d.DUI + " was found, updating record.", false);
                reader = queryDatabase("UPDATE tbl_Save_Data " +
                                       "SET Current_Medals = " + Current_Medals + ", " +
                                            "Total_Medals_Earned = " + Total_Medals_Earned + ", " +
                                            "Current_Gems = " + Current_Gems + ", " +
                                            "Total_Gems_Earned = " + Total_Gems_Earned + ", " +
                                            "DT_Stamp = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                                       "WHERE Unique_Identifier = '" + d.DUI + "';");
                d.Log(true, "The following Game State data was saved successfully > Current_Medals: " + Current_Medals + ", Total_Medals_Earned: " + Total_Medals_Earned + 
                    ", Current_Gems: " + Current_Gems + ", Total_Gems_Earned: " + Total_Gems_Earned + 
                    ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Attempting to save the " + Level_Data.Count + " individual levels.", false);
                foreach (Level_Info level in Level_Data)
                {
                    try
                    {
                        d.Log(true, "Starting save of level #" + level.Number, false);
                        ReturnCode L_Exists = checkDatabase("SELECT * FROM tbl_Level_Save_Data WHERE Save_ID = " + Save_ID + " AND Map_ID = " + level.Number + ";");
                        if (L_Exists == ReturnCode.True)
                        {
                            d.Log(true, "Data found in tbl_Level_Save_Data for level #" + level.Number + ", Updating record", false);
                            reader = queryDatabase("UPDATE tbl_Level_Save_Data " +
                                                    "SET Medals_Earned = " + level.Medals + " " +
                                                    "WHERE Save_ID = " + Save_ID + " AND Map_ID = " + level.Number + ";");
                        }
                        else if (L_Exists == ReturnCode.False)
                        {
                            d.Log(true, "Data not found in tbl_Level_Save_Data for level #" + level.Number + ", Inserting record", false);
                            reader = queryDatabase("INSERT INTO tbl_Level_Save_Data " +
                                                            "(Map_ID, Save_ID, Medals_Earned) " +
                                                       "VALUES (" + level.Number + ", " + Save_ID + ", " + level.Medals + ");");
                            d.Log(true, "Level #" + level.Number + " saved successfully.", false);
                        }
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Saving level #" + level.Number + " failed: " + e.Message + ". Attempting to save the next level (if applicable).", true);
                        continue;
                    }
                }
                d.Log(true, "Game State data successfully updated.", false);
            }
            else
            {
                // No data was found, insert into
                d.Log(true, "Game State data for Unique ID: " + d.DUI + " not was found, updating record.", false);
                reader = queryDatabase("INSERT INTO tbl_Save_Data " +
                                            "(Current_Medals, Total_Medals_Earned, Current_Gems, Total_Gems_Earned, Unique_Identifier, DT_Stamp) " +
                                       "VALUES (" + Current_Medals + ", " + Total_Medals_Earned + ", " + Current_Gems + ", " + Total_Gems_Earned + ", '" + d.DUI + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "');");
                Save_ID = getDatabaseValue<int>("Save_ID", "SELECT Save_ID FROM tbl_Save_Data WHERE Unique_Identifier = '" + d.DUI + "';");
                d.Log(true, "The following Game State data was saved successfully > Current_Medals: " + Current_Medals + ", Total_Medals_Earned: " + Total_Medals_Earned +
                    ", Current_Gems: " + Current_Gems + ", Total_Gems_Earned: " + Total_Gems_Earned +
                    ", Date Time Stamp: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", Attempting to save the " + Level_Data.Count + " individual levels.", false);
                foreach (Level_Info level in Level_Data)
                {
                    try
                    {
                        d.Log(true, "Attempting to insert record for level #" + level.Number, false);
                        reader = queryDatabase("INSERT INTO tbl_Level_Save_Data " +
                                                "(Map_ID, Save_ID, Medals_Earned) " +
                                           "VALUES (" + level.Number + ", " + Save_ID + ", " + level.Medals + ");");
                        d.Log(true, "Level #" + level.Number + " saved successfully.", false);
                    }
                    catch (Exception e)
                    {
                        d.Log(false, "Saving level #" + level.Number + " failed: " + e.Message + ". Attempting to save the next level (if applicable).", true);
                        continue;
                    }
                }
                d.Log(true, "Game State data successfull inserted > ", false);
                return true;
            }
        }
        catch (Exception e)
        {
            d.Log(false, "Update Game State data failed: " + e.Message, true);
            return false;
        }
        return false;
    }

    // The amount of Medals the Player currently has.
    private byte current_Medals = 0;
    public byte Current_Medals { get { return this.current_Medals; } set { this.current_Medals = value; Modified = DateTime.Now; } }

    // The total amount of Medals the Player has earned.
    //This is probably not necessary but I wanted to put it in just in case we wanted to give the Player the ability to trade them.
    private byte total_Medals_Earned = 0;
    public byte Total_Medals_Earned { get { return this.total_Medals_Earned; } set { this.total_Medals_Earned = value; Modified = DateTime.Now; } }

    // The amount of Gems currently in the Players inventory.
    private int current_Gems = 0;
    public int Current_Gems { get { return this.current_Gems; } set { this.current_Gems = value; Modified = DateTime.Now; } }

    // The lifetime total amount of Gems the Player has earned.
    private int total_Gems_Earned = 0;
    public int Total_Gems_Earned { get { return this.total_Gems_Earned; } set { this.total_Gems_Earned = value; Modified = DateTime.Now; } }

    // List of all level saves completed (Not used for non-completed levels).
    public List<Level_Info> Level_Data = new List<Level_Info>();
}
[Serializable]
public class Level_Info
{
    // The name of the level being saved.
    private string name = "";
    public string Name { get { return name; } set { name = value; } }

    // The number of the level being saved.
    private byte number = 0;
    public byte Number { get { return number; } set { number = value; } }

    // The amount of medals earned by the player for this level.
    private byte medals = 1;
    public byte Medals { get { return medals; } set { medals = value; } }

    /// <summary>
    /// Constructor for Level_Info
    /// </summary>
    /// <param name="Name_">The name of the level being saved</param>
    /// <param name="Number_">The number of the level being saved</param>
    /// <param name="Medals_">The number of medals being saved (1 to 3)</param>
    public Level_Info(string Name_, byte Number_, byte Medals_)
    {
        name = Name_;
        number = Number_;
        medals = Medals_;
    }
}