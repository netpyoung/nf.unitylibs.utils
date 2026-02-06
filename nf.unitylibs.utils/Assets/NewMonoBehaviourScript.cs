using NF.UnityLibs.Utils;
using UnityEngine;


public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO(asdf): hello
        // TODO(asdf): hello
        double x = MathUtil.Round_Normal(2.1f);
        Debug.Log(x);

        Hello();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Nodiscard]
    int Hello()
    {
        return 10;
    }
}