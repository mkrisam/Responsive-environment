using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlantDictionary
{
    //Como es una clase estática el diccionario se mantiene siempre disponible para ser referenciado
    private static Dictionary<string, GameObject> plantDictionary;

    static PlantDictionary()
    {
        var plants = Resources.LoadAll<GameObject>("Prefabs/Plants");
        plantDictionary = new Dictionary<string, GameObject>(plants.Length);

        //Esto es syntax estandar de listas,arrays, diccionarios y parecidos.
        foreach (GameObject plant in plants)
        {
            plantDictionary.Add(plant.name, plant);
        }
    }

    //Este método retorna un GameObject para hacer posible el instantiate desde la clase "Grass"
    public static GameObject SpawnPlant(string plantName, Vector3 pos, Vector3 rot)
    {

        //ContainsKey es una propiedad de Dictionary para leer los strings de los tipos guardados. En este caso los tipos son de GameObject.
        if (plantDictionary.ContainsKey(plantName))
        {
            //Crea una instancia de la referencia en el diccionario, que se llama con su nombre, que se le entrega a "plantDictionary" a través de SpawnPlant, como podemos ver en la clase "Grass".
            GameObject drop = Object.Instantiate(plantDictionary[plantName], pos, Quaternion.Euler(rot));
            return drop;
        }
        else
        {
            Debug.LogError(plantName + " could not be " + "found and spawned.");
            return null;
        }
    }
}
