using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM_存取設定條 : MonoBehaviour
{

    public GameObject 背景音量;
    public Text 背景T;
    public GameObject 特效音量;
    public Text 特效T;
    public GameObject 視角速度;
    public Text 視角T;

    // Use this for initialization
    void Start()
    {
        背景音量.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AudioBack", 0.8f); //背景音樂
        特效音量.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AudioEffects", 0.5f); //特效音樂
        視角速度.GetComponent<Slider>().value = PlayerPrefs.GetFloat("PerspectiveMove", 0.7f); //視角速度
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void 背景音樂(float newVolume)
    {
        背景T.text = ((int)(newVolume * 100)).ToString();
        PlayerPrefs.SetFloat("AudioBack", newVolume);
        PlayerPrefs.Save();
    }
    public void 特效音樂(float newVolume)
    {
        特效T.text = ((int)(newVolume * 100)).ToString();
        PlayerPrefs.SetFloat("AudioEffects", newVolume);
        PlayerPrefs.Save();
    }
    public void 視角移動(float newVolume)
    {
        視角T.text = ((int)(newVolume * 100)).ToString();
        PlayerPrefs.SetFloat("PerspectiveMove", newVolume);
        PlayerPrefs.Save();
    }
}
