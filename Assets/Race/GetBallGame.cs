using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetBallGame : MonoBehaviour
{
    public static GetBallGame game;

    public Text text;
    public float score = 0f;
    // Start is called before the first frame update
    void Start()
    {
        game = this;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = score.ToString("N2");
    }
}
