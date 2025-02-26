using System.Collections.Generic;
using System.Linq;
using src.Dev;

namespace src;

public static class GameObjectManager
{
    public static readonly List<GameObject> objs = [];


    public static GameObject Create(GameObjectData data)
    {
        GameObject obj = new(data);
        objs.Add(obj);
        return obj;
    }

    public static GameObject[] CreateMany(GameObjectData[] data)
    {
        GameObject[] objs = data
            .Select(d => new GameObject(d))
            .ToArray();

        GameObjectManager.objs.AddRange(objs);

        return objs;
    }

    public static void Destroy(GameObject obj)
        => objs.Remove(obj);

    public static void DestroyAll()
        => objs.Clear();
}