using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class APIManager : MonoBehaviour
{
    [SerializeField] private string APIurl = "https://my-json-server.typicode.com/JuanJAreiza/jsonDB/users";
    [SerializeField] private TMP_Text usernameText;
    [SerializeField] private TMP_Text skillText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private RawImage[] cardImages;
    [SerializeField] private TMP_Text[] cardNumbers;
    [SerializeField] private TMP_Text[] cardNames; // 🔹 NUEVO: Texto para los nombres de los Pokémon
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    private string pokeAPIUrl = "https://pokeapi.co/api/v2/pokemon/";

    public void RequestUserData(int userId)
    {
        StartCoroutine(GetUser(userId));
    }

    IEnumerator GetUser(int id)
    {
        string url = APIurl + "/" + id;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            User user = JsonUtility.FromJson<User>(request.downloadHandler.text);
            usernameText.text = user.username;
            usernameText.color = user.state ? activeColor : inactiveColor;

            skillText.text = "Nivel: " + user.skill;

            string stateWord = user.state ? "<color=#006400>Activo</color>" : "<color=#404040>Inactivo</color>";
            stateText.text = $"Estado: {stateWord}";

            for (int i = 0; i < cardNumbers.Length; i++)
            {
                if (i < user.deck.Length)
                {
                    int cardId = user.deck[i]; // ID de la carta (Pokédex)
                    cardNumbers[i].text = cardId.ToString();
                    StartCoroutine(GetPokemonData(cardId, cardImages[i], cardNames[i])); // 🔹 Ahora también obtenemos el nombre
                }
                else
                {
                    cardNumbers[i].text = "";
                    cardImages[i].color = Color.clear;
                    cardNames[i].text = ""; // 🔹 Si no hay carta, también limpiamos el nombre
                }
            }
        }
    }

    IEnumerator GetPokemonData(int id, RawImage image, TMP_Text nameText)
    {
        string url = pokeAPIUrl + id.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error al obtener datos del Pokémon ID " + id + ": " + request.error);
        }
        else
        {
            // 🔹 Extraemos la info de la API de Pokémon
            PokemonData pokemon = JsonUtility.FromJson<PokemonData>(request.downloadHandler.text);

            // 🔹 Asignamos el nombre con la primera letra en mayúscula
            nameText.text = char.ToUpper(pokemon.name[0]) + pokemon.name.Substring(1);

            // 🔹 Cargamos la imagen
            StartCoroutine(GetPokemonImage(id, image));
        }
    }

    IEnumerator GetPokemonImage(int id, RawImage image)
    {
        string url = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{id}.png";
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error al cargar imagen de Pokémon ID " + id + ": " + request.error);
        }
        else
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.color = Color.white;
        }
    }
}

// 🔹 Clase para extraer el nombre del Pokémon de la API
[System.Serializable]
public class PokemonData
{
    public string name;
}

[System.Serializable]
public class User
{
    public int id;
    public string username;
    public bool state;
    public string skill;
    public int[] deck;
}
