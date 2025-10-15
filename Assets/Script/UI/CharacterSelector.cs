using UnityEngine;
using UnityEngine.Events;
public class CharacterSelector: MonoBehaviour
{
    [Header("Character")]
    public GameObject[] characters;
    private int currentIndex = 0;

    [Header("Events")]
    public UnityEvent<int> onCharacterChanged; // Event to notify character change
    public UnityEvent<int> onCharacterSelected; // Event to notify character selection


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextCharacter()
    {
        //characters[currentIndex].SetActive(false);
        currentIndex = (currentIndex + 1) % characters.Length;
        //characters[currentIndex].SetActive(true);
        //onCharacterChanged?.Invoke(currentIndex);
        UpdateCharacterDisplay();


    }

    public void PreviousCharacter()
    {
        currentIndex--;
        if(currentIndex <0) currentIndex = characters.Length - 1;
        UpdateCharacterDisplay();
    }

    public void SelectCharacter()
    {
        onCharacterSelected?.Invoke(currentIndex);
    }
    public void UpdateCharacterDisplay()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currentIndex);
        }
        onCharacterChanged?.Invoke(currentIndex);
    }
}
