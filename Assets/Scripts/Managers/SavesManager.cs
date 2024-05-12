using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意这个using
using System.Linq;
using System.IO;

public class SavesManager : MonoBehaviour
{
    public static SavesManager instance;

    //游戏数据
    private GameData gameData;

    //储存所有相关接口，方便批量处理；注意列表（以List<DataType>方式创建）和数组（以DataType[]方式创建）的区别，前者可对元素进行增删操作等
    public List<ISavesManager> savesManagers;
    //存档处理器
    private FileDataHandler dataHandler;
    //存档名称
    [SerializeField] private string saveFileName;

    //使得我们可以在Unity内SaveManager脚本处右键选择"Delete Saved File"清除存档，便于测试
    [ContextMenu("Delete Saved File")]
    public void DeleteSavedGameDate()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);
        dataHandler.DeleteSavedGameData();
    }

    private void Awake()
    {
        //确保管理器仅有一个
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        //传入存档文件路径与存档文件名；Application.persistentDataPath对应不同系统的不同路径，windows则在AppData\Localow\DefaultCompany\xxxxx里
        dataHandler = new FileDataHandler(Application.persistentDataPath, saveFileName);

        //记录所有接口
        savesManagers = FindAllSavesManagers();

        //开始的时候加载游戏存档，若没有，则新建游戏存档
        LoadGame();
    }

    #region BasicSavesManagement
    public void NewGame()
    //新建游戏，意味着新建一个可读取的存档数据
    {
        //新建一个游戏存档的数据
        gameData = new GameData();
    }
    public void LoadGame()
    {
        //读取数据
        gameData = dataHandler.LoadGameData();

        if(this.gameData == null)
        {
            Debug.Log("No Saved Data Found!");
            
            //开启新游戏
            NewGame();
        }

        if(this.gameData != null)
        {
            foreach (ISavesManager _savesManager in savesManagers)
            {
                //此处只需让数据被读取即可，不需要传入引用
                _savesManager.LoadData(gameData);
            }
        }
    }
    public void SaveGame()
    //储存游戏
    {
        foreach(ISavesManager _savesManager in savesManagers)
        //将存储在此Manager内的gameData传给所有用到存档接口的类内
        {
            //传入引用，才能使得对象能被修改
            _savesManager.SaveData(ref gameData);
        }

        //在循环之后存储存档数据
        dataHandler.SaveGameData(gameData);

        Debug.Log("Game Saved!");
    }
    private void OnApplicationQuit()
    //即当退出程序的时候，自动存储一下游戏数据，防止数据丢失
    {
        SaveGame();
    }
    #endregion

    #region Interfaces
    private List<ISavesManager> FindAllSavesManagers()
    {
        //找到所有继承了MonoBehaviour与ISavesManager的类
        IEnumerable<ISavesManager> _savesManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISavesManager>();

        //返回找到的所有使用了存档接口的类；注意返回的是new出来的的一个列表
        return new List<ISavesManager>(_savesManagers);
    }
    #endregion

    public bool WhetherHasSavedGameData()
    //检测是否有已保存的游戏数据
    {
        if(dataHandler.LoadGameData() != null)
        {
            return true;
        }

        return false;
    }
}
