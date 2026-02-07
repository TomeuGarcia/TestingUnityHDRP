using TextureGeneration;
using UnityEngine;


[CreateAssetMenu(fileName = "AlphabetSO_NAME", 
    menuName = "Scriptable Objects/AlphabetSO")]
public class AlphabetSO : ScriptableObject
{
    [SerializeField] private Alphabet _alphabet;
    public Alphabet Alphabet => _alphabet;


    private void OnValidate()
    {
        _alphabet.Validate();
    }
}
