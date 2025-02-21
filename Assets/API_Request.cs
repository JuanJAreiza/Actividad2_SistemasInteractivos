using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class API_Request : MonoBehaviour
{
    //Pal Character
    [SerializeField] private string APIurl = "https://rickandmortyapi.com/api/character";
    [SerializeField] private RawImage characterImage;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text characterSpeciesText;


    public void RequestCharacterData(int characterId)
    {
        StartCoroutine(GetCharacter(characterId));
    }

    public void SearchCharacter()
    {
        string inputText = idInputField.text.Trim();
        Debug.Log("Texto ingresado en InputField: " + inputText);

        if (int.TryParse(inputText, out int characterId))
        {
            Debug.Log("ID convertido correctamente: " + characterId);
            StartCoroutine(GetCharacter(characterId));
        }
        else
        {
            Debug.LogError("Por favor, ingresa un ID válido.");
        }
    }

    IEnumerator GetCharacter(int id)
    {
        string url = APIurl + "/" + id;
        Debug.Log("URL de la solicitud: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
            characterNameText.text = character.name;
            characterSpeciesText.text = "Especie: " + character.species;
            StartCoroutine(GetImage(character.image));
        }
    }

    IEnumerator GetImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error cargando la imagen: " + www.error);
        }
        else
        {
            characterImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }
}


[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string image;
    public string species;
}

