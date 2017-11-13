using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tree node which holds the path string for the tower prefab
/// </summary>
public class TowerData
{
    private Towers id;
    public Towers Id
    {
        get { return id;}
        set { id = value;}
    }
    private string path;
    public string Path
    {
        get { return path;}
        set { path = value;}
    }

    public TowerData(string path)
    {
        this.path = path;
        this.id = Towers.Root;
    }
    public TowerData(string path, Towers id)
    {
        this.path = path;
        this.id = id;
    }

    private static bool calculateEquals(TowerData obj1, TowerData obj2)
    {
        if (obj1.path.Equals(obj2.path))
        {
            return true;
        }
        return false;
    }

    public static bool operator ==(TowerData obj1, TowerData obj2)
    {
        return calculateEquals(obj1, obj2);
    }
    
    public static bool operator !=(TowerData obj1, TowerData obj2)
    {
        return !calculateEquals(obj1, obj2);
    }

    public override bool Equals(object obj)
    {
        return calculateEquals(this, obj as TowerData);
    }
    public override int GetHashCode() 
    {
        return path.GetHashCode();
    }
}

/// <summary>
/// Tree structure which holds the upgrade tree of the towers
/// </summary>
public class TowerNode<T> where T : TowerData
{
    private T data;
    private List<TowerNode<T>> children;

    public TowerNode(T data)
    {
        this.data = data;
        children = new List<TowerNode<T>>();
    }
	
    public void AddNode(T data)
    {
        children.Add(new TowerNode<T>(data));
    }

    public int GetChildrenCount()
    {
        return children.Count;
    }

    public TowerNode<T> GetChild(int i)
    {
        if (i < children.Count && i >= 0)
            return children[i];
        return null;
    }

    public TowerNode<T> this[Towers id]
    {
        get 
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].data.Id == id)
                {
                    return children[i];
                }
                else
                {
                    TowerNode<T> find = children[i][id];
                    if (find != null)
                    {
                        return find;
                    }
                }
            }
            return null; 
        }
    }
    public TowerNode<T> this[T dataValue]
    {
        get 
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].data == dataValue)
                {
                    return children[i];
                }
                else
                {
                    TowerNode<T> find = children[i][dataValue];
                    if (find != null)
                    {
                        return find;
                    }
                }
            }
            return null; 
        }
    }
}
