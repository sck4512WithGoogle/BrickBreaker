
namespace MJ.Manager
{
    using UnityEngine;
    public static class PlayMapDataManager
    {
        private static readonly string MapSaveKey = "MapData";

        public static void DeleteData()
        {
            PlayerPrefs.DeleteKey(MapSaveKey);
        }

        public static void SaveData(PlayData _Data)
        {
            var data = JsonUtility.ToJson(_Data);
            PlayerPrefs.SetString(MapSaveKey, data);
        }

        public static bool HasData => PlayerPrefs.HasKey(MapSaveKey);

        public static PlayData GetData()
        {
            if(PlayerPrefs.HasKey(MapSaveKey))
            {
                var data = PlayerPrefs.GetString(MapSaveKey);
                return JsonUtility.FromJson<PlayData>(data);
            }
            else
            {
                return null;
            }
        }
    }
}

