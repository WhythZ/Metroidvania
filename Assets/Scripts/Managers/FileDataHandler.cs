using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//注意此处using
using System;
using System.IO;

public class FileDataHandler
{
    //读取文件数据首先需要文件的路径
    private string dataFileDirPath = "";
    //以及文件的名字
    private string dateFileName = "";

    public FileDataHandler(string _dataFileDirPath, string _dateFileName)
    //默认构造函数
    {
        this.dataFileDirPath = _dataFileDirPath;
        this.dateFileName = _dateFileName;
    }

    public GameData LoadGameData()
    {
        //复合路径
        string _fullPath = Path.Combine(dataFileDirPath, dateFileName);

        //在创建新的存档数据并赋值过来之前，都保持为null
        GameData _loadData = null;

        if(File.Exists(_fullPath))
        {
            try
            {
                string _dataToLoad = "";

                using(FileStream _stream = new FileStream(_fullPath, FileMode.Open))
                {
                    using(StreamReader _reader = new StreamReader(_stream))
                    {
                        _dataToLoad = _reader.ReadToEnd();
                    }
                }

                _loadData = JsonUtility.FromJson<GameData>(_dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error On Trying To Load GameData From File " + _fullPath + "\n" + e);
            }
        }

        return _loadData;
    }

    public void SaveGameData(GameData _gameData)
    {
        //复合路径
        string _fullPath = Path.Combine(dataFileDirPath, dateFileName);
    
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
        
            //第二个参数true表示美化
            string _dataToStore = JsonUtility.ToJson(_gameData, true);

            //创建
            using (FileStream _stream = new FileStream(_fullPath, FileMode.Create))
            {
                using (StreamWriter _writer = new StreamWriter(_stream))
                {
                    _writer.Write(_dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error On Trying To Save GameData To File " + _fullPath + "\n" + e);
        }
    }

    public void DeleteSavedGameData()
    {
        string _fullPath = Path.Combine(dataFileDirPath, dateFileName);

        if(File.Exists(_fullPath))
        {
            File.Delete(_fullPath);
        }
    }
}
