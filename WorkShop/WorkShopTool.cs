using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace NextFrameworkForYao.WorkShop;

public static class WorkshopTool
{
  private static string workshopRootPath;
  private static string disableModPath;
  private static List<string> disableMods;

  public static string WorkshopRootPath
  {
    get
    {
      if (string.IsNullOrEmpty(WorkshopTool.workshopRootPath))
        WorkshopTool.workshopRootPath = Application.dataPath + "/../../../workshop/content/1915510";
      return WorkshopTool.workshopRootPath;
    }
  }

  public static string DisableModPath
  {
    get
    {
      if (string.IsNullOrEmpty(WorkshopTool.disableModPath))
        WorkshopTool.disableModPath = Application.dataPath + "/../DontLoadModsList.txt";
      return WorkshopTool.disableModPath;
    }
  }

  public static DirectoryInfo GetModDirectory(string workshopID) => new DirectoryInfo(WorkshopTool.WorkshopRootPath + "/" + workshopID);

  public static List<DirectoryInfo> GetAllModDirectory()
  {
    List<DirectoryInfo> allModDirectory = new List<DirectoryInfo>();
    DirectoryInfo directoryInfo = new DirectoryInfo(WorkshopTool.WorkshopRootPath);
    Debug.Log("f1 "+directoryInfo.Name);
    
    if (directoryInfo.Exists)
    {
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        foreach (FileSystemInfo file in directory.GetFiles())
        {
          if (file.Name == "Mod.bin")
          {
            allModDirectory.Add(directory);
            break;
          }
        }
      }
    }

    foreach (var abc in allModDirectory)
    {
      Debug.Log("abc: "+abc.Name);
    }
    return allModDirectory;
  }

  public static List<DirectoryInfo> GetAllModChildDirectoryByName(string directoryName)
  {
    List<DirectoryInfo> childDirectoryByName = new List<DirectoryInfo>();
    foreach (DirectoryInfo directoryInfo in WorkshopTool.GetAllModDirectory())
    {
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        if (directory.Name == directoryName)
        {
          childDirectoryByName.Add(directory);
          break;
        }
      }
    }
    return childDirectoryByName;
  }

  public static bool CheckModIsDisable(string workshopID)
  {
    if (WorkshopTool.disableMods == null)
      WorkshopTool.InitDisableMods();
    return WorkshopTool.disableMods.Contains(workshopID);
  }

  private static void InitDisableMods()
  {
    Debug.Log("444");
    WorkshopTool.disableMods = new List<string>();
    if (!File.Exists(WorkshopTool.DisableModPath))
      return;
    Debug.Log("555");
    foreach (string readAllLine in File.ReadAllLines(WorkshopTool.DisableModPath))
    {
      Debug.Log("666");
      if (!string.IsNullOrWhiteSpace(readAllLine))
        WorkshopTool.disableMods.Add(readAllLine);
    }
  }

  public static void OpenMod(string workshopID)
  {
    if (WorkshopTool.disableMods == null)
      WorkshopTool.InitDisableMods();
    if (!WorkshopTool.disableMods.Contains(workshopID))
      return;
    WorkshopTool.disableMods.Remove(workshopID);
    File.WriteAllLines(WorkshopTool.disableModPath, (IEnumerable<string>) WorkshopTool.disableMods);
  }

  public static void CloseMod(string workshopID)
  {
    if (WorkshopTool.disableMods == null)
      WorkshopTool.InitDisableMods();
    if (WorkshopTool.disableMods.Contains(workshopID))
      return;
    WorkshopTool.disableMods.Add(workshopID);
    File.WriteAllLines(WorkshopTool.disableModPath, (IEnumerable<string>) WorkshopTool.disableMods);
  }

  /*public static string GetModsDetails()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("创意工坊文件夹的Mod列表:");
    foreach (DirectoryInfo directoryInfo in WorkshopTool.GetAllModDirectory())
    {
      string path = directoryInfo.FullName + "/Mod.bin";
      if (File.Exists(path))
      {
        try
        {
          WorkShopItem workShopItem1 = new WorkShopItem();
          FileStream serializationStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
          WorkShopItem workShopItem2 = (WorkShopItem) new BinaryFormatter().Deserialize((Stream) serializationStream);
          serializationStream.Close();
          stringBuilder.AppendLine(workShopItem2.Title + "\tPath:" + directoryInfo.FullName);
        }
        catch
        {
          stringBuilder.AppendLine("未成功读取的Mod.bin\tPath:" + directoryInfo.FullName);
        }
      }
    }
    stringBuilder.AppendLine("本地测试的Mod列表:");
    DirectoryInfo directoryInfo1 = new DirectoryInfo(Application.dataPath + "/../本地Mod测试");
    if (directoryInfo1.Exists)
    {
      foreach (DirectoryInfo directory in directoryInfo1.GetDirectories())
        stringBuilder.AppendLine(directory.FullName);
    }
    return stringBuilder.ToString();
  }*/
}