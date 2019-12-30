using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public enum State
    {
        Spawn,
        Sprout,
        Young,
        Grown,
        Done,
        Seed
    }

    public State state;

    [Header("Para Tweakear los datos de la distribución normal")]
    public float stdPosition = 2f;
    public float stdGrassStats = 0.2f;

    [Header("Datos/Stats de la planta")]
    [SerializeField]
    public int grassGen;

    [SerializeField]
    float numberOfSeeds;
    [SerializeField]
    float seedSize;
    [SerializeField]
    float waterResistance;
    [SerializeField]
    float growthTime;

    private float sphereRadius;

    //Variables para almacenar información del color del material de la planta.
    Color32 grassColor;
    float numberToColor;
    float sizeToColor;
    float resistanceToColor;
    float colorAlpha;

    public float timeBetweenStates = 2f;
    bool isCoroutineStatesStarted = false;

    public void Awake()
    {
        sphereRadius = gameObject.GetComponent<BoxCollider>().bounds.extents.y * 1.41f;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

        state = State.Spawn;
        StateHandler();
    }

    void StateHandler()
    {
        //Esto es un controlador de estados. Cada estado activa cierta parte del código (por lo menos en teoría).
        //El primer estado es Spawn y se queda ahí hasta que se cambie a otro estado.
        switch (state)
        {
            case State.Spawn:
                CheckSpawnPoint();
                colorAlpha = 255f;
                isCoroutineStatesStarted = true;
                break;
            case State.Sprout:
                //Switch 3dModel to sprout
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                AssignColor();
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                transform.position = new Vector3(transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
                break;
            case State.Young:
                //Switch 3dModel to young plant
                transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                transform.position = new Vector3(transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
                break;
            case State.Grown:
                //Switch 3dModel to grown plant
                transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                transform.position = new Vector3(transform.position.x, transform.localScale.y * 0.5f, transform.position.z);
                break;
            case State.Done:
                StartCoroutine(TimeForNextState(timeBetweenStates * 2f));
                Debug.Log("State Done!");
                //No hace nada. Es para alivianar el rendimiento.
                break;
            case State.Seed:
                Debug.Log("State Seed!");
                NewSeed();
                state = State.Done;
                break;
        }
    }

    void Update()
    {
        if (isCoroutineStatesStarted)
        {
            isCoroutineStatesStarted = false;
            StartCoroutine(TimeForNextState(timeBetweenStates));
        }

        //Mientras esté apretado espacio se activa la corrutina "AutomateSeeds"
        //A pesar de ser llamado varias veces por segundo, la corrutina solo responde una vez al ser llamado 
        //y hay que esperar a que termine de correr el código. Por esto mismo se usa para cooldowns, temporizadores, etc.

        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(AutomateSeeds());
        }
        else
        {
            StopCoroutine(AutomateSeeds());
        }
    }



    // La corrutina. Espera 0.5 segundos para correr el código.
    IEnumerator AutomateSeeds()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        NewSeed();
    }

    IEnumerator TimeForNextState(float time)
    {
        yield return new WaitForSeconds(CustomMath.NextGaussian(time, 2f));
        state += 1;
        if ((int)state < 5)
        {
            isCoroutineStatesStarted = true;
            StateHandler();
        }
        else if (state == State.Seed)
        {
            isCoroutineStatesStarted = true;
            StateHandler();
        }
    }

    //Esta es la esfera que detecta si hay algo dentro del área
    public void CheckSpawnPoint()
    {
        RaycastHit hit;
        if (Physics.SphereCast(gameObject.transform.position, sphereRadius, transform.forward, out hit))
        {
            if (hit.collider.gameObject.GetComponent<Grass>() != null)
            {
                if (seedSize > hit.collider.gameObject.GetComponent<Grass>().seedSize && (int)state > (int)hit.collider.gameObject.GetComponent<Grass>().state)
                {
                    Destroy(hit.collider.gameObject);
                }
                else if (seedSize < hit.collider.gameObject.GetComponent<Grass>().seedSize && (int)state > (int)hit.collider.gameObject.GetComponent<Grass>().state)
                {
                    Destroy(hit.collider.gameObject);
                }
                else if (seedSize > hit.collider.gameObject.GetComponent<Grass>().seedSize && (int)state < (int)hit.collider.gameObject.GetComponent<Grass>().state)
                {
                    Destroy(gameObject);
                }
                else if(seedSize < hit.collider.gameObject.GetComponent<Grass>().seedSize && (int)state < (int)hit.collider.gameObject.GetComponent<Grass>().state)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    //Muestra un "dibujo" del área de detección de la planta
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gameObject.transform.position, sphereRadius);
    }

    //Acá se llaman 2 métodos. CalculateSeedValues para mutar los datos de la planta y SpawnGrass para poner la nueva semilla en el mapa
    public void NewSeed()
    {
        grassGen++;

        CalculateSeedValues();

        SpawnGrass(numberOfSeeds, seedSize, waterResistance, growthTime, grassGen);

        //1% chance of survival
        /*if (Random.Range(0f, 1f) > 0.01f && grassGen != 0)
        {
            // A veces se clona el objecto destruido (que desactiva el script antes de destruirse) por lo que debo crear una forma de evitar que se desactiven los scripts
            Destroy(this.gameObject);
        }*/
    }

    public void CalculateSeedValues()
    {
        //Mutate Values
        var newNumber = Mathf.Round(Mathf.Min(Mathf.Max(numberOfSeeds + CustomMath.NextGaussian(0f, stdGrassStats), 1), 3));
        var newSize = Mathf.Round(Mathf.Min(Mathf.Max(seedSize + CustomMath.NextGaussian(0f, stdGrassStats), 1), 13 - newNumber));
        var newResistance = Mathf.Round(Mathf.Min(Mathf.Max(seedSize + CustomMath.NextGaussian(0f, stdGrassStats), 1), 14 - newSize));
        var newTime = Mathf.Round(15 - (newNumber + newSize + newResistance));

        numberOfSeeds = newNumber;
        seedSize = newSize;
        waterResistance = newResistance;
        growthTime = newTime;

        //Asign Color Value
        AssignColor();
    }

    //Solo le asigna el color a la planta
    public void AssignColor()
    {
        numberToColor = 50f / numberOfSeeds;
        sizeToColor = 200f / seedSize;
        resistanceToColor = 50f / waterResistance;

        grassColor = new Color32((byte)numberToColor, (byte)sizeToColor, (byte)resistanceToColor, (byte)colorAlpha);
    }

    //Donde ocurre la magia
    GameObject SpawnGrass(float number = 3f, float size = 4f, float resistance = 4f, float time = 4f, int gen = 0)
    {
        var halfHeight = gameObject.GetComponent<BoxCollider>().bounds.extents.y;
        if (gen == 0)
            halfHeight = 0.4f;

        //CustomMath es una clase pública que ojalá se agrande en el proceso del proyecto
        //Por ahora solo contiene NextGaussian

        //PlantDictionary.SpawnPlant toma 3 variables. La primera es un string, para saber qué planta espawnear. La segunda es posición y la tercera rotación.

        GameObject seed = PlantDictionary.SpawnPlant("Grass", new Vector3(CustomMath.NextGaussian(transform.position.x, stdPosition), 
            halfHeight, CustomMath.NextGaussian(transform.position.z, stdPosition)), Vector3.up * Random.Range(0, 360));

        //Esto es para tomar los datos que nos dieron al crear la semilla y pasarlos a nuestras variables base, finalizando el proceso de mutación.
        seed.GetComponent<Grass>().numberOfSeeds = number;
        seed.GetComponent<Grass>().seedSize = size;
        seed.GetComponent<Grass>().waterResistance = resistance;
        seed.GetComponent<Grass>().growthTime = time;
        seed.GetComponent<Grass>().grassGen = gen;

        seed.transform.GetChild(0).GetComponent<Renderer>().material.color = grassColor;


        //Esto es solo para darle un nombre distinto a "Grass(clone)(clone)(clone)..." a las distintas instancias.
        if (grassGen == 0)
        {
            seed.name = "(Grass) Parent 0";
        }
        else
        {
            seed.name = "(Grass) Child of Gen " + (grassGen - 1) + " - This is Gen " + grassGen;
        }
        return seed;
    }

    public void Default()
    {
        //Resets all values to default
        grassGen = -1;
        numberOfSeeds = 3f;
        seedSize = 4f;
        waterResistance = 4f;
        growthTime = 4f;

        grassColor = new Color32(33, 198, 38,255);

        transform.position = new Vector3(0f, 0.5f, 0f);
        transform.rotation = Quaternion.identity;
    }
}

