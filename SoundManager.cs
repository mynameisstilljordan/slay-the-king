using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static AudioClip _sword, _button, _win, _fail, _mastery;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start() {
        _sword = Resources.Load<AudioClip>("sword");
        _button = Resources.Load<AudioClip>("button");
        _win = Resources.Load<AudioClip>("win");
        _fail = Resources.Load<AudioClip>("fail");
        _mastery = Resources.Load<AudioClip>("mastery");
        audioSrc = GetComponent<AudioSource>();
    }

    //this method plays the inputted sound
    public static void PlaySound(string clip) {
        if (PlayerPrefs.GetInt("sound", 1) == 1) {
            switch (clip) {
                case "sword":
                    audioSrc.PlayOneShot(_sword);
                    break;
                case "button":
                    audioSrc.PlayOneShot(_button);
                    break;
                case "win":
                    audioSrc.PlayOneShot(_win);
                    break;
                case "fail":
                    audioSrc.PlayOneShot(_fail);
                    break;
                case "mastery":
                    audioSrc.PlayOneShot(_mastery);
                    break;
            }
        }
    }
}