
namespace MJ.Manager
{
    using UnityEngine;
    using System.IO;
    using System.Text;

    public static class PlayMapDataManager
    {
        private static string MapSavePath;
        public static bool HasData => File.Exists(MapSavePath);

        static PlayMapDataManager()
        {
            MapSavePath = Path.Combine(Application.persistentDataPath, "MapData");
        }

        public static void DeleteData()
        {
            //PlayerPrefs.DeleteKey(MapSaveKey);
            File.Delete(MapSavePath);
        }

        public static void SaveData(PlayData _Data)
        {
            var data = JsonUtility.ToJson(_Data);

            FileStream fileStream = new FileStream(MapSavePath, FileMode.Create);
            var bytes = Encoding.UTF8.GetBytes(data);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }


        public static PlayData GetData()
        {
            //if(PlayerPrefs.HasKey(MapSaveKey))
            //{
            //    var data = PlayerPrefs.GetString(MapSaveKey);
            //    return JsonUtility.FromJson<PlayData>(data);
            //}
            //else
            //{
            //    return null;
            //}
            if(!File.Exists(MapSavePath))
            {
                return null;
            }

            var data = File.ReadAllText(MapSavePath);
            return JsonUtility.FromJson<PlayData>(data);
        }
    }
}

