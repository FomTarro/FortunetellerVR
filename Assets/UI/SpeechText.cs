using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Text))]
public class SpeechText : MonoBehaviour{

    [SerializeField]
    private AudioClip[] _voiceSamples;

    private AudioSource _voicePlayer;

    private Text _text;

    private bool _finishedTalking = true;
    public bool FinishedTalking
    {
        get { return _finishedTalking; }
    }


	// Use this for initialization
	void Awake () {

        _voicePlayer = GetComponent<AudioSource>();
        _text = GetComponent<Text>();
        //StartCoroutine(SayText("This has been a test..."));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetText(string textToSet)
    {
        _text.text = textToSet;
    }

    public IEnumerator SayText(string textToSay)
    {
        _finishedTalking = false;
        _text.text = "";
        char[] charToSay = textToSay.ToCharArray();
        for(int i = 0; i < textToSay.Length; i++)
        {

            _text.text += charToSay[i];
            if (charToSay[i] == '.' || charToSay[i] == '!' || charToSay[i] == '?' || charToSay[i] == ',' || charToSay[i] == '\n')
            {
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                _voicePlayer.clip = _voiceSamples[Random.Range(0, _voiceSamples.Length)];
                _voicePlayer.Play();
                yield return new WaitForSeconds(0.095f);
            }
        }
        yield return new WaitForSeconds(0.75f);
        _finishedTalking = true;
        yield return null;
    }
}
